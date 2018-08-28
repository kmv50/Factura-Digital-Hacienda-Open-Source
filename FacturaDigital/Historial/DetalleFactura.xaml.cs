using DataModel;
using DataModel.EF;
using FacturaDigital.FacturaPDF;
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

namespace FacturaDigital.Historial
{
    /// <summary>
    /// Lógica de interacción para DetalleFactura.xaml
    /// </summary>
    public partial class DetalleFactura : Window , ILog
    {
        private int IdFactura;
        private Factura fac;
        public DetalleFactura(int IdFactura)
        {
            InitializeComponent();
            this.IdFactura = IdFactura;
            RenderFactura();
        }
        

        private void RenderFactura() {
            try
            {
                fac = null;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    fac = db.Factura.Include("Factura_Detalle").FirstOrDefault(q => q.Id_Factura == IdFactura);
                }

                if(fac == null)
                {
                    MessageBox.Show("Factura no encontrada","Error", MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }




            }catch(Exception ex)
            {
                MessageBox.Show("Error al cargar los datos de factura","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                this.LogError(ex);
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MostrarPDF(object sender, RoutedEventArgs e)
        {
            try
            {
                FacturaElectronicaPDF pdf =  new FacturaElectronicaPDF();                
                pdf.MostrarFactura(pdf.CrearFactura(fac));
            }
            catch(Exception ex)
            {
                this.LogError(ex);
            }
        }

        private void EnviarEmail(object sender, RoutedEventArgs e)
        {
            try
            {
                bool EnviarEmail = true;
                if (string.IsNullOrEmpty(fac.XML_Respuesta))
                {
                    EnviarEmail = false;
                    if (MessageBox.Show("Actualmente falata la respuesta de Hacienda. Aun asi desea enviar el email?", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        EnviarEmail = true;

                }

                if(EnviarEmail)
                    new SendSmtp.SendSmtp(IdFactura).Enviar();
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
