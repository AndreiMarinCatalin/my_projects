using keymanager_Dev.Clases;
using keymanager_Dev.Pages;
using keymanager_Dev.ViewModel;
using keymanager_Dev.Widgets;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev
{
    /// <summary>
    /// Lógica de interacción para HomeView.xaml
    /// </summary>
    public partial class HomeView : System.Windows.Controls.UserControl
    {

        Color eye_open_background = (Color)Application.Current.Resources["eye_open_background"];
        Color eye_closed_background = (Color)Application.Current.Resources["eye_closed_background"];
        AddCardWindow addCardWindow = new AddCardWindow();
        List<Credential> credentials = null;
        CredentialDao credentialDao = null;

        UserDao dao;
        bool eyeopen = false;

        public HomeView()
        {
            InitializeComponent();
            reloadPage();
            InitializePasswordCards();

        }

        private void InitializePasswordCards()
        {
            foreach (var item in stackPanel.Children)
            {
                if (item is PasswordCard passwordCard)
                {
                    passwordCard.IsCheckedChanged += PasswordCard_IsCheckedChanged;
                    passwordCard.SomeEvent += PassCardSomeEvent_Handler;
                    passwordCard.EditEvent += PassCardEdit_IsCredentialEdited;

                }
            }

            // Testing DateTime
            //DateTime fechaManual = new DateTime(2023, 8, 26, 11, 30, 0);

            //PasswordCard card = new PasswordCard(123, "1", "1", "1", fechaManual);
            //stackPanel.Children.Add(card);
        }

        private void PassCardEdit_IsCredentialEdited(object sender, EventArgs e)
        {
            reloadPage();
        }

        private void PassCardSomeEvent_Handler(object sender, EventArgs e)
        {
            reloadPage();
        }
        private void PasswordCard_IsPasswordChanged(object sender, EventArgs e)
        {
            reloadPage();
        }

        private void PasswordCard_IsCheckedChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }
        private void UpdateButtonState()
        {
            bool anyCardSelected = checkAnyCardSelected();
            btRemove.IsEnabled = anyCardSelected;
        }
        private void passCardCreator(List<Credential> credentials)
        {
            stackPanel.Children.Clear();

            foreach (Credential credential in credentials)
            {
                PasswordCard card = new PasswordCard(credential.Id, credential.Name, credential.Username, credential.Password, credential.expiration_data);
                stackPanel.Children.Add(card);
            }
        }

        public List<Credential>? getCredentials()
        {
            List<Credential> list = null;

            UserDao userDao = new UserDao();

            int idUser = userDao.getUserIdByUsername(Environment.UserName);
            if (idUser != -1)
            {
                credentialDao = new CredentialDao();
                list = credentialDao.getCredentialsByUserId(idUser);

            }
            return list;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            // Aqui tiene que aparecer la ventana AddCardWindow
            AddCardWindow addCard = new AddCardWindow();
            addCard.WindowStyle = WindowStyle.None;
            addCard.AllowsTransparency = true;
            addCard.Background = Brushes.Transparent;
            addCard.Topmost = true;
            bool? result = addCard.ShowDialog();

            if (result == true)
            {

                reloadPage();
                btRemove.IsEnabled = false;
            }


        }

        private void sAllbt_Click(object sender, RoutedEventArgs e)
        {
            int counterFalse = 0;

            foreach (var item in stackPanel.Children)
            {
                if (item is PasswordCard passwordCard)
                {
                    if (!passwordCard.IsChecked)
                    {
                        counterFalse++;
                    }
                }
            }

            if (counterFalse > 0)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is PasswordCard passwordCard)
                    {
                        if (!passwordCard.IsChecked)
                        {
                            passwordCard.Check();
                        }
                    }
                }
            }
            else
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is PasswordCard passwordCard)
                    {
                        if (passwordCard.IsChecked)
                        {
                            passwordCard.Check();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// removes the selected credential from database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Removebt_Click(object sender, RoutedEventArgs e)
        {
            int changes = 0;

            if (checkAnyCardSelected())
            {
                MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
                bool? dialogResult = masterPassConfirm.ShowDialog();

                if (dialogResult == true)
                {
                    foreach (var item in stackPanel.Children)
                    {
                        if (item is PasswordCard passwordCard)
                        {

                            if (passwordCard.isChecked())
                            {
                                int success = credentialDao.deleteCredential(passwordCard.Id);

                                if (success != -1)
                                {
                                    changes++;
                                }
                                else
                                {
                                    NotificationWindow.ShowNotification("Error deleting credential", NotificationType.Error, "Error");
                                }
                            }
                        }
                    }
                }

            }


            if (changes != 0)
            {
                NotificationWindow.ShowNotification("Credential removed", NotificationType.Success, "Success");
                reloadPage();
                btRemove.IsEnabled = false;
            }

        }

        /// <summary>
        /// Checks if any card is selected
        /// </summary>
        /// <returns></returns>
        private bool checkAnyCardSelected()
        {
            foreach (var item in stackPanel.Children)
            {
                if (item is PasswordCard passwordCard)
                {
                    if (passwordCard.isChecked())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void reloadPage()
        {
            
            credentials = getCredentials();

            if (credentials != null)
            {
                passCardCreator(credentials);

            }

            InitializePasswordCards();
        }


        private void btShowPass_Click(object sender, RoutedEventArgs e)
        {
            if (checkAnyCardSelected() && !eyeopen)
            {
                MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
                bool? dialogResult = masterPassConfirm.ShowDialog();

                if (dialogResult == true)
                {
                    dao = new UserDao();
                    foreach (var item in stackPanel.Children)
                    {
                        if (item is PasswordCard passwordCard)
                        {
                            if (passwordCard.isChecked())
                            {
                                showHidePass(passwordCard.isChecked(), passwordCard);
                            }
                            
                        }

                    }
                }
            }
            else
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is PasswordCard passwordCard)
                    {
                        showHidePass(false, passwordCard);
                    }

                }
            }
        }

        private void showHidePass(bool showDecrypted, PasswordCard passwordCard)
        {
            if (showDecrypted)
            {
                string decryptKey = Encryptor.Decrypt(dao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);

                if (!string.IsNullOrWhiteSpace(decryptKey))
                {
                    string key = decryptKey.Substring(0, Math.Min(decryptKey.Length, 16));
                    string pass = Encryptor.Decrypt(passwordCard.passCard.Password, key);

                    
                    SetEyeButtonImageSource("/Images/eye_open.png");
                    SolidColorBrush brush = new SolidColorBrush(eye_open_background);
                    eyeBtn.Background = brush;
                    passwordCard.texCard.Text = pass;
                    passwordCard.ChangeCheck();
                    passwordCard.passCard.Visibility = Visibility.Collapsed;
                    passwordCard.texCard.Visibility = Visibility.Visible;
                }
            }
            else
            {
                SetEyeButtonImageSource("/Images/close-eye.png");
                SolidColorBrush brush = new SolidColorBrush(eye_closed_background);
                eyeBtn.Background = brush;
                passwordCard.texCard.Text = string.Empty;  // Es preferible usar string.Empty en lugar de un espacio en blanco
                passwordCard.passCard.Visibility = Visibility.Visible;
                passwordCard.texCard.Visibility = Visibility.Collapsed;
            }
        }

        private void SetEyeButtonImageSource(string imagePath)
        {
            var imageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            eyeButton.Source = imageSource;
        }

        private void checkPass_Click(object sender, RoutedEventArgs e)
        {
            // Aqui tiene que aparecer la ventana AddCardWindow
            CheckPasswordsWindow checkPasswordsWindow= new CheckPasswordsWindow();
            checkPasswordsWindow.WindowStyle = WindowStyle.None;
            //checkPasswordsWindow.AllowsTransparency = true;
            //checkPasswordsWindow.Background = Brushes.Transparent;
            checkPasswordsWindow.Topmost = true;
            bool? result = checkPasswordsWindow.ShowDialog();

            if (result == true)
            {
                reloadPage();
                btRemove.IsEnabled = false;
            }
        }
    }
}
