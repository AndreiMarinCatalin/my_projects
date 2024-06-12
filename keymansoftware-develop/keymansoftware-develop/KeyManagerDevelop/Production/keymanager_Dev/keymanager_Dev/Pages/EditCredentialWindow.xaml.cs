using keymanager_Dev.Clases;
using keymanager_Dev.Widgets;
using System;
using System.Collections.Generic;
using System.Windows;
using static keymanager_Dev.Clases.PassManager;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Pages
{
    /// <summary>
    /// Lógica de interacción para EditCredentialWindow.xaml
    /// </summary>
    public partial class EditCredentialWindow : Window
    {
        int Id;
        string title;
        string username;
        string password;
        DateTime expirationDate;
        CredentialDao credentialDao = new CredentialDao();
        UserDao userDao = new UserDao();
        public EditCredentialWindow(int Id, string title, string username, string password)
        {
            InitializeComponent();
            this.Id = Id;
            this.title = title;
            this.username = username;
            this.password = password;
            titleTxtBox.Text = this.title;
            usernameTxtBox.Text = this.username;

            string decryptKey = Encryptor.Decrypt(userDao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);
            if (!string.IsNullOrWhiteSpace(decryptKey))
            {
                string key = decryptKey.Substring(0, Math.Min(decryptKey.Length, 16));
                string pass = Encryptor.Decrypt(password, key);
                passwordBox.Text = pass;
                this.password = pass;
            }
        }


        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (allFieldsCompleted())
            {
                if (passwordBox.Text.Length >= 6)
                {
                    userDao = new UserDao();
                    string keyHash = Encryptor.Decrypt(userDao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);
                    string key = keyHash.Substring(0, Math.Min(keyHash.Length, 16));
                    string pass = Encryptor.Encrypt(passwordBox.Text, key);
                    int idUser = userDao.getUserIdByUsername(Environment.UserName);
                    if (idUser != -1)
                    {
                        Credential credential = new Credential(
                            Id,
                            titleTxtBox.Text,
                            usernameTxtBox.Text,
                            pass,
                            expirationDate
                            );
                        int success = credentialDao.changeCredentialById(credential);
                        if (success != 0)
                        {
                            NotificationWindow.ShowNotification("Credential modified successfully!", NotificationType.Success, "Succes");
                            this.DialogResult = true;
                            Close_Window(null, null);

                        }
                        else
                        {
                            NotificationWindow.ShowNotification("Error editing credential. Please try again.", NotificationType.Error, "Error");
                        }
                    }
                }
                else
                {
                    NotificationWindow.ShowNotification("Password length is too short", NotificationType.Error, "Error");
                }
            }
            else
            {
                NotificationWindow.ShowNotification("All fields should be completed", NotificationType.Error, "Error");

            }
        }

        private void genPassBtn_Click(object sender, RoutedEventArgs e)
        {
            PassManager passManager = new PassManager();

            // 👍All checked 
            password = PassManager.passGenerator(12);
            // 👍All unchecked 
            if (!(bool)lowercaseCheckBox.IsChecked & !(bool)uppercaseCheckBox.IsChecked & !(bool)numbersCheckBox.IsChecked)
            {
                passwordBox.Text = "";
                return;
            }

            // 👍Uncheck lowercase
            if (!(bool)lowercaseCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.Lowercase);
            // 👍Uncheck uppercase
            if (!(bool)uppercaseCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.Uppercase);
            // 👍Uncheck numbers
            if (!(bool)numbersCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.Numbers);
            // 👍Uncheck lowercase and uppercase
            if (!(bool)lowercaseCheckBox.IsChecked & !(bool)uppercaseCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.onlyNumbers);
            // 👍Uncheck lowercase and numbers
            if (!(bool)lowercaseCheckBox.IsChecked & !(bool)numbersCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.onlyUppercase);
            // 👍Uncheck uppercase and numbers
            if (!(bool)uppercaseCheckBox.IsChecked & !(bool)numbersCheckBox.IsChecked) password = PassManager.passGenerator(12, PassManager.CharSets.onlyLowercase);


            if (!string.IsNullOrEmpty(passwordBox.Text))
            {
                passwordBox.Text = "";
            }
            passwordBox.Text = password;
        }

        private void expirationTimeOption_Checked(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.RadioButton selectedRadioButton = sender as System.Windows.Controls.RadioButton;

            if (selectedRadioButton != null && selectedRadioButton.IsChecked == true)
            {
                Dictionary<string, Action> actions = new Dictionary<string, Action>
                {
                    { "firstRadioButton", () => setExpirationDate(1) },
                    { "secondRadioButton", () => setExpirationDate(3)  },
                    { "thirdRadioButton", () => setExpirationDate(12)  }
                };

                if (actions.ContainsKey(selectedRadioButton.Name))
                {
                    actions[selectedRadioButton.Name]();
                }
            }
        }

        private void setExpirationDate(int mounths)
        {
            DateTime actualDate = DateTime.Now;
            DateTime dateinXmounths = actualDate.AddMonths(mounths);

            // This adjust the mounth in case it is higher than 12
            while (dateinXmounths.Month > 12)
            {
                dateinXmounths = dateinXmounths.AddYears(1);
                dateinXmounths = dateinXmounths.AddMonths(-12);
            }
            expirationDate = dateinXmounths;

        }



        private bool allFieldsCompleted()
        {
            return !string.IsNullOrEmpty(titleTxtBox.Text)
                && !string.IsNullOrEmpty(usernameTxtBox.Text)
                && !string.IsNullOrEmpty(passwordBox.Text);
        }

        private void Close_Window(object sender, EventArgs e)
        {
            this.Close();
        }


        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close_Window(null, null);
        }
    }
}
