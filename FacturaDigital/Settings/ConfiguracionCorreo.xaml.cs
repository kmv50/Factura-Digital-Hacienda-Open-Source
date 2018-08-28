using DataModel;
using DataModel.EF;
using FacturaDigital.Recursos;
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

namespace FacturaDigital.Settings
{
    /// <summary>
    /// Lógica de interacción para ConfiguracionCorreo.xaml
    /// </summary>
    public partial class ConfiguracionCorreo : Page , ILog
    {
        public ConfiguracionCorreo()
        {
            InitializeComponent();
            CargarVista();
        }

        private void CargarVista() {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    SMTP value = db.SMTP.FirstOrDefault();
                    if (value == null)
                        return;

                    txt_host.Text = value.Url_Servidor;
                    txt_Puerto.Text = value.Puerto.ToString();
                    txt_Usuario.Text = value.Usuario;
                    txt_contrasena.Password = value.Contrasena;
                    chk_SSL.IsChecked = value.SSL;
                }
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al cargar los datos del correo","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validaciones
                if (string.IsNullOrEmpty(txt_host.Text))
                {
                    MessageBox.Show("Error debe de ingresar la dirección del servidor", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (string.IsNullOrEmpty(txt_Usuario.Text))
                {
                    MessageBox.Show("Error debe de ingresar el usuario", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (string.IsNullOrEmpty(txt_contrasena.Password))
                {
                    MessageBox.Show("Error debe de ingresar la contraseña", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (string.IsNullOrEmpty(txt_Puerto.Text))
                {
                    MessageBox.Show("Error debe de ingresar el numero de puerto", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                int puerto;
                if (!int.TryParse(txt_Puerto.Text, out puerto) || puerto <= 0)
                {
                    MessageBox.Show("El puerto debe de ser un dato numerico mayor a 0", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                #endregion

                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    SMTP value = db.SMTP.FirstOrDefault();
                    if(value == null)
                    {
                        value = new SMTP();
                        db.SMTP.Add(value);
                    }

                   
                    value.Contrasena = txt_contrasena.Password;
                    value.Puerto = puerto;
                    value.Url_Servidor = txt_host.Text;
                    value.Usuario = txt_Usuario.Text;
                    value.SSL = chk_SSL.IsChecked.Value;
                    value.Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente;
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al guardar los datos del SMPT","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Datos de smtp almacenados correctamente","Informacion",MessageBoxButton.OK,MessageBoxImage.Information);

            try
            {
                RecursosSistema.OnStartMain_Load();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Reinicie el sistema para continuar");
            }
        }
    }
}
