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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacturaDigital.Productos
{
    /// <summary>
    /// Interaction logic for Productos.xaml
    /// </summary>
    public partial class Productos : Page
    {
        ObservableCollection<Producto_ImpuestoSeleccionado> ColeccionImpuesto;
        public Productos()
        {
            InitializeComponent();
            ColeccionImpuesto = new ObservableCollection<Producto_ImpuestoSeleccionado>();
            cb_UnidadMedida.ItemsSource = ProductosData.UnidadesMedida;
            dgv_Impuestos.ItemsSource = ColeccionImpuesto;
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
                decimal precioUnitario;
                if(!decimal.TryParse(txt_precioUnitario.Text,out precioUnitario))
                {
                    MessageBox.Show("El precio unitario debe de estar en un formato decimal", "Error validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                

                Producto nuevoProducto = new Producto()
                {                    
                    Codigo = txt_Codigo.Text,
                    PrecioUnitario = precioUnitario,
                    ProductoServicio = txt_Producto.Text,
                    Tipo = cb_Tipo.SelectedIndex == 0 ? true : false,
                    Unidad_Medida = ((UnidadMedida)cb_UnidadMedida.SelectedItem).Value,
                    Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente,                    
                };

                decimal TarifaTotal = 0;
                if (ColeccionImpuesto.Count > 0)
                {
                    foreach (Producto_ImpuestoSeleccionado s in ColeccionImpuesto)
                    {
                        if (s.Impuesto_Tarifa.HasValue)
                        {
                            TarifaTotal += s.Impuesto_Tarifa.Value;
                            nuevoProducto.Producto_Impuesto.Add(new Producto_Impuesto()
                            {
                                Impuesto_Tarifa = s.Impuesto_Tarifa,
                                Impuesto_Tipo = s.Impuesto_Tipo,
                            });
                        }
                    }
                }
                nuevoProducto.ImpuestosTarifaTotal = TarifaTotal;

                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    db.Producto.Add(nuevoProducto);
                    db.SaveChanges();
                }

                VolverListaProductos(sender,e);
            }
            catch (Exception ex)
            {
                RecursosSistema.LogError(ex);
                MessageBox.Show("Ocurrio un error al guardar el producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AgregarImpuesto(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal PrecioUnitario;
                if(string.IsNullOrEmpty(txt_precioUnitario.Text) ||  !decimal.TryParse(txt_precioUnitario.Text, out PrecioUnitario))
                {
                    MessageBox.Show("Agregue el precio unitario antes de continuar");
                    return;
                }


                ProductoImpuestoDialog dia = new ProductoImpuestoDialog(ColeccionImpuesto, PrecioUnitario);
                if (dia.ShowDialog() == true)
                {
                    Producto_ImpuestoSeleccionado im = dia.GetImpuesto();
                    if(im.Monto > 0)
                        ColeccionImpuesto.Add(im);
                }
            }catch(Exception ex)
            {
                RecursosSistema.LogError(ex);
            }
        }
    }
}
