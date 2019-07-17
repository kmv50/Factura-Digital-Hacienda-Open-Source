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
using DataModel;
using DataModel.EF;

namespace FacturaDigital.Productos
{
    /// <summary>
    /// Interaction logic for ProductoImpuestoDialog.xaml
    /// </summary>
    public partial class ProductoImpuestoDialog : Window , ILog
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
            cb_codigotarifa.ItemsSource = ProductosData.TarifaCodigo;
            txt_monto.Text = "0";
            cb_TipoDocumento.ItemsSource = ProductosData.TiposDocumentosExoneracion;
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
                decimal montoImpuesto = ((Tarifa / (decimal)100) * precioUnitario);
                if (chk_Exoneracion.IsChecked.Value)
                {
                    decimal MontoAExonerar = ((Convert.ToDecimal(txt_procentajeExoneracion.Text) / (decimal)100) * montoImpuesto);
                    txt_ExoneracionTotal.Text = MontoAExonerar.ToString();
                    txt_monto.Text =  (montoImpuesto - MontoAExonerar).ToString();
                }
                else
                {
                    txt_monto.Text = montoImpuesto.ToString();
                }

            }
            catch(Exception ex)
            {
                this.LogError(ex);
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

            TiposDocumentosExoneracion Exoneracion_TipoDocumento = cb_TipoDocumento.SelectedItem as TiposDocumentosExoneracion;
            Producto_Impuesto = new Producto_ImpuestoSeleccionado()
            {
                Impuesto_Tarifa = Tarifa,
                Impuesto_Codigo = v1.Value,
                CodigoTarifa = (cb_codigotarifa.SelectedItem as TarifaCodigo).Value,
                Nombre = v1.Text,
                Monto = Convert.ToDecimal(txt_monto.Text),                
                Exento = chk_Exoneracion.IsChecked.Value,
                Exoneracion_FechaEmision = dt_FechaExoneracion.SelectedDate,
                Exoneracion_MontoImpuesto = string.IsNullOrEmpty(txt_ExoneracionTotal.Text) ? 0 : Convert.ToDecimal(txt_ExoneracionTotal.Text),
                Exoneracion_NombreInstitucion = txt_nombreinstitucion.Text,
                Exoneracion_NumeroDocumento = txt_numerodocumento.Text,
                Exoneracion_PorcentajeCompra = string.IsNullOrEmpty(txt_procentajeExoneracion.Text) ? 0 : Convert.ToInt32(txt_procentajeExoneracion.Text),
                Exoneracion_TipoDocumento = Exoneracion_TipoDocumento == null || Exoneracion_TipoDocumento.Value == null ? null : Exoneracion_TipoDocumento.Value,                
            };

            #region Validacion Exoneracion
            if (Producto_Impuesto.Exento) {
                if (string.IsNullOrEmpty(Producto_Impuesto.Exoneracion_NombreInstitucion)) {
                    MessageBox.Show("Favor llenar el nombre de la institucion");
                    return;
                }

                if (string.IsNullOrEmpty(Producto_Impuesto.Exoneracion_NumeroDocumento))
                {
                    MessageBox.Show("Favor llenar el numero del documento");
                    return;
                }

                if (!Producto_Impuesto.Exoneracion_FechaEmision.HasValue)
                {
                    MessageBox.Show("Favor seleccionar la fecha de emicion del documento");
                    return;
                }

                if (!Producto_Impuesto.Exoneracion_MontoImpuesto.HasValue || Producto_Impuesto.Exoneracion_MontoImpuesto.Value <= 0 ) {
                    MessageBox.Show("Favor verificar el monto a exonerar");
                    return;
                }
            }
            #endregion

            this.DialogResult = true;
            this.Close();
        }

        private void CambiarSelecionCodigoTarifa(object sender, SelectionChangedEventArgs e)
        {
            TarifaCodigo cod = cb_codigotarifa.SelectedItem as TarifaCodigo;
            if (cod == null)
                return;
            txt_Tarifa.Text =  cod.Tarifa.ToString();
            CalculeMonto(txt_Tarifa,null);
        }

        private void CalcularExoneracion(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt == null)
                {
                    return;
                }
                int v = 0;
                if (!int.TryParse(txt.Text, out v) || v < 0 || v >= 99)
                {
                    txt.Text = null;
                }
                CalculeMonto(txt_Tarifa, null);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al calcular el subtotal");
            }
        }

        void limpiarExoneracion() {
            cb_TipoDocumento.SelectedItem = 0;
            txt_numerodocumento.Text = null;
            txt_nombreinstitucion.Text = null;
            dt_FechaExoneracion.SelectedDate = null;
            txt_procentajeExoneracion.Text = null;
            txt_ExoneracionTotal.Text = null;
        }

        private void CambiarEstadoExoneracion(object sender, RoutedEventArgs e)
        {
            limpiarExoneracion();
            if (chk_Exoneracion.IsChecked.Value) 
                PanelDisableExoneracion.Visibility = Visibility.Collapsed;
            else
                PanelDisableExoneracion.Visibility = Visibility.Visible;
        }
    }
}
