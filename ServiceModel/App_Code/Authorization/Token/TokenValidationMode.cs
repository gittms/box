using System;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents token validation mode.
    /// </summary>
    public enum TokenValidationMode
    {
        /// <summary>
        /// Skips token validation.
        /// <remarks>
        /// It is possible to remove attribute from operation
        /// for same behavior.
        /// </remarks>
        /// </summary>
        None,
        /// <summary>
        /// Secret mode only checks validity of random signing.
        /// md5( method:Authorization-Key:Random-Hash:Secret )
        /// </summary>
        Secret,
        /// <summary>
        /// Frob mode checks for valid frob and sign.
        /// md5( method:Authorization-Key:Authorization-Frob:Random-Hash:Secret )
        /// </summary>
        Frob,
        /// <summary>
        /// Token mode checks for valid token and sign.
        /// md5( method:Authorization-Key:Authorization-Token:Random-Hash:Secret )
        /// </summary>
        Token
    }
}
