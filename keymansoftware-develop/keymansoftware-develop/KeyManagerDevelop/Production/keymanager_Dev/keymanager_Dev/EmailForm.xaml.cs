using keymanager_Dev.Clases;
using keymanager_Dev.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev
{
    /// <summary>
    /// Lógica de interacción para EmailForm.xaml
    /// </summary>
    public partial class EmailForm : Page
    {
        string Code;

        private readonly Frame _myFrame;
        User user;
        UserDao userDao;

        public EmailForm(Frame frame, string username)
        {
            InitializeComponent();
            tbWelcome.Text = AppTexts.WelcomeText;
            lbInstructions.Content = AppTexts.Welcome2;
            tbUsername.Text = username;
            FirstGrid.Visibility = Visibility.Visible;
            SecondGrid.Visibility = Visibility.Collapsed;
            FirstGridRow.IsEnabled = false;
            FirstGridRow.Height = GridLength.Auto;
            SecondGridRow.IsEnabled = true;

            //Code = "NUM";
            lbKey_Instructions.Text = AppTexts.NewMasterkey;
            user = new User();
            userDao = new UserDao();
            this._myFrame = frame;

        }


        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbEmail.Text))
            {
                if (IsValidEmail(tbEmail.Text))
                {
                    Code = generateCode(6);

                    //Check the generated code
                    if (!string.IsNullOrEmpty(Code))
                    {   

                        EmailSender.SendEmailAsync(Code, tbEmail.Text);
                        


                    }
                    else
                    {
                        NotificationWindow.ShowNotification("Error", NotificationType.Error, "Fail generating code");
                    }
                }
                else
                {
                    NotificationWindow.ShowNotification("Error", NotificationType.Error, "Invalid email address");
                }
            }
            else
            {
                NotificationWindow.ShowNotification("Error", NotificationType.Error, "Email cannot be empty");
            }
        }

        //private async Task sendMail(string code, string v)
        //{
        //    EmailSender eSender = new EmailSender(code, v);
        //    await eSender.SendEmailAsync();

        //}

        private string generateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool IsValidEmail(string email)
        {
            // Use a regular expression to validate the email format
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return new Regex(pattern).IsMatch(email);
        }


        private void btValidate_Click(object sender, RoutedEventArgs e)
        {
            if (tbCode.Text.Equals(Code))
            {

                FirstGrid.Visibility = Visibility.Collapsed;
                SecondGrid.Visibility = Visibility.Visible;

                //Saves the user
                user.Username = tbUsername.Text;
                user.Email = tbEmail.Text;
            }
            else
            {
                NotificationWindow.ShowNotification("Code Incorrect", NotificationType.Error, "Error");

            }

            }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {

            FirstGrid.Visibility = Visibility.Visible;
            SecondGrid.Visibility = Visibility.Collapsed;
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            string password = new System.Net.NetworkCredential(string.Empty, tbKey.SecurePassword).Password;
            string confirmPassword = new System.Net.NetworkCredential(string.Empty, tbconfirm.SecurePassword).Password;

            // Si los campos están vacíos
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {

                NotificationWindow.ShowNotification("Fields should be completed", NotificationType.Error, "Error");
                return;
            }

            // Si el texto tiene menos de 6 caracteres
            if (password.Length < 6 || confirmPassword.Length < 6)
            {

                NotificationWindow.ShowNotification("Password should contain at least 6 characters", NotificationType.Error, "Error");
                return;
            }

            // Si contiene espacios en blanco
            if (password.Contains(" ") || confirmPassword.Contains(" "))
            {

                NotificationWindow.ShowNotification("Password should not contain spaces", NotificationType.Error, "Error");
                return;
            }

            // Si los textos no son iguales
            if (password != confirmPassword)
            {

                NotificationWindow.ShowNotification("Passwords do not match", NotificationType.Error, "Error");
                return;
            }

            string hashKey = Encryptor.GenerateHash(password);
            string masterKey = Encryptor.Encrypt(hashKey, Encryptor.KEY);
            user.MasterKey = masterKey;


            int succes = userDao.saveUser(user);

            if (succes != 0)
            {
                NotificationWindow.ShowNotification("Correct password", NotificationType.Success, "Succes");
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow.myFrame.NavigationService.Navigate(null);
            }
            else
            {
                NotificationWindow.ShowNotification("En error ocurred", NotificationType.Error, "Error");
            }
            // Si todos los requisitos se cumplen


        }


        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
