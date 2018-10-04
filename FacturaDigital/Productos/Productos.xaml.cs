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
            }
            else
            {
                cb_Tipo.SelectedIndex = 1;
            }

            //ProductoActual.Unidad_Medida = ((UnidadMedida)cb_UnidadMedida.SelectedItem).Value;
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
