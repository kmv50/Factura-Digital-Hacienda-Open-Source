using DataModel;
using DataModel.EF;
using FacturaDigital.FacturaPDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FacturaDigital.Historial
{
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }

    }
    public class XmlHacienda
    {
        public string Tipo { set; get; }
        private string xmlUrl;
        public string XmlUrl
        {
            set
            {

                string path = Path.GetTempPath();
                string fileName = Guid.NewGuid().ToString() + ".xml";
                string fullFileName = Path.Combine(path, fileName);
                File.WriteAllText(fullFileName, value);
                xmlUrl = fullFileName;
            }
            get => xmlUrl;
        }
    }

    /// <summary>
    /// Lógica de interacción para DetalleFactura.xaml
    /// </summary>
    public partial class DetalleFactura : Window, ILog
    {
        private int IdFactura;
        private Factura fac;
        public DetalleFactura(int IdFactura)
        {
            InitializeComponent();
            this.IdFactura = IdFactura;
            RenderFactura();
        }


        private void RenderFactura()
        {
            try
            {
                fac = null;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    fac = db.Factura.Include("Factura_Detalle").FirstOrDefault(q => q.Id_Factura == IdFactura);
                }

                if (fac == null)
                {
                    MessageBox.Show("Factura no encontrada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                EstadoComprobante estado = (EstadoComprobante)fac.Estado;


                txt_CCorreo.Text = fac.Receptor_CorreoElectronico;
                txt_CIdentificacion.Text = fac.Receptor_Identificacion_Numero;
                txt_Ctelefono.Text = fac.Receptor_Telefono_Numero.ToString();
                txt_CNombre.Text = fac.Receptor_Nombre;

                Txt_Estado.Text = estado.GetAttribute<DescriptionAttribute>().Description;
                txt_Consecutivo.Text = fac.NumeroConsecutivo.ToString();
                txt_Fecha.Text = fac.Fecha_Emision_Documento.ToString();
                tb_Clave.Text = fac.Clave;

                dg_detalleFactura.ItemsSource = fac.Factura_Detalle;

                txt_SubTotal.Text = fac.TotalVentaNeta.ToString();
                txt_Descuento.Text = fac.TotalDescuentos.ToString();
                txt_Impuesto.Text = fac.TotalImpuesto.ToString();
                txt_Total.Text = fac.TotalComprobante.ToString();

                List<XmlHacienda> XmlHacienda = new List<XmlHacienda>();
                if (!string.IsNullOrEmpty(fac.XML_Enviado))
                    XmlHacienda.Add(new XmlHacienda()
                    {
                        Tipo = "Enviado",
                        XmlUrl = fac.XML_Enviado
                    });

                if (!string.IsNullOrEmpty(fac.XML_Respuesta))
                    XmlHacienda.Add(new XmlHacienda()
                    {
                        Tipo = "Respuesta",
                        XmlUrl = fac.XML_Respuesta
                    });

                lb_xmls.ItemsSource = XmlHacienda;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos de factura", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.LogError(ex);
            }
        }


        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MostrarPDF(object sender, RoutedEventArgs e)
        {
            try
            {
                FacturaElectronicaPDF pdf = new FacturaElectronicaPDF();
                pdf.MostrarFactura(pdf.CrearFactura(fac));
            }
            catch (Exception ex)
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
                    {
                        EnviarEmail = true;
                    }
                }

                if (EnviarEmail)
                {
                    new SendSmtp.SendSmtp(IdFactura).Enviar();
                    MessageBox.Show("Correo Renviado correctamente","Informacion",MessageBoxButton.OK,MessageBoxImage.Information);
                    Button btn = sender as Button;
                    if (btn != null)
                        btn.IsEnabled = false;
                }



              
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
