using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security;
using System;
using static keymanager_Dev.Widgets.NotificationWindow;
using keymanager_Dev.Widgets;
using System.Threading.Channels;
using keymanager_Dev.Pages;

namespace keymanager_Dev.ViewModel
{
    public partial class PasswordCard : UserControl
    {
        public bool IsChecked = false;
        public int Id;
        public DateTime expiration_Date;
        Color themeColor = (Color)Application.Current.Resources["cardThemeColor"];
        Color btn_foreground = (Color)Application.Current.Resources["card_btn_foreground"];
        Color btn_foreground_checked = (Color)Application.Current.Resources["card_btn_foreground_checked"];
        Color checked_background= (Color)Application.Current.Resources["card_checked_background"];

        

        public void Check()
        {
            btnCheckBox_Click(null, null);
        }

        public bool isChecked()
        {
            return IsChecked;
        }

        public event EventHandler IsCheckedChanged;

        public event EventHandler SomeEvent;
        public event EventHandler EditEvent;

        private void RaiseEditEvent()
        {
            EditEvent?.Invoke(this, EventArgs.Empty);
        }
        private void RaiseSomeEvent()
        {
            SomeEvent?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnIsCheckedChanged()
        {
            IsCheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler IsPasswordChanged;

        protected virtual void OnIsPasswordChanged()
        {
            IsPasswordChanged?.Invoke(this, EventArgs.Empty);
        }

        public PasswordCard(int id, string title, string username, string password, DateTime expDate)
        {
            InitializeComponent();

            Id = id;
            titleCard.Content = title;
            usernameCard.Text = username;
            passCard.Password = password;
            expiration_Date = expDate;

            if (isExpired())
            {
                dateAlertBtn.Visibility = Visibility.Visible;
                column0.Width = new GridLength(45);
            }
            else
            {
                dateAlertBtn.Visibility = Visibility.Hidden;
                column0.Width = new GridLength(0);
            }
        }

        public void ChangeCheck()
        {
            btnCheckBox_Click(null, null);
        }

        private void btnCheckBox_Click(object sender, RoutedEventArgs e)
        {
            IsChecked = !IsChecked;

            if (!IsChecked)
            {
                btnCheckBox.Content = "⭕";
                btnCheckBox.FontSize = 30;
                btnCheckBox.FontWeight = FontWeights.Bold;
                SetButtonColors(btnCheckBox, themeColor, btn_foreground);
            }
            else
            {
                btnCheckBox.Content = "✔";
                btnCheckBox.FontSize = 20;
                SetButtonColors(btnCheckBox, checked_background, btn_foreground_checked);
                OnIsCheckedChanged();

            }
        }
        private void SetButtonColors(Button button, string hexBackgroundColor, string hexForegroundColor)
        {
            SolidColorBrush backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexBackgroundColor));
            SolidColorBrush foregroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexForegroundColor));

            button.Background = backgroundBrush;
            button.Foreground = foregroundBrush;
        }

        private void SetButtonColors(Button button, Color backgroundColor, Color foregroundColor)
        {
            SolidColorBrush backgroundBrush = new SolidColorBrush(backgroundColor);
            SolidColorBrush foregroundBrush = new SolidColorBrush(foregroundColor);

            button.Background = backgroundBrush;
            button.Foreground = foregroundBrush;
        }



        private void SetButtonColors(Button button, SolidColorBrush background, SolidColorBrush foreground)
        {

            button.Background = background;
            button.Foreground = foreground;
        }

        private void dateAlertBtn_Click(object sender, RoutedEventArgs e)
        {
            NotificationWindow.ShowNotification("This password has expired. You should change your password.", NotificationType.Error, "Password expired");
            MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
            bool? dialogRes = masterPassConfirm.ShowDialog();
            if (dialogRes == true)
            {
                ChangePassWindow changePassWindow = new ChangePassWindow(Id);
                bool? dialogResult = changePassWindow.ShowDialog();

                if (dialogResult == true)
                {
                    //NotificationWindow.ShowNotification("Password changed successfully!", NotificationType.Success, "Password changed");
                    RaiseSomeEvent();
                }
            }





        }

        private bool isExpired()
        {
            if (expiration_Date <= DateTime.Now)
            {
                return true;
            }
            return false;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            MasterPassConfirm masterPassConfirm = new MasterPassConfirm();
            bool? dialogRes = masterPassConfirm.ShowDialog();
            if (dialogRes == true)
            {
                EditCredentialWindow ecw  = new EditCredentialWindow(Id, titleCard.Content.ToString(), usernameCard.Text, passCard.Password);
                bool? dialogResult = ecw.ShowDialog();

                if (dialogResult == true)
                {
                    RaiseEditEvent();
                }
            }
        }
    }

}
