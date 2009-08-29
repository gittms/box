using System;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents custom Token-based authorization provider.
    /// </summary>
    public interface ITokenAuthorizationProvider
    {
        /// <summary>
        /// Gets user object by authorization key string.
        /// </summary>
        object GetUser(string key);
        /// <summary>
        /// Gets secret string by authorization key string.
        /// </summary>
        string GetSecret(string key);
        /// <summary>
        /// Gets last random hash used by authorization key string.
        /// </summary>
        string GetLastRandom(string key);
        /// <summary>
        /// Sets last random hash for authorization key string.
        /// </summary>
        void SetLastRandom(string key, string random);
    }
}
