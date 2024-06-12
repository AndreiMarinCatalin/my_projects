using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace keymanager_Dev.Clases
{
    public class PassManager
    {
        private static int DIGITS = 8;
        private const string LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
        private const string UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NUMBERS = "0123456789";
        //private const string SPECIALS = "@#$%^&*_-+=[]{}|:,.?/<>()~";

        /// <summary>
        /// Generates passwords by given number of digits
        /// </summary>
        /// <returns></returns>
        [Flags]
        public enum CharSets
        {
            None = 0,
            Lowercase = 1,
            Uppercase = 2,
            Numbers = 4,
            onlyNumbers = 5,
            onlyUppercase = 6,
            onlyLowercase = 7,
            //Specials = 8,
            All = Lowercase | Uppercase | Numbers /*| Specials*/
        }

        /// <summary>
        /// Generates passwords by given number of digits and specified character sets
        /// </summary>
        /// <returns></returns>
        public static string passGenerator(int num, CharSets excludeSet = CharSets.None)
        {
            string allChars = "";

            if (excludeSet != CharSets.Lowercase)
                allChars += LOWERCASE;

            if (excludeSet != CharSets.Uppercase)
                allChars += UPPERCASE;

            if (excludeSet != CharSets.Numbers)
                allChars += NUMBERS;

            if (excludeSet == CharSets.onlyNumbers)
            {
                allChars = "";
                allChars += NUMBERS;
            }

            if (excludeSet == CharSets.onlyUppercase)
            {
                allChars = "";
                allChars += UPPERCASE;
            }

            if (excludeSet == CharSets.onlyLowercase)
            {
                allChars = "";
                allChars += LOWERCASE;
            }
            //if (excludeSet != CharSets.Specials)
            //    allChars += SPECIALS;

            if (num <= 0)
            {
                return "";
            }

            if (string.IsNullOrEmpty(allChars))
            {
                return "";
            }

            char[] characters = allChars.ToCharArray();
            StringBuilder password = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[num];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < num; i++)
                {
                    char newChar = characters[randomBytes[i] % characters.Length];
                    password.Append(newChar);
                }
            }

            return password.ToString();
        }

        


        //public int changeMasterKey(string newKey, string currentPass)
        //{
        //    int result = 0;
        //    string newPass = License.GenerateHash(newKey);



        //    if (string.IsNullOrEmpty(newPass))
        //    {
        //        UserDao dao = new UserDao();

        //        User user = dao.getUserByUsername(Environment.UserName);


        //        if (user != null)
        //        {
        //            CredentialDao credentialDao = new CredentialDao();
        //            List<Credential> credentials = null;
        //            if (credentialDao.getCredentialsByUserId(user.Id) != null)
        //            {

        //            }
        //            else
        //            {
        //                //NO passwords saved to this user
        //            }

        //        }
        //        else
        //        {
        //            //No user found
        //        }
        //    }
        //    else
        //    {
        //        //Error hasheando la contraseña
        //    }


        //    return result;
        //}
    }



    //COMO USARLO
    //PassManager manager = new PassManager();

    //string passwordAll = manager.passGenerator(12); // Use all char sets by default.

    //string passwordJustNumbers = manager.passGenerator(12, PassManager.CharSets.Numbers);

    //string passwordNumbersAndSpecials = manager.passGenerator(12, PassManager.CharSets.Numbers | PassManager.CharSets.Specials);

    //string passwordUpperAndLower = manager.passGenerator(12, PassManager.CharSets.Uppercase | PassManager.CharSets.Lowercase);



}
