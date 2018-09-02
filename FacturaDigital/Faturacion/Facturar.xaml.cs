using DataModel;
using DataModel.EF;
using DataModel.Hacienda_Comunication;
using FacturaDigital.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FacturaDigital.Faturacion
{
    /// <summary>
    /// Interaction logic for Facturar.xaml
    /// </summary>
    public partial class Facturar : Page, INotifyPropertyChanged , ILog
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
        #endregion

        #region CalculoTotales
        public decimal ResumenSubTotalNeto { set; get; }
        public decimal ResumenImpuesto { set; get; }
        public decimal ResumenDescuentos { set; get; }
        public decimal ResumenTotales { set; get; }

        public decimal ResumenServicioGravado { set; get; }
        public decimal ResumenServicioExento { set; get; }

        public decimal ResumenProductoGravado { set; get; }
        public decimal ResumenProductoExento { set; get; }
        #endregion
        private ObservableCollection<Factura_Detalle> FacturaDetalle;

        public Facturar()
        {
            InitializeComponent();
            FacturaDetalle = new ObservableCollection<Factura_Detalle>();
            dgv_DetalleFactura.ItemsSource = FacturaDetalle;
            LoadData();
        }

        private void LoadData()
        {
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                cb_Clientes.ItemsSource = db.Cliente.ToList();
                cb_Productos.ItemsSource = db.Producto.Include("Producto_Impuesto").ToList();
            }
        }

        private void AgregarItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Producto p = cb_Productos.SelectedItem as Producto;
                if (p == null)
                {
                    return;
                }

                Factura_Detalle item = new Factura_Detalle();

                #region Validar Datos 
                if (!decimal.TryParse(txt_precioUnitario.Text, out decimal PrecioUnitario) || PrecioUnitario <= 0)
                {
                    MessageBox.Show("Error el precio unitario debe ser numerico mayor a 0", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                item.PrecioUnitario = PrecioUnitario;

                if (!int.TryParse(txt_Cantidad.Text, out int Cantidad))
                {
                    MessageBox.Show("Error la cantidad debe de ser de tipo entero", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (Cantidad == 0)
                {
                    MessageBox.Show("La cantidad no puede ser 0", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                item.Cantidad = Cantidad;


                int? DescuentoRealPorcentaje = null;
                if (!string.IsNullOrEmpty(txt_Descuento.Text))
                {
                    if (!int.TryParse(txt_Descuento.Text, out int Descuento) && Descuento <= 99 && Descuento > 0)
                    {
                        MessageBox.Show("Error el decuento debe ser un dato numerico positivo de tipo entero no mayor a 99 y mayor a 0", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    if (Descuento != 0)
                    {
                        DescuentoRealPorcentaje = Descuento;
                    }
                }


                if (DescuentoRealPorcentaje.HasValue && string.IsNullOrEmpty(txt_NaturalezaDescuento.Text))
                {
                    MessageBox.Show("Error si ingresa un descuento por normativa de Hacienda debe de indicar la razon del mismo", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                else
                {
                    item.Naturaleza_Descuento = txt_NaturalezaDescuento.Text;
                }
                #endregion


                item.Monto_Total = item.Cantidad * item.PrecioUnitario;

                if (DescuentoRealPorcentaje.HasValue)
                {
                    item.Monto_Descuento = ((DescuentoRealPorcentaje / 100) * item.Monto_Total);
                }
                else
                {
                    item.Monto_Descuento = 0;
                }

                item.ProductoServicio = p.ProductoServicio;
                item.Unidad_Medida = p.Unidad_Medida;
                item.Tipo = p.Tipo;
                item.Codigo = p.Codigo;
                item.Gravado = false;

                if (p.Producto_Impuesto != null && p.Producto_Impuesto.Count > 0)
                {
                    decimal Impuesto_Monto = 0;
                    List<Factura_Detalle_Impuesto> Factura_Detalle_Impuesto = new List<Factura_Detalle_Impuesto>();
                    foreach (Producto_Impuesto impuesto in p.Producto_Impuesto)
                    {
                        decimal SubImpuesto = ((impuesto.Impuesto_Tarifa / 100) * item.Monto_Total);
                        Factura_Detalle_Impuesto.Add(new Factura_Detalle_Impuesto()
                        {
                            Exento = impuesto.Exento,
                            Exoneracion_FechaEmision = impuesto.Exoneracion_FechaEmision,
                            Exoneracion_MontoImpuesto = impuesto.Exoneracion_MontoImpuesto,
                            Exoneracion_NombreInstitucion = impuesto.Exoneracion_NombreInstitucion,
                            Exoneracion_NumeroDocumento = impuesto.Exoneracion_NumeroDocumento,
                            Exoneracion_PorcentajeCompra = impuesto.Exoneracion_PorcentajeCompra,
                            Exoneracion_TipoDocumento = impuesto.Exoneracion_TipoDocumento,
                            Impuesto_Codigo = impuesto.Impuesto_Codigo,
                            Impuesto_Tarifa = impuesto.Impuesto_Tarifa,
                            Impuesto_Monto = SubImpuesto
                        });
                        Impuesto_Monto += SubImpuesto;
                    }
                    item.Gravado = true;
                    item.Impuesto_Monto = Impuesto_Monto;
                    item.Factura_Detalle_Impuesto = Factura_Detalle_Impuesto;
                }
                item.SubTotal = item.Monto_Total - (item.Monto_Descuento.HasValue ? item.Monto_Descuento.Value : 0);
                item.Monto_Total_Linea = item.SubTotal + (item.Impuesto_Monto.HasValue ? item.Impuesto_Monto.Value : 0);

                FacturaDetalle.Add(item);
                LimpiarSelectorProducto();
                CalcularTotales();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al seleccionar el articulo");
            }
        }

        private void CalcularTotales()
        {
            try
            {
                if (FacturaDetalle.Count == 0)
                {
                    LimpiarResumenTotales();
                    return;
                }
                LimpiarResumenTotales(false);


                foreach (Factura_Detalle item in FacturaDetalle)
                {
                    if (item.Impuesto_Monto.HasValue)
                    {
                        ResumenImpuesto += item.Impuesto_Monto.Value;
                    }

                    if (item.Monto_Descuento.HasValue)
                    {
                        ResumenDescuentos += item.Monto_Descuento.Value;
                    }

                    ResumenSubTotalNeto += item.SubTotal;
                    ResumenTotales += item.Monto_Total_Linea;

                    if (item.Tipo) // tipo true => servicio
                    {
                        if (item.Gravado && item.Impuesto_Monto.HasValue)
                        {
                            ResumenServicioGravado += item.SubTotal;
                        }
                        else
                        {
                            ResumenServicioExento += item.SubTotal;
                        }
                    }
                    else
                    {
                        if (item.Gravado && item.Impuesto_Monto.HasValue)
                        {
                            ResumenProductoGravado += item.SubTotal;
                        }
                        else
                        {
                            ResumenProductoExento += item.SubTotal;
                        }
                    }
                }



                NotificarCambioTotales();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al calcular el sub total", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void NotificarCambioTotales()
        {
            OnPropertyChanged("ResumenSubTotalNeto");
            OnPropertyChanged("ResumenImpuesto");
            OnPropertyChanged("ResumenDescuentos");
            OnPropertyChanged("ResumenTotales");
            OnPropertyChanged("ResumenServicioGravado");
            OnPropertyChanged("ResumenServicioExento");
            OnPropertyChanged("ResumenProductoGravado");
            OnPropertyChanged("ResumenProductoExento");
        }

        private void LimpiarResumenTotales(bool Notificar = true)
        {
            ResumenSubTotalNeto = 0;
            ResumenImpuesto = 0;
            ResumenDescuentos = 0;
            ResumenTotales = 0;

            ResumenServicioGravado = 0;
            ResumenServicioExento = 0;

            ResumenProductoGravado = 0;
            ResumenProductoExento = 0;
            if(Notificar)
                NotificarCambioTotales();
        }

        private void LimpiarSelectorProducto()
        {
            try
            {
                cb_Productos.SelectedItem = null;
                txt_Cantidad.Text = null;
                txt_Descuento.Text = null;
                txt_NaturalezaDescuento.Text = null;
                txt_precioUnitario.Text = null;
                txt_subtotal.Text = null;
                txt_NaturalezaDescuento.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        private void Select_Producto_Servicio(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Producto p = cb_Productos.SelectedItem as Producto;
                if (p == null)
                {
                    return;
                }

                txt_precioUnitario.Text = p.PrecioUnitario.ToString();
                txt_Cantidad.Text = "1";
                if (p.ImpuestosTarifaTotal.HasValue)
                {
                    txt_Impuesto.Text = p.ImpuestosTarifaTotal.ToString();
                }
                else
                {
                    txt_Impuesto.Text = (0).ToString();
                }

                txt_Descuento.Text = "0";
                CalcularSubTotal();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al seleccionar el articulo");
            }
        }


        private void CalcularSubTotal()
        {
            decimal SubTotal = 0;

            decimal Unitario = Convert.ToDecimal(txt_precioUnitario.Text);
            int Cantidad = Convert.ToInt32(txt_Cantidad.Text);

            decimal Decuento = 0;
            if (!string.IsNullOrEmpty(txt_Descuento.Text))
            {
                Decuento = Convert.ToDecimal(txt_Descuento.Text);
            }

            decimal Impuesto = 0;
            if (!string.IsNullOrEmpty(txt_Impuesto.Text))
            {
                Impuesto = Convert.ToDecimal(txt_Impuesto.Text);
            }

            SubTotal = Unitario * Cantidad;

            if (Decuento > 0)
            {
                SubTotal = SubTotal - ((Decuento / 100) * SubTotal);
            }

            if (Impuesto > 0)
            {
                SubTotal = SubTotal + ((Impuesto / 100) * SubTotal);
            }

            txt_subtotal.Text = SubTotal.ToString();
        }

        private void keyUpAddProductoServicio(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt == null)
                {
                    return;
                }

                if (!int.TryParse(txt.Text, out int v))
                {
                    if (txt.Name == "txt_Descuento")
                    {
                        txt.Text = null;
                    }
                    else if (txt.Name == "txt_Cantidad")
                    {
                        txt.Text = "1";
                    }
                }
                CalcularSubTotal();

                if (txt.Name == "txt_Descuento" && !string.IsNullOrEmpty(txt.Text))
                {
                    txt_NaturalezaDescuento.Visibility = Visibility.Visible;
                }
                else
                {
                    txt_NaturalezaDescuento.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al calcular el subtotal");
            }


        }

        private void keyUpAddProductoServicioDecimal(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt == null)
                {
                    return;
                }

                if (!decimal.TryParse(txt.Text, out decimal v))
                {
                    txt.Text = null;
                }
                CalcularSubTotal();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al calcular el subtotal");
            }
        }

        private void EliminarDeTabla(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null)
                {
                    return;
                }

                Factura_Detalle item = btn.CommandParameter as Factura_Detalle;
                FacturaDetalle.Remove(item);
                CalcularTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al eliminar el producto de la lista", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.LogError(ex);
            }
        }

        private void CambiarTipoFactura(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tb = sender as ToggleButton;
                if (tb == null)
                {
                    return;
                }

                if (tb.IsChecked.Value)
                {
                    txt_Cliente_Tiquete.Visibility = Visibility.Visible;
                    cb_Clientes.Visibility = Visibility.Hidden;
                    lb_TipoFactura.Content = "Tiquete electronico";
                }
                else
                {
                    txt_Cliente_Tiquete.Visibility = Visibility.Hidden;
                    cb_Clientes.Visibility = Visibility.Visible;
                    lb_TipoFactura.Content = "Factura Electronica";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar el tipo de factura", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.LogError(ex);
            }
        }

        private void ModalFacturacion(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FacturaDetalle.Count == 0 || ResumenTotales == 0)
                {
                    MessageBox.Show("Por favor agregue almenops un producto / servicio a la factura", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }


                loadingDisplayer.Visibility = Visibility.Visible;
                FacturaMedioPagos dia = new FacturaMedioPagos(ResumenTotales)
                {
                    Owner = Window.GetWindow(this)
                };
                if (dia.ShowDialog() == true)
                {
                    CrearFactura(dia.CondicionVenta, dia.MedioPago);
                }
                else
                {
                    loadingDisplayer.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al facturar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CrearFactura(string CondicionVenta, string MedioPago)
        {
            try
            {
                DateTime FechaEmicionDocumento = DateTime.Now;
                int casaMatriz = 1;
                int PuntoVenta = 1;
                Tipo_documento tipoDocumento = Tipo_documento.Factura_electrónica;
                if (tb_TipoFactura.IsChecked.Value)
                {
                    tipoDocumento = Tipo_documento.Tiquete_Electrónico;
                }

                #region HeaderFactura
                Factura fac = new Factura()
                {
                    Codigo_Moneda = "CRC",
                    CondicionVenta = CondicionVenta,
                    Email_Enviado = false,
                    CasaMatriz = casaMatriz,
                    PuntoVenta = PuntoVenta,
                    Emisor_CorreoElectronico = RecursosSistema.Contribuyente.CorreoElectronico,
                    Emisor_Identificacion_Numero = RecursosSistema.Contribuyente.Identificacion_Numero,
                    Emisor_Identificacion_Tipo = RecursosSistema.Contribuyente.Identificacion_Tipo,
                    Emisor_Nombre = RecursosSistema.Contribuyente.Nombre,
                    Emisor_NombreComercial = RecursosSistema.Contribuyente.NombreComercial,
                    Emisor_Telefono_Codigo = RecursosSistema.Contribuyente.Telefono_Codigo,
                    Emisor_Telefono_Numero = RecursosSistema.Contribuyente.Telefono_Numero,
                    Emisor_Ubicacion_Barrio = RecursosSistema.Contribuyente.Barrio,
                    Emisor_Ubicacion_Canton = RecursosSistema.Contribuyente.Canton,
                    Emisor_Ubicacion_Distrito = RecursosSistema.Contribuyente.Distrito,
                    Emisor_Ubicacion_Provincia = RecursosSistema.Contribuyente.Provincia,
                    Emisor_Ubicacion_OtrasSenas = RecursosSistema.Contribuyente.OtrasSenas,
                    Fecha_Emision_Documento = FechaEmicionDocumento,
                    Estado = (int)EstadoComprobante.Enviado,
                    Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente,
                    Id_TipoDocumento = (int)tipoDocumento,
                    MedioPago = MedioPago,
                };
                #endregion

                #region Totales
                if (ResumenProductoExento != 0)
                {
                    fac.TotalMercanciasExentas = ResumenProductoExento;
                }

                if (ResumenProductoGravado != 0)
                {
                    fac.TotalMercanciasGravadas = ResumenProductoGravado;
                }

                if (ResumenServicioExento != 0)
                {
                    fac.TotalServExentos = ResumenServicioExento;
                }

                if (ResumenServicioGravado != 0)
                {
                    fac.TotalServGravados = ResumenServicioGravado;
                }

                if (ResumenImpuesto != 0)
                {
                    fac.TotalImpuesto = ResumenImpuesto;
                }

                if (ResumenDescuentos != 0)
                {
                    fac.TotalDescuentos = ResumenDescuentos;
                }

                fac.TotalGravado = (fac.TotalMercanciasGravadas ?? 0) + (fac.TotalServGravados ?? 0);
                fac.TotalExento = (fac.TotalMercanciasExentas ?? 0) + (fac.TotalServExentos ?? 0);
                fac.TotalVenta = (fac.TotalGravado ?? 0) + (fac.TotalExento ?? 0);

                fac.TotalVentaNeta = fac.TotalVenta - (fac.TotalDescuentos ?? 0);
                fac.TotalComprobante = fac.TotalVentaNeta + (fac.TotalImpuesto ?? 0);

                #endregion



                if (tipoDocumento == Tipo_documento.Factura_electrónica)
                {
                    Cliente ClienteSeleccionado = cb_Clientes.SelectedItem as Cliente;
                    if (ClienteSeleccionado == null)
                    {
                        loadingDisplayer.Visibility = Visibility.Collapsed;
                        MessageBox.Show("Error al obtener datos del cliente seleccionado", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    fac.Receptor_CorreoElectronico = ClienteSeleccionado.CorreoElectronico;
                    fac.Receptor_Identificacion_Numero = ClienteSeleccionado.Identificacion_Numero;
                    fac.Receptor_Identificacion_Tipo = ClienteSeleccionado.Identificacion_Tipo;
                    fac.Receptor_Nombre = ClienteSeleccionado.Nombre;
                    fac.Receptor_NombreComercial = ClienteSeleccionado.NombreComercial;
                    fac.Receptor_Telefono_Codigo = ClienteSeleccionado.Telefono_Codigo;
                    fac.Receptor_Telefono_Numero = ClienteSeleccionado.Telefono_Numero;
                    fac.Receptor_Ubicacion_Barrio = ClienteSeleccionado.Barrio;
                    fac.Receptor_Ubicacion_Canton = ClienteSeleccionado.Canton;
                    fac.Receptor_Ubicacion_Distrito = ClienteSeleccionado.Distrito;
                    fac.Receptor_Ubicacion_OtrasSenas = ClienteSeleccionado.OtrasSenas;
                    fac.Receptor_Ubicacion_Provincia = ClienteSeleccionado.Provincia;
                }

                if (tipoDocumento == Tipo_documento.Tiquete_Electrónico)
                {
                    string clienteTiquete;
                    if (string.IsNullOrEmpty(txt_Cliente_Tiquete.Text))
                    {
                        clienteTiquete = "Cliente Contado";
                    }
                    else
                    {
                        clienteTiquete = txt_Cliente_Tiquete.Text;
                    }

                    fac.Receptor_Nombre = clienteTiquete;
                }

                fac.Factura_Detalle = new List<Factura_Detalle>(FacturaDetalle);


                Contribuyente_Consecutivos Consecutivo;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Consecutivo = db.Contribuyente_Consecutivos.First(q => q.Id_Contribuyente == RecursosSistema.Contribuyente.Id_Contribuyente);
                    fac.NumeroConsecutivo = Consecutivo.Consecutivo_Facturas;

                    string ClaveHacienda = new GeneradorDeClavesHacienda(new GeneradorDeClavesHacienda()
                    {
                        ConsecutivoHacienda = new ConsecutivoHacienda(new ConsecutivoHacienda()
                        {
                            TipoDocumento = Tipo_documento.Factura_electrónica,
                            NumeracionConsecutiva = Consecutivo.Consecutivo_Facturas,
                            CasaMatriz = casaMatriz,
                            PuntoVenta = PuntoVenta
                        }),
                        FechaEmicion = FechaEmicionDocumento,
                        Identificacion_Contribuyente = Convert.ToInt32(RecursosSistema.Contribuyente.Identificacion_Numero),
                    }).ToString();

                    fac.Clave = ClaveHacienda;
                    db.Factura.Add(fac);
                    Consecutivo.Consecutivo_Facturas++;

                    try
                    {
                        if (tipoDocumento == Tipo_documento.Factura_electrónica)
                        {
                            FacturaDB_ToFacturaElectronica Hacienda = new FacturaDB_ToFacturaElectronica(RecursosSistema.Contribuyente);
                            Hacienda.Convertir(fac).CrearXml(tipoDocumento).Enviar();
                            fac.XML_Enviado = Hacienda.XML.InnerXml;
                            new FacturaPDF.FacturaElectronicaPDF().CrearFactura(fac);
                        }
                        else
                        {

                        }
                        db.SaveChanges();
                        LimpiarVista();
                        RecursosSistema.WindosNotification("Factura", "La factura Clave [" + fac.Clave + "] se envío para su valoración");
                        RecursosSistema.Servicio_AgregarFactura(fac.Clave);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                    loadingDisplayer.Visibility = Visibility.Collapsed;
                    
                }

            }
            catch (Exception ex)
            {
                loadingDisplayer.Visibility = Visibility.Collapsed;
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al crear la factura en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void LimpiarVista()
        {
            try
            {
                LimpiarResumenTotales();
                LimpiarSelectorProducto();
                FacturaDetalle.Clear();
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al limpiara la pagina","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
