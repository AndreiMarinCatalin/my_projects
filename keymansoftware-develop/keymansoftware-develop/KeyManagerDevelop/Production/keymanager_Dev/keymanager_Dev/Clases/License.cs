using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace keymanager_Dev.Clases
{
    public class License
    {
        public static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Crea la ruta al archivo de licencia
        public static string licenseFilePath = @"C:\Program Files (x86)\VisumMasterSoftware";
        private static byte[] Key = Encoding.UTF8.GetBytes("ClaveSecreta1234"); // La clave debe ser de 16 bytes

        public License()
        {

        }

        public static bool validateLicense(string filetext)
        {
            if (filetext.Equals(License.GenerateHash(License.GetMacAddress())))
            {
                return true;

            }
            return false;
        }

        public static bool checkLicense()
        {
            // Obtiene la ruta de la carpeta AppData
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string license_Path = Path.Combine(appDataFolderPath);
            if (!Directory.Exists(license_Path))
            {
                // El directorio no existe, puedes agregar lógica adicional aquí si deseas realizar alguna acción especial
                return false;
            }

            string[] files = Directory.GetFiles(license_Path, "License.lic");

            if (files.Length > 0)
            {
                // Se encontró el archivo License.lic
                return true;
            }

            return false;
        }


        /// <summary>
        /// Gets the MAC address of the setup
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            string macAddress = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !nic.Description.ToLowerInvariant().Contains("virtual") &&
                    !nic.Description.ToLowerInvariant().Contains("vmware") &&
                    !nic.Description.ToLowerInvariant().Contains("hamachi") &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddress;
        }

        public static string Encrypt(string input)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
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

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts the message
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decrypt(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;

                using (MemoryStream ms = new MemoryStream(inputBytes))
                {
                    byte[] IV = new byte[16];
                    ms.Read(IV, 0, IV.Length);
                    aes.IV = IV;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

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
