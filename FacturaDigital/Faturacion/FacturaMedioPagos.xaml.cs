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
using System.Windows.Shapes;

namespace FacturaDigital.Faturacion
{
    /// <summary>
    /// Lógica de interacción para FacturaMedioPagos.xaml
    /// </summary>
    public partial class FacturaMedioPagos : Window
    {
        public string CondicionVenta { set; get; }
        public string MedioPago { set; get; } 
        public FacturaMedioPagos(string Total)
        {
            InitializeComponent();
            decimal total = Convert.ToDecimal(Total);
            lb_Total.Text = total.ToString("₡0.##");
        }

        private void Aceptar(object sender, RoutedEventArgs e)
        {
            CondicionVenta = (cb_CondicionVenta.SelectedValue as ComboBoxItem).Tag.ToString();
            MedioPago = (cb_MedioPago.SelectedValue as ComboBoxItem).Tag.ToString();
            this.DialogResult = true;
            this.Close();
        }

        private void Cerrar(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
