using System;
using System.Text;
using System.Security.Cryptography;

namespace Definitif.Security.Cryptography
{
    /// <summary>
    /// Represents static MD5 cryptography algorithm.
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// Generates string MD5 hash.
        /// </summary>
        /// <param name="String">String to generate hash for.</param>
        /// <returns>MD5 hash string.</returns>
        public static string Hash(string String)
        {
            string hash = "";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] password = Encoding.ASCII.GetBytes(String);
            foreach (byte b in md5.ComputeHash(password))
            {
                hash += b.ToString("x2").ToLower();
            }
            return hash;
        }
    }
}
