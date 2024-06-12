using keymanager_Dev.Clases;
using keymanager_Dev.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
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
using static keymanager_Dev.Clases.PassManager;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Pages
{
    /// <summary>
    /// Lógica de interacción para AddCardWindow.xaml
    /// </summary>
    public partial class AddCardWindow : Window
    {
        string titleCard;
        string username;
        string password;
        DateTime expirationDate;
        UserDao userDao;
        CredentialDao credentialDao = new CredentialDao();
        HistoryDao historyDao = new HistoryDao();

        public AddCardWindow()
        {
            InitializeComponent();

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

            Console.WriteLine("Fecha actual: " + actualDate);
            Console.WriteLine("Fecha en tres meses: " + dateinXmounths);

        }

        //public static void showCardWindow()
        //{
        //    AddCardWindow addCard = new AddCardWindow();
        //    addCard.WindowStyle = WindowStyle.None;
        //    addCard.AllowsTransparency = true;
        //    addCard.Background = Brushes.Transparent;
        //    addCard.Topmost = true;
        //    addCard.ShowDialog();
        //}
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close_Window(null, null);
        }

        private void Close_Window(object sender, EventArgs e)
        {
            this.Close();
        }

        private void generateBtn_Click(object sender, RoutedEventArgs e)
        {
            PassManager passManager = new PassManager();

            // 👍All checked 
            password = PassManager.passGenerator(12);
            // 👍All unchecked 
            if (!(bool)lowercaseCheckBox.IsChecked & !(bool)uppercaseCheckBox.IsChecked & !(bool)numbersCheckBox.IsChecked)
            {
                passwordTxtBox.Text = "";
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


            if (!string.IsNullOrEmpty(passwordTxtBox.Text))
            {
                passwordTxtBox.Text = "";
            }
            passwordTxtBox.Text = password;

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (allFieldsCompleted())
            {
                if (passwordTxtBox.Text.Length >= 6)
                {


                    userDao = new UserDao();
                    //Recuperamos la MasterKey encryptada y la desencryptamos lo cual nos devoilverá el hash de la masterKey
                    string keyHash = Encryptor.Decrypt(userDao.getUserKeyByUsername(Environment.UserName), Encryptor.KEY);
                    //Nos quedamos los primeros 16 carácteres del hash de la master key
                    string key = keyHash.Substring(0, Math.Min(keyHash.Length, 16));
                    //usamos el hash de la masterKey desencyptada para encyptar esta clave
                    string pass = Encryptor.Encrypt(passwordTxtBox.Text, key);

                    int idUser = userDao.getUserIdByUsername(Environment.UserName);
                    if (idUser != -1)
                    {
                        //List<Credential> credentials = credentialDao.getCredentialsByUserId(idUser);

                        bool validPass = findCredentialsMatches(idUser, passwordTxtBox.Text, key);

                        if (validPass)
                        {
                            Credential credential = new Credential(idUser
                           , titleTxtBox.Text, usernameTxtBox.Text,
                           pass, expirationDate);

                            int succes = credentialDao.saveCredential(credential);

                            if (succes != 0)
                            {

                                NotificationWindow.ShowNotification("Credential saved", NotificationType.Success, "Succes");
                                int history_succes = historyDao.saveCredential(credential);
                                if (history_succes == 0)
                                {
                                    NotificationWindow.ShowNotification("Error saving credential history", NotificationType.Error, "Error");
                                }
                                this.DialogResult = true;
                                Close_Window(null, null);

                            }
                            else
                            {
                                NotificationWindow.ShowNotification("Error saving credential", NotificationType.Error, "Error");
                            }
                        }
                        else 
                        {
                            NotificationWindow.ShowNotification("The password is already used", NotificationType.Error, "Error");
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

        private bool findCredentialsMatches(int idUser, string pass, string key)
        {
            List<History> history = historyDao.getCredentialsByUserId(idUser);
            if (history != null)
            {
                foreach (History historyItem in history)
                {
                    if (pass.Equals(Encryptor.Decrypt(historyItem.ExpiredPassword,key)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool allFieldsCompleted()
        {
            return !string.IsNullOrEmpty(titleTxtBox.Text)
                && !string.IsNullOrEmpty(usernameTxtBox.Text)
                && !string.IsNullOrEmpty(passwordTxtBox.Text);
        }


    }
}

