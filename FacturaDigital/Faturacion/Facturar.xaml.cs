using DataModel.EF;
using FacturaDigital.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacturaDigital.Faturacion
{
    /// <summary>
    /// Interaction logic for Facturar.xaml
    /// </summary>
    public partial class Facturar : Page
    {
        ObservableCollection<Factura_Detalle> FacturaDetalle;

        public Facturar()
        {
            InitializeComponent();
            FacturaDetalle = new ObservableCollection<Factura_Detalle>();
            dgv_DetalleFactura.ItemsSource = FacturaDetalle;
            LoadData();
        }

        void LoadData() {
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                cb_Clientes.ItemsSource =  db.Cliente.ToList();
                cb_Productos.ItemsSource = db.Producto.Include("Producto_Impuesto").ToList();
            }
        }

        private void AgregarItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Producto p = cb_Productos.SelectedItem as Producto;
                if (p == null)
                    return;

                Factura_Detalle item = new Factura_Detalle();

                #region Validar Datos 
                decimal PrecioUnitario;
                if(!decimal.TryParse(txt_precioUnitario.Text,out PrecioUnitario))
                {
                    MessageBox.Show("Error el precio unitario debe ser numerico","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                    return;
                }
                item.PrecioUnitario = PrecioUnitario;

                int Cantidad;
                if (!int.TryParse(txt_Cantidad.Text, out Cantidad))
                {
                    MessageBox.Show("Error la cantidad debe de ser de tipo entero", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if(Cantidad == 0)
                {
                    MessageBox.Show("La cantidad no puede ser 0","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                    return;
                }

                item.Cantidad = Cantidad;


                int ? DescuentoReal = null;
                if (!string.IsNullOrEmpty(txt_Descuento.Text))
                {
                    int Descuento;
                    if (!int.TryParse(txt_Descuento.Text, out Descuento) && Descuento <= 99 && Descuento > 0)
                    {
                        MessageBox.Show("Error el decuento debe ser un dato numerico positivo de tipo entero no mayor a 99 y mayor a 0", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                }

                item.Monto_Descuento = DescuentoReal;

                if (DescuentoReal.HasValue && string.IsNullOrEmpty(txt_NaturalezaDescuento.Text))
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
                        decimal SubImpuesto =  ((impuesto.Impuesto_Tarifa / 100) * item.Monto_Total);
                        Factura_Detalle_Impuesto.Add(new Factura_Detalle_Impuesto() {
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
                item.SubTotal = item.Monto_Total - item.Monto_Descuento ?? 0;
                item.Monto_Total_Linea = item.SubTotal + (item.Impuesto_Monto ?? 0);

                FacturaDetalle.Add(item);
                LimpiarSelectorProducto();
                CalcularTotales();
            }
            catch (Exception ex)
            {
                RecursosSistema.LogError(ex);
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

                decimal ResumenSubTotalNeto = 0
                    , ResumenImpuesto = 0
                    , ResumenDescuentos = 0
                    , ResumenTotales = 0

                    , ResumenImpuestoServicioGravado = 0
                    , ResumenImpuestoServicioExento = 0

                    , ResumenImpuestoProductoGravado = 0
                    , ResumenImpuestoProductoExento = 0;
                ;


                foreach(Factura_Detalle item in FacturaDetalle)
                {
                    if (item.Impuesto_Monto.HasValue)
                        ResumenImpuesto += item.Impuesto_Monto.Value;                    

                    if (item.Monto_Descuento.HasValue)
                        ResumenDescuentos += item.Monto_Descuento.Value;

                    ResumenSubTotalNeto += item.SubTotal;
                    ResumenTotales += item.Monto_Total_Linea;

                    if (item.Tipo) // tipo true => servicio
                    {
                        if (item.Gravado && item.Impuesto_Monto.HasValue)
                            ResumenImpuestoServicioGravado += item.Monto_Total_Linea;
                        else
                            ResumenImpuestoServicioExento += item.Monto_Total_Linea;
                    }
                    else
                    {
                        if (item.Gravado && item.Impuesto_Monto.HasValue)
                            ResumenImpuestoProductoGravado += item.Monto_Total_Linea;
                        else
                            ResumenImpuestoProductoExento += item.Monto_Total_Linea;
                    }
                }

                txt_ResumenDescuento.Text = ResumenDescuentos.ToString();
                txt_ResumenImpuesto.Text = ResumenImpuesto.ToString();
                txt_ResumenSubTotalNeto.Text = ResumenSubTotalNeto.ToString();
                txt_ResumenTotales.Text = ResumenTotales.ToString();

                txt_ImpuestoResumenProductoExento.Text = ResumenDescuentos.ToString();
                txt_ImpuestoResumenProductoGravado.Text = ResumenDescuentos.ToString();

                txt_ImpuestoResumenServiciosExento.Text = ResumenDescuentos.ToString();
                txt_ImpuestoResumenServiciosGravado.Text = ResumenDescuentos.ToString();
            }
            catch(Exception ex)
            {
                RecursosSistema.LogError(ex);
                MessageBox.Show("Error al calcular el sub total","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void LimpiarResumenTotales()
        {
            throw new NotImplementedException();
        }

        void LimpiarSelectorProducto() {
            try
            {
                cb_Productos.SelectedItem = null;
                txt_Cantidad.Text = null;
                txt_Descuento.Text = null;
                txt_NaturalezaDescuento.Text = null;
                txt_precioUnitario.Text = null;
                txt_subtotal.Text = null;               
            }catch(Exception ex)
            {
                RecursosSistema.LogError(ex);
            }
        }

        private void Select_Producto_Servicio(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               Producto p = cb_Productos.SelectedItem as Producto;
               if (p == null)
                   return;

                txt_precioUnitario.Text = p.PrecioUnitario.ToString();
                txt_Cantidad.Text = "1";
                if (p.ImpuestosTarifaTotal.HasValue)
                    txt_Impuesto.Text = p.ImpuestosTarifaTotal.ToString();
                else
                    txt_Impuesto.Text = (0).ToString();

                txt_Descuento.Text = "0";
                CalcularSubTotal();
            }
            catch(Exception ex)
            {
                RecursosSistema.LogError(ex);
                MessageBox.Show("Ocurrio un error al seleccionar el articulo");
            }
        }


        private void CalcularSubTotal() {
            decimal SubTotal = 0;

            decimal Unitario = Convert.ToDecimal(txt_precioUnitario.Text);
            int Cantidad = Convert.ToInt32(txt_Cantidad.Text);
            decimal Decuento = Convert.ToDecimal(txt_Descuento.Text);

            decimal Impuesto = 0;
            if (!string.IsNullOrEmpty(txt_Impuesto.Text))
                Impuesto = Convert.ToDecimal(txt_Impuesto.Text);

            SubTotal = Unitario * Cantidad;

            if(Decuento > 0)
                SubTotal = SubTotal - ((Decuento / 100) * SubTotal);

            if (Impuesto > 0)
                SubTotal = SubTotal  + ((Impuesto / 100) * SubTotal);

            txt_subtotal.Text = SubTotal.ToString();
        }

        private void keyUpAddProductoServicio(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt == null)
                    return;

                int v;
                if(!int.TryParse(txt.Text, out v))
                {
                    txt.Text = "0";
                }
                CalcularSubTotal();

                if(txt.Name == "txt_Descuento" && !string.IsNullOrEmpty(txt.Text))
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
                RecursosSistema.LogError(ex);
                MessageBox.Show("Ocurrio un error al calcular el subtotal");
            }

           
        }

        private void keyUpAddProductoServicioDecimal(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt == null)
                    return;

                decimal v;
                if (!decimal.TryParse(txt.Text, out v))
                {
                    txt.Text = "0";
                }
                CalcularSubTotal();
            }
            catch (Exception ex)
            {
                RecursosSistema.LogError(ex);
                MessageBox.Show("Ocurrio un error al calcular el subtotal");
            }
        }

        private void EliminarDeTabla(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null)
                    return;

                Factura_Detalle item = btn.CommandParameter as Factura_Detalle;
                FacturaDetalle.Remove(item);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ocurrio un error al eliminar el producto de la lista","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                RecursosSistema.LogError(ex);                
            }
        }

        private void CambiarTipoFactura(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tb = sender as ToggleButton;
                if (tb == null)
                    return;
                if (tb.IsChecked.Value)
                {
                    txt_Cliente_Tiquete.Visibility = Visibility.Visible;
                    cb_Clientes.Visibility = Visibility.Hidden;
                    lb_TipoFactura.Content = "Tiquete electronico";
                }
                else {
                    txt_Cliente_Tiquete.Visibility = Visibility.Hidden;
                    cb_Clientes.Visibility = Visibility.Visible;
                    lb_TipoFactura.Content = "Factura Electronica";
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al cambiar el tipo de factura","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                RecursosSistema.LogError(ex);
            }
        }
    }
}
