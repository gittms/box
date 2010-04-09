using System;
using System.ServiceModel.Dispatcher;

namespace Definitif.ServiceModel.Authorization
{
    /// <summary>
    /// Represents authorization inspector interface.
    /// </summary>
    public interface IAuthorizationInspector : IDispatchMessageInspector
    { }
}