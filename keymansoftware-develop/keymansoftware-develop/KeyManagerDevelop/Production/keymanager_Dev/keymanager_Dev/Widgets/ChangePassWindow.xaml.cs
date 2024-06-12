using keymanager_Dev.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Widgets
{
    /// <summary>
    /// Lógica de interacción para ChangePassWindow.xaml
    /// </summary>
    public partial class ChangePassWindow : Window
    {
        User user;
        UserDao userDao;
        int idCredencial;
        CredentialDao credentialDao = new CredentialDao();

        public ChangePassWindow(int idCredencial)
        {
            InitializeComponent();
            this.idCredencial = idCredencial;
            txtBoxPass.TextChanged += xpassBox_TextChanged;
            textBoxPassConfirm.TextChanged += xpassBox_TextChanged;
        }

        private void xpassBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var waterMarkerLabel = textBox.Template?.FindName("WaterMarkerLabel", textBox) as Label;

                if (waterMarkerLabel != null)
                {
                    // Actualiza la visibilidad del WaterMarkerLabel según el contenido del PasswordBox
                    waterMarkerLabel.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (allFieldsCompleted())
            {
                if (txtBoxPass.Text.Length >= 6)
                {
                    // Si los textos no son iguales
                    if (!txtBoxPass.Text.Contains(" "))
                    {
                        if (txtBoxPass.Text != textBoxPassConfirm.Text)
                        {

                            NotificationWindow.ShowNotification("Passwords do not match", NotificationType.Error, "Error");
                            return;
                        }



                        userDao = new UserDao();
                        //Recuperamos la MasterKey encryptada y la desencryptamos lo cual nos devoilverá el hash de la masterKey
                        string keyHash = Encryptor.Decrypt(userDao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);
                        string key = keyHash.Substring(0, Math.Min(keyHash.Length, 16));
                        //usamos el hash de la masterKey desencyptada para encyptar esta clave
                        string pass = Encryptor.Encrypt(txtBoxPass.Text, key);

                        int idUser = userDao.getUserIdByUsername(Environment.UserName);
                        if (idUser != -1)
                        {
                            DateTime actualDate = DateTime.Now;
                            DateTime datein3mounths = actualDate.AddMonths(3);

                            // This adjust the mounth in case it is higher than 12
                            while (datein3mounths.Month > 12)
                            {
                                datein3mounths = datein3mounths.AddYears(1);
                                datein3mounths = datein3mounths.AddMonths(-12);
                            }

                            int succes = credentialDao.changePasswordCredential(idCredencial, pass, datein3mounths);


                            if (succes != 0)
                            {
                                NotificationWindow.ShowNotification("Password changed", NotificationType.Success, "Succes");
                                this.DialogResult = true;
                                Close_Window(null, null);
                            }
                            else
                            {
                                NotificationWindow.ShowNotification("An error ocurred. Password didn't change.", NotificationType.Error, "Error");
                            }
                        }

                    }
                    else
                    {
                        NotificationWindow.ShowNotification("Password cannot conatin empty space", NotificationType.Error, "Warning");
                    }

                }
                else
                {
                    NotificationWindow.ShowNotification("Password length is too short", NotificationType.Error, "Warning");
                }
            }
            else
            {
                NotificationWindow.ShowNotification("All fields should be completed", NotificationType.Error, "Warning");
            }

        }

        private bool allFieldsCompleted()
        {
            return !string.IsNullOrEmpty(txtBoxPass.Text)
                && !string.IsNullOrEmpty(textBoxPassConfirm.Text);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close_Window(null, null);
        }

        private void Close_Window(object sender, EventArgs e)
        {
            this.Close();
        }

        private void genPassBtn_Click(object sender, RoutedEventArgs e)
        {
            PassManager passManager = new PassManager();

            // 👍All checked 
            string password = PassManager.passGenerator(12);
            // 👍All unchecked 
            if (!(bool)lowercaseCheckBox.IsChecked & !(bool)uppercaseCheckBox.IsChecked & !(bool)numbersCheckBox.IsChecked)
            {
                txtBoxPass.Text = "";
                textBoxPassConfirm.Text = "";
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

            if (!string.IsNullOrEmpty(txtBoxPass.Text) && !string.IsNullOrEmpty(textBoxPassConfirm.Text))
            {
                txtBoxPass.Text = "";
                textBoxPassConfirm.Text = "";
            }
            txtBoxPass.Text = password;
            textBoxPassConfirm.Text = password;



        }

        private void xpassBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
