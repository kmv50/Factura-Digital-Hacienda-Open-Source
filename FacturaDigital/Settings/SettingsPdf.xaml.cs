using DataModel;
using DataModel.EF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace FacturaDigital.Settings
{
    /// <summary>
    /// Lógica de interacción para SettingsPdf.xaml
    /// </summary>
    public partial class SettingsPdf : Page , ILog
    {
        private string FileName;
        public SettingsPdf()
        {
            InitializeComponent();
            FileName = null;
            LoadPdfIcon();
        }

        private void LoadPdfIcon() {
            try
            {
                string url = System.IO.Path.Combine(Environment.CurrentDirectory, "Logo.png");

                var bi = new BitmapImage();

                using (var fs = new FileStream(url, FileMode.Open))
                {
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }

                bi.Freeze(); 

                PngLogo.Source = bi;
            }catch(Exception ex)
            {
                this.LogError(ex);
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            if (!File.Exists(FileName))
            {
                MessageBox.Show("La ruta ingresada ya no esta disponible", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            string url = System.IO.Path.Combine(Environment.CurrentDirectory, "Logo.png");
            try
            {
                File.Delete(url);
            }
            catch (Exception ex){
                this.LogError(ex);
            }

            try
            {
                File.Copy(FileName, url);
                MessageBox.Show("La informacion se a actualizado correctamente","Informacion",MessageBoxButton.OK,MessageBoxImage.Information);
            }catch(Exception ex)
            {
                MessageBox.Show("Error al copiar la imagen seleccionada","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                this.LogError(ex);
            }
        }

        private void BuscarImg(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = "*.png";
            openFileDialog.Filter = "Logo (*.png) | *.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
                PngLogo.Source = new ImageSourceConverter().ConvertFromString(FileName) as ImageSource;
            }


        }
    }
}
