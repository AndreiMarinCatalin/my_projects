using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
    /// Lógica de interacción para AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            profileCard pcard = new profileCard("C:/Users/andre/OneDrive/Imágenes/Álbum de cámara/1703069387132.jpeg", "Andrei Catalin Marin", "https://www.linkedin.com/in/andrei-marin-dev/");
            profileCard xpcard = new profileCard("C:/Users/andre/OneDrive/Imágenes/Álbum de cámara/1691497722678.jpeg", "Jonathan Vladimir Segura", "https://www.linkedin.com/in/jonathan-segura-2a7684199/");
            teamStackPanel.Children.Add(pcard);
            teamStackPanel.Children.Add(xpcard);
        }
    }
}
