using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents Token-based authorization behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TokenAuthorization : Attribute, IServiceBehavior
    {
        private ITokenAuthorizationProvider provider;
        private string sessionKey;

        /// <summary>
        /// Sets Token-based authorization behavior for ServiceContract.
        /// </summary>
        /// <param name="provider">Token provider type that implements ITokenAuthorizationProvider interface.</param>
        /// <param name="sessionKey">Session key used to store user information.</param>
        public TokenAuthorization(Type provider, string sessionKey)
        {
            this.provider = (ITokenAuthorizationProvider)Activator.CreateInstance(provider);
            this.sessionKey = sessionKey;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                {
                    epDisp.DispatchRuntime.MessageInspectors.Add(
                        new TokenAuthorizationInspector(
                            this.provider, this.sessionKey));
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }
    }
}
