using DataModel;
using DataModel.EF;
using FacturaDigital.Recursos;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FacturaDigital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , ILog
    {
        bool ConexionDBChequeada, DatosSMTP , DatosContribuyente;
        public MainWindow()
        {
            RecursosSistema.OnStartMain_Load = new RecursosSistema.StartMain_Load(StartMain);
            InitializeComponent();
            ListViewMenu.Visibility = Visibility.Hidden;
            RecursosSistema.MainConteiner = MainConteiner;
            StartMain();         
        }

        private bool TestDbConection()
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    if (!db.Database.Exists())
                    {
                        MainConteiner.Content = new Settings.DbSettingsConnection();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch
            {
                MainConteiner.Content = new Settings.DbSettingsConnection();
                return false;
            }
        }

        private void StartMain() {
            if (!ConexionDBChequeada  && !TestDbConection())
                return;
            else
                ConexionDBChequeada = true;

            if (!DatosContribuyente && !VerificarDatosContribuyente())
                return;
            else
                DatosContribuyente = true;

            if (!DatosSMTP && !CheckSmtp())
                return;
            else
                DatosSMTP = true;


            ListViewMenu.Visibility = Visibility.Visible;
            lb_mainUser.Text = RecursosSistema.Contribuyente.Nombre;
            RecursosSistema.IniciarServicioConsulta();
            ShowHistorial();
        }

        private bool CheckSmtp()
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    if(db.SMTP.Any())                    
                        return true;

                    MessageBox.Show("Antes de continuar debe llenar los datos de smpt con el fin de poder enviar sus facturas", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    ConfiguracionCorreo(null, null);
                    return false;
                }
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al validar datos de SMTP","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
        }

        private bool VerificarDatosContribuyente()
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    RecursosSistema.Contribuyente = db.Contribuyente.FirstOrDefault();
                    if (RecursosSistema.Contribuyente == null)
                    {
                        MessageBox.Show("Antes de continuar llene el perfil de contribuyente", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        PerfilHacienda(null, null);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ListViewMenu.Visibility = Visibility.Hidden;
                MessageBox.Show("Error al consultar la base de datos. Esto de se puede deber a que la base de datos no responde. Verfique que el servidor de mysql se encuente en linea. Verifique los datos de conexion de la base datos. " + ex.Message + " " + ex.StackTrace, "Erro", MessageBoxButton.OK);
                return false;
            }

        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }


        private void ShowCrearFactura()
        {
            MainConteiner.Content = new Faturacion.Facturar();
        }

        private void ShowHistorial()
        {
            MainConteiner.Content = new Historial.Historial();
        }

        private void ShowProductos_Servicios()
        {
            MainConteiner.Content = new Productos.Lista_Productos();
        }

        private void ShowClientes()
        {
            MainConteiner.Content = new Clientes.Lista_Clientes();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCloseMenu_Click(sender, e);
            string ItemSelected = ((ListViewItem)((ListView)sender).SelectedItem).Name;
            switch (ItemSelected)
            {
                case "Facturar": ShowCrearFactura(); break;
                case "Historial": ShowHistorial(); break;
                case "Clientes": ShowClientes(); break;
                case "Productos_Servicios": ShowProductos_Servicios(); break;
                default: MessageBox.Show("Error al seleccionar el tipo de menu", "Error", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }

        }

        private void PerfilHacienda(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu_Click(sender, e);
            MainConteiner.Content = new Contribuyente.PerfilHacienda();
        }

        private void Consecutivos(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu_Click(sender, e);
            MainConteiner.Content = new Settings.Consecutivos();
        }

        private void ConfiguracionCorreo(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu_Click(sender, e);
            MainConteiner.Content = new Settings.ConfiguracionCorreo();
        }
    }
}
