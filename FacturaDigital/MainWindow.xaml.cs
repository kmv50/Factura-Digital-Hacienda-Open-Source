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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            RecursosSistema.OnContribuyente_Load = new RecursosSistema.Contribuyente_Load(LoadContribuyente);
            InitializeComponent();
            RecursosSistema.MainConteiner = MainConteiner;
            if (TestDbConection())
            {
                LoadContribuyente();
            }

            //Esto hay q borrarlo 
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                
                new FacturaPDF.FacturaElectronicaPDF().CrearFactura(db.Factura.Include("Factura_Detalle").OrderByDescending(q => q.Id_Factura).First());
            }
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

        private void LoadContribuyente()
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    RecursosSistema.Contribuyente = db.Contribuyente.FirstOrDefault();
                    if (RecursosSistema.Contribuyente == null)
                    {
                        MessageBox.Show("Antes de continuar llene el perfil de contribuyente", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        ListViewMenu.Visibility = Visibility.Hidden;
                        PerfilHacienda(null, null);
                    }
                    else
                    {
                        ListViewMenu.Visibility = Visibility.Visible;
                        lb_mainUser.Text = RecursosSistema.Contribuyente.Nombre;
                    }
                }

                RecursosSistema.IniciarServicioConsulta();
            }
            catch (Exception ex)
            {
                ListViewMenu.Visibility = Visibility.Hidden;
                MessageBox.Show("Error al consultar la base de datos. Esto de se puede deber a que la base de datos no responde. Verfique que el servidor de mysql se encuente en linea. Verifique los datos de conexion de la base datos. " + ex.Message + " " + ex.StackTrace, "Erro", MessageBoxButton.OK);
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
    }
}
