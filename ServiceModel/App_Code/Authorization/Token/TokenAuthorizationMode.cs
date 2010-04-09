using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Sets Token-based authorization mode for OperationContract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TokenAuthorizationMode : Attribute, IOperationBehavior
    {
        private TokenValidationMode mode = TokenValidationMode.Token;

        /// <summary>
        /// Sets Token validation mode.
        /// </summary>
        public TokenValidationMode Mode
        {
            set { this.mode = value; }
        }

        /// <summary>
        /// Creates new Token-based authorization requirement using
        /// default Token validation mode.
        /// </summary>
        public TokenAuthorizationMode() { }
        /// <summary>
        /// Creates new Token-based authorization requirement using
        /// given Token validation mode.
        /// <param name="mode">Token-based authorizatin validation mode.</param>
        /// </summary>
        public TokenAuthorizationMode(TokenValidationMode mode)
        {
            this.mode = mode;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        { }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            // Searching for parent authorization inspector and
            // associating it with new operation parameter inspector.
            foreach (IDispatchMessageInspector inspector in dispatchOperation.Parent.MessageInspectors)
            {
                if (inspector is TokenAuthorizationInspector)
                {
                    dispatchOperation.ParameterInspectors.Add(
                        new TokenAuthorizationParameterInspector(
                            inspector as TokenAuthorizationInspector, this.mode));
                    break;
                }
            }
        }

        public void Validate(OperationDescription operationDescription)
        { }
    }
}
