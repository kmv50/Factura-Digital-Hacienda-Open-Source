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
using System.Windows.Shapes;
using DataModel.EF;

namespace FacturaDigital.Productos
{
    /// <summary>
    /// Interaction logic for ProductoImpuestoDialog.xaml
    /// </summary>
    public partial class ProductoImpuestoDialog : Window
    {
        private ObservableCollection<Producto_ImpuestoSeleccionado> coleccionImpuesto;
        private decimal precioUnitario;
        private decimal TarifaTotalActual;
        private Producto_ImpuestoSeleccionado Producto_Impuesto;
        public ProductoImpuestoDialog(ObservableCollection<Producto_ImpuestoSeleccionado> coleccionImpuesto , decimal precioUnitario)
        {
            this.precioUnitario = precioUnitario;
            this.coleccionImpuesto = coleccionImpuesto;
            List<Impuestos> ImpuestosDisponbles = ProductosData.Impuestos.Where(q => !coleccionImpuesto.Any( w => w.Impuesto_Codigo == q.Value)).ToList();            
            foreach (Producto_ImpuestoSeleccionado v in coleccionImpuesto)
            {
                    TarifaTotalActual += v.Impuesto_Tarifa;
            }
            
            InitializeComponent();

            if (ImpuestosDisponbles.Count == 0)
            {
                MessageBox.Show("Ya agrego todos los impuestos disponibles", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = false;
                this.Close();                
            }
            cb_impuestoTipo.ItemsSource = ImpuestosDisponbles;
            cb_impuestoTipo.SelectedIndex = 0;
            txt_monto.Text = "0";
        }

        internal Producto_ImpuestoSeleccionado GetImpuesto()
        {
            return Producto_Impuesto;
        }

        private void CalculeMonto(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox ui = (TextBox)sender;
                decimal Tarifa;
                if(!decimal.TryParse(ui.Text,out Tarifa))
                {
                    ui.Text = null;
                    return;
                }

                if(Tarifa + TarifaTotalActual >= 100)
                {
                    MessageBox.Show("Las tarifas agregadas suman un 100");
                    ui.Text = null;
                    return;
                }

                txt_monto.Text = ((Tarifa / 100) * precioUnitario).ToString();

            }
            catch(Exception ex)
            {
                Recursos.RecursosSistema.LogError(ex);
            }
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            decimal Tarifa;
            if (!decimal.TryParse(txt_Tarifa.Text, out Tarifa))
            {
                MessageBox.Show("Error al obtener la tarifa");
                return;
            }
            var v1 = ((Impuestos)cb_impuestoTipo.SelectedItem);
            Producto_Impuesto = new Producto_ImpuestoSeleccionado()
            {
                Impuesto_Tarifa = Tarifa,
                Impuesto_Codigo = v1.Value,
                Nombre = v1.Text,
                Monto = Convert.ToDecimal(txt_monto.Text)
            };
            
            this.DialogResult = true;
            this.Close();
        }
       
    }
}
