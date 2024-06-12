using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace keymanager_Dev.Widgets
{
    /// <summary>
    /// Lógica de interacción para ConfirmOrCancelWindow.xaml
    /// </summary>
    public partial class ConfirmOrCancelWindow : Window
    {
        public ConfirmOrCancelWindow(string title ="Deleting data", string description = "Are you sure ?")
        {
            InitializeComponent();
            titleLbl.Content = title;
            descriptionLbl.Content = description;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
