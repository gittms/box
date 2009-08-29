using System;
using System.Web;
using System.ServiceModel.Dispatcher;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represets Token-base authorization operation parameters inspector.
    /// </summary>
    public class TokenAuthorizationParameterInspector : IParameterInspector
    {
        private TokenAuthorizationInspector parent;
        private TokenValidationMode mode;

        /// <summary>
        /// Creates new parameter inspector for Token-based authorization.
        /// </summary>
        /// <param name="parent">Token-based authorization service message inspector.</param>
        /// <param name="mode">Token-based authorizatin validation mode.</param>
        public TokenAuthorizationParameterInspector(TokenAuthorizationInspector parent, TokenValidationMode mode)
        {
            this.parent = parent;
            this.mode = mode;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            if (!parent.ValidateContext(HttpContext.Current, this.mode))
            {
                throw new WebException(System.Net.HttpStatusCode.Unauthorized, "");
            }
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        { }
    }
}
