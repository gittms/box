using System;
using Definitif.Security.Cryptography;

namespace Definitif.ServiceModel.Authorization.Token
{
    /// <summary>
    /// Represents static token generator.
    /// </summary>
    public class TokenGenerator
    {
        private static StringGenerator generator = new StringGenerator();
        private static string keyPattern = "[a-z0-9]{32}",
                              secretPattern = "[a-z0-9]{16}",
                              frobPattern = "[0-9]{4}-[a-z0-9]{8}@ddMMYY",
                              tokenPattern = "[a-z0-9]{12}@ddMMYY";

        /// <summary>
        /// Gets new singleton or sets existing global randomizer.
        /// </summary>
        public static Random Random
        {
            get { return generator.Random; }
            set { generator.Random = value; }
        }

        /// <summary>
        /// Gets or sets static key pattern.
        /// </summary>
        public static string KeyPattern
        {
            get { return keyPattern; }
            set { keyPattern = value; }
        }
        /// <summary>
        /// Gets or sets static token generation pattern.
        /// </summary>
        public static string TokenPattern
        {
            get { return tokenPattern; }
            set { tokenPattern = value; }
        }
        /// <summary>
        /// Gets or sets static secret generation pattern.
        /// </summary>
        public static string SecretPattern
        {
            get { return secretPattern; }
            set { secretPattern = value; }
        }
        /// <summary>
        /// Gets or sets static frob generation pattern.
        /// </summary>
        public static string FrobPattern
        {
            get { return frobPattern; }
            set { frobPattern = value; }
        }

        /// <summary>
        /// Generates new Token using pattern. 
        /// </summary>
        public static string NewToken()
        {
            return generator.Generate(tokenPattern)
                .Replace("ddMMYY", DateTime.Now.ToString("ddMMyy"));
        }
        /// <summary>
        /// Generates new Frob using pattern. 
        /// </summary>
        public static string NewFrob()
        {
            return generator.Generate(frobPattern)
                .Replace("ddMMYY", DateTime.Now.ToString("ddMMyy"));
        }
        /// <summary>
        /// Generates new Key using pattern. 
        /// </summary>
        public static string NewKey()
        {
            return generator.Generate(keyPattern)
                .Replace("ddMMYY", DateTime.Now.ToString("ddMMyy"));
        }
        /// <summary>
        /// Generates new Secret using pattern. 
        /// </summary>
        public static string NewSecret()
        {
            return generator.Generate(secretPattern)
                .Replace("ddMMYY", DateTime.Now.ToString("ddMMyy"));
        }
    }
}
