using keymanager_Dev.Clases;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Widgets
{
    public partial class MasterPassConfirm : Window
    {
        UserDao userDao;
        public MasterPassConfirm()
        {
            InitializeComponent();
            
            // Registra el controlador para el evento PasswordChanged del PasswordBox
            passTextBox.PasswordChanged += PassTextBox_PasswordChanged;

            Loaded += MasterPassConfirm_Loaded;
            Loaded += checkKeyTimer_Loaded;

        }

        private void checkKeyTimer_Loaded(object sender, RoutedEventArgs e)
        {
            if (KeyTimer.Instance.BooleanValue)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
        private void MasterPassConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            // Asegúrate de que el WaterMarkerLabel esté visible al inicio
            var waterMarkerLabel = passTextBox.Template.FindName("WaterMarkerLabel", passTextBox) as Label;
            if (waterMarkerLabel != null)
            {
                waterMarkerLabel.Visibility = Visibility.Visible;
            }
        }

        private void PassTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var waterMarkerLabel = passTextBox.Template.FindName("WaterMarkerLabel", passTextBox) as Label;

            if (waterMarkerLabel != null)
            {
                // Actualiza la visibilidad del WaterMarkerLabel según el contenido del PasswordBox
                waterMarkerLabel.Visibility = string.IsNullOrEmpty(passTextBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }


        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string confirmPassword = new System.Net.NetworkCredential(string.Empty, passTextBox.SecurePassword).Password;

            if (!string.IsNullOrEmpty(confirmPassword))
            {
                userDao = new UserDao();
                string data = userDao.getUserKeyByUsername(Environment.UserName);

                string masterKey = Encryptor.Decrypt(data, Encryptor.KEY);

                string userKey = Encryptor.GenerateHash(confirmPassword);
                if (userKey.Equals(masterKey))
                {
                    if (!KeyTimer.Instance.BooleanValue)
                    {
                        KeyTimer.Instance.BooleanValue = true;
                    }
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    NotificationWindow.ShowNotification("The Password introduced is not correct", NotificationType.Error, "Error");
                }

            }
            else
            {
                NotificationWindow.ShowNotification("you should complete the field", NotificationType.Error, "Warning");
            }


        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            string confirmPassword = new System.Net.NetworkCredential(string.Empty, passTextBox.SecurePassword).Password;

            if (!string.IsNullOrEmpty(confirmPassword))
            {
                userDao = new UserDao();
                string data = userDao.getUserKeyByUsername(Environment.UserName);

                string masterKey = Encryptor.Decrypt(data, Encryptor.KEY);

                string userKey = Encryptor.GenerateHash(confirmPassword);
               
                if (!userKey.Equals(masterKey))
                {
                    int id = userDao.getUserIdByUsername(Environment.UserName);
                    if (id != 0) 
                    {
                        CredentialDao credentialDao = new CredentialDao();

                        List<Credential> credentialList = credentialDao.getCredentialsByUserId(id);

                        if (credentialList != null && credentialList.Count > 0) 
                        {
                            if(credentialDao.changeMasterKey(userKey, masterKey, credentialList)) 
                            {
                                this.Close();
                                NotificationWindow.ShowNotification("Master Key succesfully changed", NotificationType.Success, "Success");
                            }
                            else
                            {

                            }
                            
                        }
                        else
                        {
                            NotificationWindow.ShowNotification("Not credentials found", NotificationType.Error, "Error");
                        }

                    }
                    else
                    {
                        NotificationWindow.ShowNotification("Not credentials found", NotificationType.Error, "Error");
                    }
                    
                }
                else
                {
                    NotificationWindow.ShowNotification("The Password introduced needs to be changed", NotificationType.Error, "Error");
                }

            }
            else
            {
                NotificationWindow.ShowNotification("you should complete the field", NotificationType.Error, "Warning");
            }


        }
    }
}
