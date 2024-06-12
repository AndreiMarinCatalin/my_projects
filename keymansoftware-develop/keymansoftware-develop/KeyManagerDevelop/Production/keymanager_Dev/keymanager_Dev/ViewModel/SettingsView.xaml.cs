using keymanager_Dev.Clases;
using keymanager_Dev.Widgets;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev
{
    public partial class SettingsView : UserControl
    {
        UserDao userDao = new UserDao();
        HistoryDao historyDao = new HistoryDao();
        CredentialDao credentialDao = new CredentialDao();
        editBtnState state;
        sendCodeBtnState codeBtnState;
        string username;
        string emailString1;
        string emailString2;
        string code;
        string email;
        //string themeToggleBtn_background = ((Color)Application.Current.Resources["themeToggleBtn_background"]).ToString();
        string themeToggleBtn_background = "#22274E";


        public enum editBtnState
        {
            Edit, Cancel, Save
        }

        public enum sendCodeBtnState
        {
            Send, Confirm
        }
        public SettingsView()
        {
            InitializeComponent();
            username = GetWindowsUserName();
            email = userDao.getUserMailByUsername(username);
            //setThemeToggleBtnContent(themeToggleBtn_background, checkThemeToggleBtnBackgroundColor());
            state = editBtnState.Edit;
            emailTxtBox.Text = email;
            codeBox.PasswordChanged += CodeBox_PasswordChanged;
            emailString1 = emailTxtBox.Text;
            emailString2 = emailTxtBox.Text;
            //DataContext = this; // Establece el contexto de datos de la vista a esta instancia

        }

        private void CodeBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var waterMarkerLabel = passwordBox.Template.FindName("WaterMarkerLabel", passwordBox) as Label;

                if (waterMarkerLabel != null)
                {
                    // Actualiza la visibilidad del WaterMarkerLabel según el contenido del PasswordBox
                    waterMarkerLabel.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
            bool? dialogResult = masterPassConfirm.ShowDialog();

            if (dialogResult == true)
            {
                if (state == editBtnState.Edit)
                {
                    if (emailString1.Equals(emailString2))
                    {
                        emailTxtBox.IsEnabled = true;
                        state = editBtnState.Cancel;
                        editBtn.Content = "Cancel";
                        editBtn.BorderBrush = Brushes.Red;
                    }
                    else
                    {
                        state = editBtnState.Save;
                        editBtn.Content = "Save";
                    }
                }
                else if (state == editBtnState.Cancel | state == editBtnState.Save)
                {
                    if (state == editBtnState.Save)
                    {
                        userDao.changeMailByUser(GetWindowsUserName(), emailTxtBox.Text);
                        emailString1 = emailTxtBox.Text;
                        emailString2 = emailTxtBox.Text;
                    }

                    emailTxtBox.IsEnabled = false;
                    state = editBtnState.Edit;
                    editBtn.BorderBrush =Brushes.White;
                    editBtn.Content = "Edit";
                }
            }
        }

        private void emailTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            emailString2 = emailTxtBox.Text;
            if (emailString1 != null)
            {
                if (emailString1.Equals(emailString2))
                {
                    emailTxtBox.IsEnabled = true;
                    state = editBtnState.Cancel;
                    editBtn.Content = "Cancel";
                    editBtn.BorderBrush = Brushes.Red;
                }
                else
                {
                    state = editBtnState.Save;
                    editBtn.Content = "Save";
                    editBtn.BorderBrush = Brushes.Green;
                }
            }
        }


        /// <summary>
        /// Gets the windows Username
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsUserName()
        {
            return Environment.UserName;
        }

        private void clearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
            bool? dialogRes = masterPassConfirm.ShowDialog();
            if (dialogRes == true)
            {
                ConfirmOrCancelWindow confirmOrCancelWindow = new ConfirmOrCancelWindow();
                bool? x = confirmOrCancelWindow.ShowDialog();
                if (x == true)
                {
                    int id_user = userDao.getUserIdByUsername(username);
                    int success = historyDao.deleteHistory(id_user);
                    if (success > 0)
                    {
                        NotificationWindow.ShowNotification("History deleted!", NotificationType.Success, "Success");
                    }
                    else
                    {
                        NotificationWindow.ShowNotification("The history is already empty.", NotificationType.Error, "Error");
                    }
                }
            }
        }



        private void clearPasswordsBtn_Click(object sender, RoutedEventArgs e)
        {
            MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
            bool? dialogRes = masterPassConfirm.ShowDialog();
            if (dialogRes == true)
            {
                ConfirmOrCancelWindow confirmOrCancelWindow = new ConfirmOrCancelWindow();
                if (confirmOrCancelWindow.ShowDialog() == true)
                {
                    int id_user = userDao.getUserIdByUsername(username);
                    int success = credentialDao.removeAllCredentialByUserId(id_user);
                    if (success > 0)
                    {
                        NotificationWindow.ShowNotification("All passwords were deleted!", NotificationType.Success, "Success");
                    }
                    else
                    {
                        NotificationWindow.ShowNotification("There isn't any password to delete.", NotificationType.Error, "Error");
                    }
                }
            }
        }

        private void sendCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (codeBtnState == sendCodeBtnState.Send)
            {
                sendCodeBtn.BorderBrush = Brushes.Green;

                EmailSender.GenerateCode(6);

                if (!string.IsNullOrEmpty(EmailSender.Code) && !string.IsNullOrEmpty(email))
                {
                    EmailSender.SendEmailAsync(EmailSender.Code, email);

                    codeBtnState = sendCodeBtnState.Confirm;
                    sendCodeBtn.Content = "Confirm code";
                    codeBox.IsEnabled = true;

                }
                
            }
            else if (codeBtnState == sendCodeBtnState.Confirm)
            {
                // if the code is correct..
                if (codeBox.Password == EmailSender.Code)
                {
                    NotificationWindow.ShowNotification("Correct Code", NotificationType.Success, "Success");
                    codeBox.Password = "";
                    MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
                    masterPassConfirm.Tag = "Insert your new Master Key";
                    masterPassConfirm.acceptBtn.Visibility = Visibility.Visible;
                    masterPassConfirm.confirmBtn.Visibility = Visibility.Collapsed;   
                    bool? dialogResult = masterPassConfirm.ShowDialog();
                    if (dialogResult == true) 
                    {
                      
                    }
                    EmailSender.Code = null;
                }
                else
                {
                    NotificationWindow.ShowNotification("The code you have inserted is incorrect.", NotificationType.Error, "Error!");
                    codeBox.Password = "";
                }

                sendCodeBtn.BorderBrush = Brushes.White;
                codeBtnState = sendCodeBtnState.Send;
                sendCodeBtn.Content = "Send code";
                codeBox.IsEnabled = false;
            }


        }



        private void themeToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (themeToggleBtn.Content.Equals("Dark Theme"))
            {
                themeToggleBtn.Content = "Light Theme"; 
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary
                {
                    Source = new Uri("/Themes/UIColors_Dark.xaml", UriKind.Relative)
                };
            }
            else
            {
                themeToggleBtn.Content = "Dark Theme"; 
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary
                {
                    Source = new Uri("/Themes/UIColors.xaml", UriKind.Relative)
                };
            }

        }
    }

}
