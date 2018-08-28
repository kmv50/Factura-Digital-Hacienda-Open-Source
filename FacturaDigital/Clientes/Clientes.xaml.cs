using DataModel;
using DataModel.EF;
using DataModel.UbicacionesData;
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

namespace FacturaDigital.Clientes
{
    /// <summary>
    /// Interaction logic for Clientes.xaml
    /// </summary>
    public partial class Clientes : Page , ILog
    {
        public Clientes()
        {
            InitializeComponent();
            cb_Provincia.ItemsSource = new UbicacionesData().GetProvincias();
            LoadEvents();
            txt_Identificacion.Visibility = Visibility.Hidden;
        }

        private void LoadEvents()
        {
            cb_Provincia.SelectionChanged += SeleccionProvincia;
            cb_canton.SelectionChanged += SeleccionarCanton;
            cb_distrito.SelectionChanged += SeleccionarDistrito;
            cb_Provincia.SelectionChanged += SeleccionProvincia;
            cb_Cedula.SelectionChanged += Cb_Cedula_SelectionChanged;
        }

        private void Cb_Cedula_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_Identificacion.Text = null;
            if (cb_Cedula.SelectedIndex != 0)
            {
                txt_Identificacion.Visibility = Visibility.Visible;
            }
        }

        private void SeleccionProvincia(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                cb_canton.ItemsSource = new UbicacionesData().GetCantones(provincia.Id);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarCanton(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                UbicacionesType canton = (UbicacionesType)cb_canton.SelectedItem;
                cb_distrito.ItemsSource = new UbicacionesData().GetDistritos(provincia.Id, canton.Id);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarDistrito(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                UbicacionesType canton = (UbicacionesType)cb_canton.SelectedItem;
                UbicacionesType distrito = (UbicacionesType)cb_distrito.SelectedItem;
                cb_barrio.ItemsSource = new UbicacionesData().GetBarrios(provincia.Id, canton.Id, distrito.Id);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Cliente cli = new Cliente();
                    cli.Nombre = txt_Nombre.Text;
                    cli.CorreoElectronico = txt_Correo.Text;
                    if (string.IsNullOrEmpty(cli.Nombre))
                    {
                        MessageBox.Show("Favor ingresar el nombre del cliente","Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                        return;
                    }

                    if (string.IsNullOrEmpty(cli.CorreoElectronico))
                    {
                        MessageBox.Show("Favor ingresar el correo del cliente", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    cli.NombreComercial = txt_comercial.Text;
                    cli.Identificacion_Numero = txt_Identificacion.Text;
                    if(cb_Cedula.SelectedItem != null &&  !string.IsNullOrEmpty((cb_Cedula.SelectedItem as ComboBoxItem).Tag as string))
                        cli.Identificacion_Tipo = (cb_Cedula.SelectedItem as ComboBoxItem).Tag.ToString();
                    else
                        cli.Identificacion_Tipo = null;

                    #region Telefono
                    if (string.IsNullOrEmpty(txt_TelefonoRegion.Text))
                    {
                        cli.Telefono_Codigo = null;
                    }
                    else
                    {
                        int Region;
                        if (!int.TryParse(txt_TelefonoRegion.Text, out Region))
                        {
                            MessageBox.Show("Favor llenar el campo de region (solamente numeros) ", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.Telefono_Codigo = Region;
                    }

                    if (string.IsNullOrEmpty(txt_TelefonoNumero.Text))
                    {
                        cli.Telefono_Numero = null;
                    }
                    else
                    {
                        int telefono;
                        if (!int.TryParse(txt_TelefonoNumero.Text, out telefono))
                        {
                            MessageBox.Show("Favor llenar el campo de telefono (solamente numeros)", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.Telefono_Numero = telefono;
                    }
                    #endregion

                    #region Ubicacion
                    UbicacionesType provincia = cb_Provincia.SelectedItem as UbicacionesType;
                    if (provincia != null)
                    {
                        cli.Provincia = provincia.Id;
                        UbicacionesType canton = cb_canton.SelectedItem as UbicacionesType;
                        if (canton == null)
                        {
                            MessageBox.Show("Favor seleccionar una Canton", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.Canton = canton.Id;


                        UbicacionesType Distrito = cb_distrito.SelectedItem as UbicacionesType;
                        if (Distrito == null)
                        {
                            MessageBox.Show("Favor seleccionar una Distrito", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.Distrito = Distrito.Id;


                        UbicacionesType Barrio = cb_barrio.SelectedItem as UbicacionesType;
                        if (Distrito == null)
                        {
                            MessageBox.Show("Favor seleccionar una Barrio", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.Barrio = Barrio.Id;


                        if (string.IsNullOrEmpty(txt_otrasSenas.Text))
                        {
                            MessageBox.Show("Favor llenar el campo de otras senas", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        cli.OtrasSenas = txt_otrasSenas.Text;
                    }
                    #endregion

                    if (cli.Identificacion_Numero != null)
                    {
                        if (db.Cliente.Any(q => q.Identificacion_Numero == cli.Identificacion_Numero))
                        {
                            MessageBox.Show("Ya existe un cliente con ese numero de identificacion", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    else
                    {
                        if (db.Cliente.Any(q => q.Nombre == cli.Nombre))
                        {
                            MessageBox.Show("Ya existe un cliente con ese nombre registrado", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    cli.Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente;
                    db.Cliente.Add(cli);
                    db.SaveChanges();
                    Volver(sender,e);
                }
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al guardar datos del cliente","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Volver(object sender, RoutedEventArgs e)
        {
            try
            {
                RecursosSistema.MainConteiner.Content = new Lista_Clientes();
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al volver a la lista de clientes","Error",MessageBoxButton.OK,MessageBoxImage.Error);            
            }
        }
    }
}
