using System;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents custom Token-based authorization provider.
    /// </summary>
    public interface ITokenAuthorizationProvider
    {
        /// <summary>
        /// Gets token details object by API key.
        /// </summary>
        /// <returns>TokenDetails object if frob is valid, overwise null.</returns>
        TokenDetails GetDetailsByKey(string key);
        /// <summary>
        /// Gets token details object by authorization frob.
        /// </summary>
        /// <returns>TokenDetails object if frob is valid, overwise null.</returns>
        TokenDetails GetDetailsByFrob(string frob);
        /// <summary>
        /// Gets token details object by authorization token.
        /// </summary>
        /// <returns>TokenDetails object if frob is valid, overwise null.</returns>
        TokenDetails GetDetailsByToken(string token);

        /// <summary>
        /// Sets last random value by token details object.
        /// </summary>
        void SetLastRandom(TokenDetails details, string random);
        /// <summary>
        /// Sets new frob value by token details object.
        /// </summary>
        void SetFrob(TokenDetails details, string frob);
    }
}
