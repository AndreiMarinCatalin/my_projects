using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace keymanager_Dev.ViewModel
{
    /// <summary>
    /// Lógica de interacción para profileCard.xaml
    /// </summary>
    public partial class profileCard : UserControl
    {
        string linkedIn_link = "";
        public profileCard(string iconPath, string name, string linkPath)
        {
            InitializeComponent();
            try
            {
                if (iconPath == null || iconPath == "")
                {
                    iconPath = "../../../Images/logo.png";
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(iconPath, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();
                iconImageBrush.ImageSource = bitmap;

                nameTxtBLock.Text = name;
                linkedIn_link = linkPath;
            }
            catch (DirectoryNotFoundException e)
            {
                // Directory not found exception
            }

        }

        private void linkBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkedIn_link) { UseShellExecute = true });
        }
    }
}
