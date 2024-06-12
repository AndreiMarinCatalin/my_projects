using keymanager_Dev.Clases;
using keymanager_Dev.Pages;
using keymanager_Dev.ViewModel;
using keymanager_Dev.Widgets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using static keymanager_Dev.Widgets.MasterPassConfirm;

namespace keymanager_Dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        UserDao userDao = new UserDao();
        HomeView homeView = new HomeView();
        SettingsView settingsView = new SettingsView();
        AboutView aboutView = new AboutView();
        public MainWindow()
        {

            if (!checkSetupLicense())
            {

                System.Windows.Application.Current.Shutdown();

            }
            else
            {
                InitializeComponent();
                this.Loaded += MainWindow_Loaded;
                homeBtn.IsChecked= true;
                setUsername();
                //testDDBB();

                List<User> users = userDao.getAll();
                if (users != null)
                {

                }
                else
                {
                    string username = GetWindowsUserName();

                    //Si en la base no hay ningun usuario
                    //if (true)
                    ////{
                    welcomeUser(username);

                }

            }

        }

        private void welcomeUser(string username)
        {
            EmailForm emailForm = new EmailForm(this.myFrame, username);
            myFrame.NavigationService.Navigate(emailForm);
        }
        /// <summary>
        /// Gets the windows Username
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsUserName()
        {
            return Environment.UserName;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // cogemos la resolución
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var screenWidth = SystemParameters.PrimaryScreenWidth;

            // Calcular la altura deseada como una fracción de la altura de la pantalla.
            // hacer que la ventana sea el 90% de la altura de la pantalla:
            var desiredHeight = screenHeight * 0.90;

            // Limitamos pantalla a 780 de altura
            if (desiredHeight > 780)
            {
                desiredHeight = 780;

            }

            this.Top = (screenHeight - desiredHeight) /3;
            this.Left = (screenWidth - this.Width) / 2;

            this.Height = desiredHeight;

        }

        private bool checkSetupLicense()
        {
            bool isValidLicense = false;
            bool hasLicense = License.checkLicense();

            if (hasLicense)
            {
                Debug.WriteLine("Licencia encontrada.");
                string licensePath = System.IO.Path.Combine(appDataFolderPath, "License.lic");
                string content = File.ReadAllText(licensePath);

                isValidLicense = License.validateLicense(content);
            }

            if (!isValidLicense)
            {
                LicenseForm form = new LicenseForm();
                bool? dialogResult = form.ShowDialog();

                if (dialogResult == true)
                {
                    form.Close();
                    isValidLicense = true;
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }

            return isValidLicense;
        }



        private bool openLicenseForm()
        {

            LicenseForm form = new LicenseForm();
            bool? dialogResult = form.ShowDialog();

            if (dialogResult == true)
            {
                form.Close();
                // isValidLicense = true;
                return true;
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
                return false;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current?.Shutdown();

        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Maximized;
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                //WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                WindowState = WindowState.Normal;
                //WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        


        private void menuOption_Checked(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.RadioButton selectedRadioButton = sender as System.Windows.Controls.RadioButton;

            if (selectedRadioButton != null && selectedRadioButton.IsChecked == true)
            {
                Dictionary<string, Action> actions = new Dictionary<string, Action>
                {
                    { "homeBtn", () => showView(homeView) },
                    { "settingsBtn", () => showView(settingsView) },
                    { "aboutBtn", () => showView(aboutView) }
                };

                if (actions.ContainsKey(selectedRadioButton.Name))
                {
                    actions[selectedRadioButton.Name]();
                }
            }
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
        }

        private void showView(object obj)
        {
            contentControl.Content = null;

            if (contentControl == null)
            {
            } else
            {
                contentControl.Content = obj;
                if (obj == homeView)
                {
                    homeView.reloadPage();
                }
            }
        }


        private void setUsername()
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string userPC = textInfo.ToTitleCase(Environment.UserName);
            userNameTxtBlock.Text = userPC;
        }
    }

}

