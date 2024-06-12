using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace keymanager_Dev.Clases
{
    
    public class Encryptor
    {
        private static string key = "ClaveSecreta1234";

        public static string KEY
        {
            get { return key; }
        }

        private const string IV = "1234567890123456";

        public static string Encrypt(string input, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey) || encryptionKey.Length != 16)
            {
                throw new ArgumentException("The encryption key must be 16 characters long.");
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(input);
                        }
                    }

                    // Return IV concatenated with encrypted message
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        /// <summary>
        /// Decrypt message by his key
        /// </summary>
        /// <param name="input"></param>
        /// <param name="decryptionKey"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Decrypt(string input, string decryptionKey)
        {
            if (string.IsNullOrEmpty(decryptionKey) || decryptionKey.Length != 16)
            {
                throw new ArgumentException("The decryption key must be 16 characters long.");
            }

            try
            {
                byte[] inputBytes = Convert.FromBase64String(input);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(decryptionKey);

                    // Extract IV from input
                    byte[] iv = new byte[16];
                    Array.Copy(inputBytes, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(inputBytes, iv.Length, inputBytes.Length - iv.Length))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                return string.Empty; // Devuelve un texto vacío en caso de excepción de formato
            }
            catch (Exception ex)
            {
                // Manejo de otras excepciones
                Console.WriteLine("Error during decrypt: " + ex.Message);
                return string.Empty; // Devuelve un texto vacío en caso de otras excepciones
            }
        }

        public static string GenerateHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

   

}
