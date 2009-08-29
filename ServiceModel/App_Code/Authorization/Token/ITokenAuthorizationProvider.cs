using System;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents custom Token-based authorization provider.
    /// </summary>
    public interface ITokenAuthorizationProvider
    {
        /// <summary>
        /// Gets user object by frob string.
        /// </summary>
        object GetUserByFrob(string frob);
        /// <summary>
        /// Gets user object by authorization token string.
        /// </summary>
        object GetUserByToken(string token);
        /// <summary>
        /// Gets secret string by authorization key string.
        /// </summary>
        string GetSecret(string key);
        /// <summary>
        /// Gets authorization key by frob string.
        /// </summary>
        string GetKeyByFrob(string frob);
        /// <summary>
        /// Gets authorization key by authorization token string.
        /// </summary>
        string GetKeyByToken(string token);
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
