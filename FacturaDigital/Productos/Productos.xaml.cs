using DataModel;
using DataModel.EF;
using FacturaDigital.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FacturaDigital.Productos
{
    /// <summary>
    /// Interaction logic for Productos.xaml
    /// </summary>
    public partial class Productos : Page, ILog
    {
        private ObservableCollection<Producto_ImpuestoSeleccionado> ColeccionImpuesto;
        private Producto ProductoActual;
        public Productos()
        {
            ProductoActual = null;
            StarViewProductos();
        }

        public Productos(int idProducto)
        {
            StarViewProductos();
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                ProductoActual = db.Producto.Include("Producto_Impuesto").FirstOrDefault(q => q.Id_Producto == idProducto);
            }

            if (ProductoActual == null)
            {
                return;
            }

            loadProducto();
        }

        private void loadProducto()
        {

            txt_Codigo.Text = ProductoActual.Codigo;
            txt_precioUnitario.Text = ProductoActual.PrecioUnitario.ToString();
            txt_Producto.Text = ProductoActual.ProductoServicio;

            if (ProductoActual.Tipo)
            {
                cb_Tipo.SelectedIndex = 0;
                cb_UnidadMedida.SelectedItem = ProductosData.UnidadesMedida.Where(q => !q.EsServicio);
            }
            else
            {
                cb_Tipo.SelectedIndex = 1;
                cb_UnidadMedida.SelectedItem = ProductosData.UnidadesMedida.Where(q => q.EsServicio);
            }
            cb_UnidadMedida.SelectedItem = ProductosData.UnidadesMedida.FirstOrDefault(q => q.Value == ProductoActual.Unidad_Medida);

            foreach (Producto_Impuesto impuesto in ProductoActual.Producto_Impuesto) {
                Producto_ImpuestoSeleccionado val = new Producto_ImpuestoSeleccionado() {
                    CodigoTarifa = impuesto.CodigoTarifa,
                    Exento = impuesto.Exento,
                    Exoneracion_FechaEmision = impuesto.Exoneracion_FechaEmision,
                    Exoneracion_MontoImpuesto = impuesto.Exoneracion_MontoImpuesto,
                    Impuesto_Tarifa = impuesto.Impuesto_Tarifa,
                    Exoneracion_NombreInstitucion = impuesto.Exoneracion_NombreInstitucion,
                    Exoneracion_NumeroDocumento = impuesto.Exoneracion_NumeroDocumento,
                    Exoneracion_PorcentajeCompra = impuesto.Exoneracion_PorcentajeCompra,
                    Exoneracion_TipoDocumento = impuesto.Exoneracion_TipoDocumento,
                    Impuesto_Codigo = impuesto.Impuesto_Codigo,
                    Id_Producto = impuesto.Id_Producto,
                    Id_Producto_Impuesto = impuesto.Id_Producto_Impuesto,                    
                };
                Impuestos im = ProductosData.Impuestos.FirstOrDefault(q => q.Value == impuesto.Impuesto_Codigo);
                if (im == null)
                    val.Nombre = "Impuesto";
                else
                    val.Nombre = im.Text;

                val.Monto = ((val.Impuesto_Tarifa / (decimal)100) * ProductoActual.PrecioUnitario);
                if (val.Exento)
                    val.Monto -= val.Exoneracion_MontoImpuesto ?? 0;

                ColeccionImpuesto.Add(val);
            }
            
        }

        private void StarViewProductos()
        {
            InitializeComponent();
            ColeccionImpuesto = new ObservableCollection<Producto_ImpuestoSeleccionado>();
            dgv_Impuestos.ItemsSource = ColeccionImpuesto;
            cb_UnidadMedida.ItemsSource = ProductosData.UnidadesMedida;
            try
            {
                cb_UnidadMedida.SelectedItem = ((List<UnidadMedida>)cb_UnidadMedida.ItemsSource).First(q => q.Value == "Sp");
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {

        }

        private void VolverListaProductos(object sender, RoutedEventArgs e)
        {
            RecursosSistema.MainConteiner.Content = new Lista_Productos();
        }

        private void AgregarProducto(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txt_precioUnitario.Text, out decimal precioUnitario))
                {
                    MessageBox.Show("El precio unitario debe de estar en un formato decimal", "Error validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    bool ProductoNuevo = false;
                    if (ProductoActual == null)
                    {
                        ProductoActual = new Producto();
                        ProductoNuevo = true;
                    }
                    else
                    {
                        db.Producto.Attach(ProductoActual);
                    }

                    ProductoActual.Codigo = txt_Codigo.Text;
                    ProductoActual.PrecioUnitario = precioUnitario;
                    ProductoActual.ProductoServicio = txt_Producto.Text;
                    ProductoActual.Tipo = cb_Tipo.SelectedIndex == 0 ? true : false;
                    ProductoActual.Unidad_Medida = ((UnidadMedida)cb_UnidadMedida.SelectedItem).Value;
                    ProductoActual.Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente;


                    List<Producto_Impuesto> Impuestos = new List<Producto_Impuesto>();

                    decimal TarifaTotal = 0;
                    if (ColeccionImpuesto.Count > 0)
                    {
                        foreach (Producto_ImpuestoSeleccionado s in ColeccionImpuesto)
                        {
                            TarifaTotal += s.Impuesto_Tarifa;
                            Impuestos.Add(new Producto_Impuesto()
                            {
                                Impuesto_Tarifa = s.Impuesto_Tarifa,
                                Impuesto_Codigo = s.Impuesto_Codigo,
                                CodigoTarifa = s.CodigoTarifa,
                                Exento = s.Exento,
                                Exoneracion_FechaEmision = s.Exoneracion_FechaEmision,
                                Exoneracion_MontoImpuesto = s.Exoneracion_MontoImpuesto,
                                Exoneracion_NombreInstitucion = s.Exoneracion_NombreInstitucion,
                                Exoneracion_NumeroDocumento = s.Exoneracion_NumeroDocumento,
                                Exoneracion_PorcentajeCompra = s.Exoneracion_PorcentajeCompra,
                                Exoneracion_TipoDocumento = s.Exoneracion_TipoDocumento,                                
                            });

                        }
                    }
                    ProductoActual.Producto_Impuesto = Impuestos;
                    ProductoActual.ImpuestosTarifaTotal = TarifaTotal;


                    if (ProductoNuevo)
                    {
                        db.Producto.Add(ProductoActual);
                    }    
                    db.SaveChanges();
                }

                VolverListaProductos(sender, e);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al guardar el producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AgregarImpuesto(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_precioUnitario.Text) || !decimal.TryParse(txt_precioUnitario.Text, out decimal PrecioUnitario))
                {
                    MessageBox.Show("Agregue el precio unitario antes de continuar");
                    return;
                }


                ProductoImpuestoDialog dia = new ProductoImpuestoDialog(ColeccionImpuesto, PrecioUnitario);
                if (dia.ShowDialog() == true)
                {
                    Producto_ImpuestoSeleccionado im = dia.GetImpuesto();
                    if (im.Monto > 0)
                    {
                        ColeccionImpuesto.Add(im);
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }
    }
}
