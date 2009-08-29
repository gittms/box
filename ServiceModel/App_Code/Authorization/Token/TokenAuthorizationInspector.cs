using System;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Definitif.Security.Cryptography;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents Token-based authorization inspector.
    /// </summary>
    public class TokenAuthorizationInspector : IAuthorizationInspector
    {
        private ITokenAuthorizationProvider provider;
        private string sessionKey;

        /// <summary>
        /// Creates new Token-based authorization inspector.
        /// </summary>
        /// <param name="provider">Authorization provider.</param>
        /// <param name="sessionKey">Session key.</param>
        public TokenAuthorizationInspector(ITokenAuthorizationProvider provider, string sessionKey)
        {
            this.provider = provider;
            this.sessionKey = sessionKey;
        }

        /// <summary>
        /// Validates given HTTP context using given validation mode.
        /// </summary>
        /// <param name="current">HTTP context to validate.</param>
        /// <param name="mode">Validation mode to use.</param>
        /// <exception cref="WebException">On authorization fail.</exception>
        /// <returns>True if authorization succeded, overwise - WebException is thrown.</returns>
        public bool ValidateContext(HttpContext current, TokenValidationMode mode)
        {
            if (current == null)
            {
                // ASP.NET compatibility requirement is not
                // allowed for this service. This won't let
                // us use session, so username can not be
                // passed.
                throw new ArgumentNullException(
                    "HttpContext is empty. AspNetCompatibilityRequirements attribute " +
                    "must be set to Allowed or Required mode.");
            }
            else if (current != null && this.sessionKey != null &&
                current.Session[this.sessionKey] != null)
            {
                // We have request context, have Session key set
                // and user is authorized.
                return true;
            }
            else
            {
                // Skipping validation mode set to None.
                if (mode == TokenValidationMode.None) return true;

                // Signature calculation:
                // ======================
                // md5( method:Authorization-Key[:Authorization-Token|Authorization-Frob|None]:Random-Hash:Secret )
                //
                // Values:
                // ======================
                // method               = POST/GET/PUT/DELETE
                // Authorization-Key    = Application public API key
                // Authorization-Frob   = Application temporal frob
                // Authorization-Token  = Token requested during authorization
                // Random-Hash          = Random request hash
                // Secret               = Application private API key
                //
                // Example:
                // ======================
                // md5( "POST:MTruJQUcTEHprBtYzmJJrCySniIaLnl:032-123321@1112:JKHg5Msdd:lYDoks" )
                //      = "bb392ec4592f68e12875e5560d99d574"
                //
                // Security:
                // ======================
                // Random should not be equal to last one.

                System.Net.WebHeaderCollection headers
                    = WebOperationContext.Current.IncomingRequest.Headers;
                string method = WebOperationContext.Current.IncomingRequest.Method,
                       key = headers["Authorization-Key"],
                       token = headers["Authorization-Token"],
                       frob = headers["Authorization-Frob"],
                       rnd = headers["Random-Hash"],
                       sign = headers["Request-Sign"],
                       secret = "", intsign = "";

                // Handling common errors:
                //  1. Empty headers used in every request;
                //  2. Non-unique Random-Hash value.
                if (String.IsNullOrEmpty(key))
                    throw new WebException(
                        System.Net.HttpStatusCode.Unauthorized,
                        "Request must contain Authorization-Key header.");
                else if (String.IsNullOrEmpty(sign))
                    throw new WebException(
                        System.Net.HttpStatusCode.Unauthorized,
                        "Request must be signed using Request-Sign header.");
                else if (String.IsNullOrEmpty(rnd))
                    throw new WebException(
                        System.Net.HttpStatusCode.Unauthorized,
                        "Request must contain Random-Hash header.");
                else if (rnd == this.provider.GetLastRandom(key))
                    throw new WebException(
                        System.Net.HttpStatusCode.Unauthorized,
                        "Request Random-Hash header must not equal last used one.");

                // Getting secret private key for authorization
                // key provided in Authorization-Key header.
                secret = this.provider.GetSecret(key);

                switch (mode)
                {
                    case TokenValidationMode.Secret:
                        // md5( method:Authorization-Key:Random-Hash:Secret )
                        intsign = MD5.Hash(
                            method + ":" + key + ":" + rnd + ":" + secret);
                        break;
                    case TokenValidationMode.Frob:
                        if (String.IsNullOrEmpty(frob))
                            throw new WebException(
                                System.Net.HttpStatusCode.Unauthorized,
                                "Request must contain Authorization-Frob header.");
                        // md5( method:Authorization-Key:Authorization-Frob:Random-Hash:Secret )
                        intsign = MD5.Hash(
                            method + ":" + key + ":" + frob + ":" + rnd + ":" + secret);
                        break;
                    case TokenValidationMode.Token:
                        if (String.IsNullOrEmpty(token))
                            throw new WebException(
                                System.Net.HttpStatusCode.Unauthorized,
                                "Request must contain Authorization-Token header.");
                        // md5( method:Authorization-Key:Authorization-Token:Random-Hash:Secret )
                        intsign = MD5.Hash(
                            method + ":" + key + ":" + token + ":" + rnd + ":" + secret);
                        break;
                }

                // Validating Request-Sign header value.
                if (sign.ToLower() != intsign)
                    throw new WebException(
                        System.Net.HttpStatusCode.Unauthorized,
                        "Request-Sign header validation failed.");

                // Saving Random-Hash used for current
                // authorization and setting session state.
                this.provider.SetLastRandom(key, rnd);
                if (this.sessionKey != null)
                {
                    current.Session[this.sessionKey]
                        = this.provider.GetUser(key);
                }

                return true;
            }
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // We need to check if session is created
            // only for current random request and set
            // corelation state for session removal.
            HttpContext current = HttpContext.Current;
            if (current != null &&
                current.Session[this.sessionKey] == null)
            {
                return false;
            }
            return true;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // If we have correlation state set to
            // false from AfterReceiveRequest event,
            // we need to abandone current session.
            if ((bool)correlationState == false)
            {
                HttpContext.Current.Session.Abandon();
            }
        }
    }
}
