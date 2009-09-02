using System;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents 
    /// </summary>
    public class TokenDetails
    {
        /// <summary>
        /// Gets or sets API key string representation.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets authorization frob string.
        /// </summary>
        public string Frob { get; set; }
        /// <summary>
        /// Gets or sets athorization token string.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Gets or sets last Random value used.
        /// </summary>
        public string LastRandom { get; set; }

        /// <summary>
        /// Gets or sets user representaion object.
        /// </summary>
        public object User { get; set; }
    }
}
