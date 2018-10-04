using DataModel;
using DataModel.EF;
using DataModel.UbicacionesData;
using FacturaDigital.Recursos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FacturaDigital.Clientes
{
    /// <summary>
    /// Interaction logic for Clientes.xaml
    /// </summary>
    public partial class Clientes : Page, ILog
    {
        private Cliente ClienteActual;
        private void StarView()
        {
            InitializeComponent();
            cb_Provincia.ItemsSource = new UbicacionesData().GetProvincias();
            txt_Identificacion.Visibility = Visibility.Hidden;
        }

        public Clientes()
        {
            StarView();
            ClienteActual = null;
            LoadEvents();
        }

        public Clientes(int Id_Cliente)
        {
            StarView();
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                ClienteActual = db.Cliente.FirstOrDefault(q => q.Id_Cliente == Id_Cliente);
            }

            if (ClienteActual == null)
            {
                return;
            }

            LoadCliente();
            LoadEvents();
        }

        private void LoadCliente()
        {
            txt_Nombre.Text = ClienteActual.Nombre;
            txt_Correo.Text = ClienteActual.CorreoElectronico;
            txt_comercial.Text = ClienteActual.NombreComercial;
            try
            {
                if (!string.IsNullOrEmpty(ClienteActual.Identificacion_Tipo))
                {
                    int SelectIndex = 0;
                    switch (ClienteActual.Identificacion_Tipo)
                    {
                        case "01": SelectIndex = 1; break;
                        case "02": SelectIndex = 2; break;
                        case "03": SelectIndex = 3; break;
                        case "04": SelectIndex = 4; break;
                        case "EX": SelectIndex = 5; break;
                        default: SelectIndex = 0; break;
                    }
                    cb_Cedula.SelectedIndex = SelectIndex;
                    if (SelectIndex != 0)
                    {
                        txt_Identificacion.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                cb_Cedula.SelectedIndex = 0;
                this.LogError(ex);
            }

            txt_Identificacion.Text = ClienteActual.Identificacion_Numero;

            if (ClienteActual.Telefono_Codigo.HasValue)
            {
                txt_TelefonoRegion.Text = ClienteActual.Telefono_Codigo.Value.ToString();
            }

            if (ClienteActual.Telefono_Numero.HasValue)
            {
                txt_TelefonoNumero.Text = ClienteActual.Telefono_Numero.Value.ToString();
            }

            try
            {
                if (ClienteActual.Barrio.HasValue)
                {
                    cb_Provincia.SelectedItem = (cb_Provincia.ItemsSource as IEnumerable<UbicacionesType>).First(q => q.Id == ClienteActual.Provincia);
                    cb_canton.ItemsSource = new UbicacionesData().GetCantones(ClienteActual.Provincia.Value);
                    cb_canton.SelectedItem = (cb_canton.ItemsSource as IEnumerable<UbicacionesType>).First(q => q.Id == ClienteActual.Canton);
                    cb_distrito.ItemsSource = new UbicacionesData().GetDistritos(ClienteActual.Provincia.Value, ClienteActual.Canton.Value);
                    cb_distrito.SelectedItem = (cb_distrito.ItemsSource as IEnumerable<UbicacionesType>).First(q => q.Id == ClienteActual.Distrito);
                    cb_barrio.ItemsSource = new UbicacionesData().GetBarrios(ClienteActual.Provincia.Value, ClienteActual.Canton.Value, ClienteActual.Distrito.Value);
                    cb_barrio.SelectedItem = (cb_barrio.ItemsSource as IEnumerable<UbicacionesType>).First(q => q.Id == ClienteActual.Barrio);
                    txt_otrasSenas.Text = ClienteActual.OtrasSenas;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
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
            else
            {
                txt_Identificacion.Visibility = Visibility.Hidden;
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
                    bool ClienteNuevo = false;
                    if (ClienteActual == null)
                    {
                        ClienteActual = new Cliente();
                        ClienteNuevo = true;
                        ClienteActual.Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente;
                    }
                    else
                    {
                        db.Cliente.Attach(ClienteActual);
                    }

                    ClienteActual.Nombre = txt_Nombre.Text;
                    ClienteActual.CorreoElectronico = txt_Correo.Text;
                    if (string.IsNullOrEmpty(ClienteActual.Nombre))
                    {
                        MessageBox.Show("Favor ingresar el nombre del cliente", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    if (string.IsNullOrEmpty(ClienteActual.CorreoElectronico))
                    {
                        MessageBox.Show("Favor ingresar el correo del cliente", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    ClienteActual.NombreComercial = txt_comercial.Text;
                    ClienteActual.Identificacion_Numero = txt_Identificacion.Text;

                    if (!string.IsNullOrEmpty(ClienteActual.Identificacion_Numero))
                    {
                        for (int i = 0; i < ClienteActual.Identificacion_Numero.Length; i++)
                        {
                            if (!char.IsDigit(ClienteActual.Identificacion_Numero[i]))
                            {
                                MessageBox.Show("El numero de identificacion solo puede contener nuemeros", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                                return;
                            }
                        }
                    }

                    if (cb_Cedula.SelectedItem != null && !string.IsNullOrEmpty((cb_Cedula.SelectedItem as ComboBoxItem).Tag as string))
                    {
                        ClienteActual.Identificacion_Tipo = (cb_Cedula.SelectedItem as ComboBoxItem).Tag.ToString();
                    }
                    else
                    {
                        ClienteActual.Identificacion_Tipo = null;
                    }

                    #region Telefono
                    if (string.IsNullOrEmpty(txt_TelefonoRegion.Text))
                    {
                        ClienteActual.Telefono_Codigo = null;
                    }
                    else
                    {
                        if (!int.TryParse(txt_TelefonoRegion.Text, out int Region))
                        {
                            MessageBox.Show("Favor llenar el campo de region (solamente numeros) ", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.Telefono_Codigo = Region;
                    }

                    if (string.IsNullOrEmpty(txt_TelefonoNumero.Text))
                    {
                        ClienteActual.Telefono_Numero = null;
                    }
                    else
                    {
                        if (!int.TryParse(txt_TelefonoNumero.Text, out int telefono))
                        {
                            MessageBox.Show("Favor llenar el campo de telefono (solamente numeros)", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.Telefono_Numero = telefono;
                    }
                    #endregion

                    #region Ubicacion
                    UbicacionesType provincia = cb_Provincia.SelectedItem as UbicacionesType;
                    if (provincia != null)
                    {
                        ClienteActual.Provincia = provincia.Id;
                        UbicacionesType canton = cb_canton.SelectedItem as UbicacionesType;
                        if (canton == null)
                        {
                            MessageBox.Show("Favor seleccionar una Canton", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.Canton = canton.Id;


                        UbicacionesType Distrito = cb_distrito.SelectedItem as UbicacionesType;
                        if (Distrito == null)
                        {
                            MessageBox.Show("Favor seleccionar una Distrito", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.Distrito = Distrito.Id;


                        UbicacionesType Barrio = cb_barrio.SelectedItem as UbicacionesType;
                        if (Distrito == null)
                        {
                            MessageBox.Show("Favor seleccionar una Barrio", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.Barrio = Barrio.Id;


                        if (string.IsNullOrEmpty(txt_otrasSenas.Text))
                        {
                            MessageBox.Show("Favor llenar el campo de otras senas", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ClienteActual.OtrasSenas = txt_otrasSenas.Text;
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(ClienteActual.Identificacion_Numero) && ClienteNuevo)
                    {
                        if (db.Cliente.Any(q => q.Identificacion_Numero == ClienteActual.Identificacion_Numero))
                        {
                            MessageBox.Show("Ya existe un cliente con ese numero de identificacion", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    else if (ClienteNuevo)
                    {
                        if (db.Cliente.Any(q => q.Nombre == ClienteActual.Nombre))
                        {
                            MessageBox.Show("Ya existe un cliente con ese nombre registrado", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }

                    if (ClienteNuevo)
                    {
                        db.Cliente.Add(ClienteActual);
                    }

                    db.SaveChanges();
                    Volver(sender, e);
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al guardar datos del cliente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Volver(object sender, RoutedEventArgs e)
        {
            try
            {
                RecursosSistema.MainConteiner.Content = new Lista_Clientes();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al volver a la lista de clientes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
