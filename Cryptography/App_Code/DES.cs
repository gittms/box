using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Definitif.Security.Cryptography
{
    /// <summary>
    /// Represets static DES cryptography algorithm.
    /// </summary>
    public static class DES
    {
        /// <summary>
        /// Encrypts string using DES algorithm.
        /// </summary>
        /// <param name="String">String to encrypt.</param>
        /// <param name="Key">Key to use for encryption.</param>
        /// <returns>Encrypted string.</returns>
        public static string Encrypt(string String, string Key)
        {
            if (Key == null) return String;

            if (String.IsNullOrEmpty(String))
            {
                throw new ArgumentNullException
                       ("Null string can not be encrypted.");
            }
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(Key);

            DESCryptoServiceProvider cryptoProvider
                = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(
                memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(String);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();

            return Convert.ToBase64String(
                memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Decrypts string using DES algorithm.
        /// </summary>
        /// <param name="String">Encrypted string.</param>
        /// <param name="Key">Key to use for decryption.</param>
        /// <returns>Descrypted string.</returns>
        public static string Decrypt(string String, string Key)
        {
            if (Key == null) return String;

            if (String.IsNullOrEmpty(String))
            {
                throw new ArgumentNullException
                       ("Null string can not be decrypted.");
            }
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(Key);

            DESCryptoServiceProvider cryptoProvider
                = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(
                Convert.FromBase64String(String));
            CryptoStream cryptoStream = new CryptoStream(
                memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
    }
}
