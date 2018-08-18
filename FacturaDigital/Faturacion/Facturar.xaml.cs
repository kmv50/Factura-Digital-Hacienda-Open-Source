using DataModel.EF;
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

namespace FacturaDigital.Faturacion
{
    /// <summary>
    /// Interaction logic for Facturar.xaml
    /// </summary>
    public partial class Facturar : Page
    {
        public Facturar()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData() {
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                cb_Clientes.ItemsSource =  db.Cliente.ToList();
                cb_Productos.ItemsSource = db.Producto.ToList();
            }
        }

        private void AgregarItem(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                RecursosSistema.LogError(ex);
                MessageBox.Show("Ocurrio un error al seleccionar el articulo");
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
    }
}
