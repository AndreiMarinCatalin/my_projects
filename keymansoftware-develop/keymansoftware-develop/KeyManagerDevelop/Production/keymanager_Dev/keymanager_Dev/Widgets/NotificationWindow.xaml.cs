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
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Widgets
{
    /// <summary>
    /// Lógica de interacción para NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public enum NotificationType
        {
            Success, Warning, Error, Information
        }
        public NotificationWindow(string messageContent, NotificationType messageType, string messageTitle)
        {
            InitializeComponent();
            DataContext = this;
            notificationTitle.Content = messageTitle;
            contentTextBlock.Text = messageContent;
            ModelType = messageType;
            iconPath = IconPath();
            Topmost= true;
        }

        public static void ShowNotification(string messageContent, NotificationType messageType, string messageTitle)
        {
            NotificationWindow window = new NotificationWindow(messageContent, messageType, messageTitle);
            window.ShowDialog();
        }

        public static void licenseExpired(string messageContent, string messageTitle)
        {
            NotificationWindow window = new NotificationWindow(messageContent, NotificationType.Error, messageTitle);
            window.ShowDialog();

            if (window.DialogResult.HasValue && window.DialogResult.Value)
            {
                Application.Current.Shutdown();
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string IconPath()
        {
            switch (ModelType)
            {
                case NotificationType.Success:
                    return "../Images/success.png";
                case NotificationType.Warning:
                    return "../Images/warning.png";
                case NotificationType.Error:
                    return "../Images/error.png";
                case NotificationType.Information:
                    return "../Images/information.png";
                default:
                    throw new ArgumentException("Invalid MessageType");
            }
        }


        public NotificationType ModelType { get; set; }
        public string iconPath { get; set; }




    }
}


