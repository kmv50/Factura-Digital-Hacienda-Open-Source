using DataModel.EF;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace FacturaDigital.Settings
{
    /// <summary>
    /// Lógica de interacción para DbSettingsConnection.xaml
    /// </summary>
    public partial class DbSettingsConnection : Page
    {
        public DbSettingsConnection()
        {
            InitializeComponent();
            try
            {
                ConnectionSettings values = new ConnectionSettings().LoadSettings();
                if(values != null)
                {
                    txt_dbName.Text = values.Database;
                    txt_host.Text = values.Servidor;
                    txt_password.Password = values.Contrasena;
                    txt_Usuario.Text = values.Usuario;
                }
            }
            catch
            {

            }
        }

        private void GuardarCredenciales() {
            new ConnectionSettings().SaveSettings(new ConnectionSettings()
            {
                Contrasena = txt_password.Password,
                Database = txt_dbName.Text,
                Servidor = txt_host.Text,
                Usuario = txt_Usuario.Text
            });
        }

        private void SaveConexion(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_dbName.Text) || string.IsNullOrEmpty(txt_host.Text) || string.IsNullOrEmpty(txt_password.Password) || string.IsNullOrEmpty(txt_Usuario.Text))
                {
                    MessageBox.Show("Ocupo todos los datos para poder conectarme a la base de datos (Esos inputs no son de adorno). Si no los tiene no me haga perder el tiempo");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Error al validar los datos","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            try
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection("Server=" + txt_host.Text + ";Port=3306;Uid=" + txt_Usuario.Text + ";Pwd=" + txt_password.Password))
                    {
                        conn.Open();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            MessageBox.Show("No que va con los datos que me dio no me puedo conectar a la base de datos. Gracias por jugar", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("No que va con los datos que me dio no me puedo conectar a la base de datos. Gracias por jugar", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                 


                using (db_FacturaDigital db = new db_FacturaDigital())
                {

                    string conectionString = "server=" + txt_host.Text + ";user id=" + txt_Usuario.Text + ";password=" + txt_password.Password + ";persistsecurityinfo=True;database=" + txt_dbName.Text;
                    db.Database.Connection.ConnectionString = conectionString;
                    if (!db.Database.Exists())
                    {
                       if(MessageBox.Show("Ya me pude conectar a la base de datos lo unico es que el esquema no existe. Quiere que lo cree?","Crear esquema",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            db.Database.CreateIfNotExists();
                            if (db.Database.Exists())
                            {
                                GuardarCredenciales();
                                MessageBox.Show("Listo conexion establecida.", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                                Recursos.RecursosSistema.OnStartMain_Load();
                            }
                        }
                    }
                    else
                    {
                        GuardarCredenciales();
                        MessageBox.Show("Listo conexion establecida.", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        Recursos.RecursosSistema.OnStartMain_Load();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Si algo salio realmente mal intente con otra combinacion. "+ex.Message+" "+ex.StackTrace,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
