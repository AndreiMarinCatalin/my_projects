using keymanager_Dev.Clases;
using keymanager_Dev.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

namespace keymanager_Dev.Pages
{
    /// <summary>
    /// Lógica de interacción para CheckPasswordsWindow.xaml
    /// </summary>
    public partial class CheckPasswordsWindow : Window
    {
        CredentialDao credentialDao;
        UserDao userDao;
        public CheckPasswordsWindow()
        {
            InitializeComponent();
            credentialDao = new CredentialDao();
            userDao = new UserDao();
            upgradablePasswords();
            repeatedPasswords();
            weakPAsswords();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Checks how many passwords can be upgraded to be stronger and safer. It just
        /// show a message with the number of the upgradable passwords. 
        /// Upgradable passwords
        /// are those which doesn't have symbols, uppercase or lowercase letters or numbers, 
        /// all in one single passowrd.
        /// </summary>
        private void upgradablePasswords()
        {
            List<String> passwordsList = decryptPasswords();
            int counterTotal = 0;

            foreach (String pass in passwordsList)
            {
                    bool hasUpperCase = false;
                    bool hasLowerCase = false;
                    bool hasNumber = false;

                    foreach (char character in pass)
                    {
                        if (char.IsUpper(character))
                            hasUpperCase = true;
                        else if (char.IsLower(character))
                            hasLowerCase = true;
                        else if (char.IsNumber(character))
                            hasNumber = true;
                    }

                    if (!hasUpperCase || !hasLowerCase || !hasNumber)
                        counterTotal++;
                
            }
            label1.Content = counterTotal.ToString() + " Upgradable passwords.";
        }

        /// <summary>
        /// Counts all repeated passwords and shows the message
        /// </summary>
        private void repeatedPasswords()
        {
            List<String> passwordsList = decryptPasswords();
            int repeatedPass = 0;
            Dictionary<String, int> cont = new Dictionary<string, int>();

            foreach (string password in passwordsList)
            {
                if (cont.ContainsKey(password))
                {
                    cont[password]++;
                }
                else
                {
                    cont[password] = 1;
                }
            }

            foreach (var value in cont.Values)
            {
                if (value > 1) // Solo contar repeticiones
                {
                    repeatedPass += value;
                }
            }

            label2.Content = repeatedPass.ToString() + " Repeated passwords.";
        }

        /// <summary>
        /// If passwords doen't have 2 of the 4 types of characters (symbols, uppercase or lowercase letters or numbers)
        /// are considered weak passwords.
        /// </summary>
        private void weakPAsswords()
        {
            List<String> passwordList = decryptPasswords();
            int counterTotal = 0;

            foreach (string password in passwordList)
            {
                int hasUpperCase = 0;
                int hasLowerCase = 0;
                int hasNumber = 0;

                foreach (char character in password)
                {
                    if (char.IsUpper(character))
                    {
                        hasUpperCase = 1;
                    }
                    if (char.IsLower(character))
                    {
                        hasLowerCase = 1;
                    }
                    if (char.IsNumber(character))
                    {
                        hasNumber = 1;
                    }
                }

                int passwordScore = hasUpperCase + hasLowerCase + hasNumber;
                if (passwordScore < 2)
                {
                    counterTotal++;
                }
            }
            label3.Content = counterTotal.ToString() + " Weak passwords.";
        }


        // Gets all credentials find by userId
        private List<Credential> getListOfCredentials()
        {
            List<Credential> credentials = null;
            int idUser = userDao.getUserIdByUsername(Environment.UserName);
            if (idUser != -1)
            {
                credentials = credentialDao.getCredentialsByUserId(idUser);
            }
            return credentials;
        }

        private List<String> decryptPasswords()
        {
            List<String> passwordList = new List<String>();
            List<Credential> credentials = getListOfCredentials();
            if (credentials != null)
            {
                foreach (Credential c in credentials)
                {
                    string decryptKey = Encryptor.Decrypt(userDao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);

                    if (!string.IsNullOrWhiteSpace(decryptKey))
                    {
                        string key = decryptKey.Substring(0, Math.Min(decryptKey.Length, 16));
                        string pass = Encryptor.Decrypt(c.Password, key);
                        passwordList.Add(pass);
                    }
                }
            }
            
            return passwordList;
        }
    }
}
