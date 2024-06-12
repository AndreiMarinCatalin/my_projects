using keymanager_Dev.Clases;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.VisualBasic;
using keymanager_Dev.Widgets;
using static keymanager_Dev.Widgets.NotificationWindow;

namespace keymanager_Dev.Pages
{
    /// <summary>
    /// Lógica de interacción para LicenseForm.xaml
    /// </summary>
    public partial class LicenseForm : Window
    {
        public LicenseForm()
        {
            InitializeComponent();
            productKeyLbl.Text = License.Encrypt(License.GetMacAddress());
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            // Saves the product key in the Clipboard
            
            string textToCopy = productKeyLbl.Text;
            Clipboard.SetText(textToCopy);
            MessageBox.Show("Mensaje: " + textToCopy);

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            var vfbd = new OpenFileDialog();


            vfbd.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            vfbd.Title = "Select a License file";
            vfbd.CheckFileExists = true;
            vfbd.CheckPathExists = true;
            vfbd.Multiselect = false;

            //Filtrar los archivos xml y visum
            vfbd.Filter = "License Files (*.lic)|*.lic";
            if (vfbd.ShowDialog() == true)
            {
                
                var selectedDirectorie = vfbd.FileName;
                var selectedFileName = System.IO.Path.GetFileName(selectedDirectorie);
                TextField.Text = selectedDirectorie;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
            Application.Current.Shutdown();
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextField.Text))
            {
                //Si el textBox está vacio, mensaje de error para indicar rellenar textbox
            }
            else
            {
                string filePath = TextField.Text.ToString();

                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        if (License.validateLicense(content))
                        {
                            string destinationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                            // Si el directorio de destino no existe, crearlo.
                            if (!Directory.Exists(destinationDirectory))
                            {
                                Directory.CreateDirectory(destinationDirectory);
                            }

                            // Crear la ruta de archivo completa para el destino.
                            string destinationFilePath = System.IO.Path.Combine(destinationDirectory, "License.lic");

                            // Si el archivo de destino ya existe, eliminarlo.
                            if (File.Exists(destinationFilePath))
                            {
                                File.Delete(destinationFilePath);
                            }

                            // Mueve el archivo de licencia a la nueva ubicación.
                            File.Move(filePath, destinationFilePath);

                            this.DialogResult = true;
                            NotificationWindow.ShowNotification("Valid License", NotificationType.Success, "Success");
                            this.Close();
                        }
                        else
                        {
                            NotificationWindow.ShowNotification("This is not a valid license.", NotificationType.Error, "Error");
                            //Si la Licencia no es válida
                        }
                    }
                    else
                    {
                     
                        //Si el archivo seleccionado tiene datos escrito y no esta vacío
                    }


                }
                else
                {
                   //Si el archivo seleccionado no existe.
                }

            }
        }

        private void ClosingLicense(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.DialogResult = false;
            //this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
