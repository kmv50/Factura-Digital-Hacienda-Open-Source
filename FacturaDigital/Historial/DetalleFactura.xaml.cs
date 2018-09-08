using DataModel;
using DataModel.EF;
using DataModel.Hacienda_Comunication;
using FacturaDigital.FacturaPDF;
using FacturaDigital.Recursos;
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
                    fac = db.Factura.AsNoTracking().Include("Factura_Detalle").FirstOrDefault(q => q.Id_Factura == IdFactura);
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

                if(estado == EstadoComprobante.Anulando || estado == EstadoComprobante.ErrorAnulando)
                {
                    using (db_FacturaDigital db = new db_FacturaDigital())
                    {
                        Factura Anulada = db.Factura.AsNoTracking().FirstOrDefault(q => q.InformacionReferencia_IdFactura == fac.Id_Factura);
                        if(Anulada != null)
                        {
                            txt_MotivoAnulacion.Text = Anulada.InformacionReferencia_Razon;
                            btn_Anular.IsEnabled = false;
                            txt_MotivoAnulacion.IsReadOnly = true;

                            if (!string.IsNullOrEmpty(Anulada.XML_Enviado))
                                XmlHacienda.Add(new XmlHacienda()
                                {
                                    Tipo = "Anulacion Enviada",
                                    XmlUrl = Anulada.XML_Enviado
                                });

                            if (!string.IsNullOrEmpty(Anulada.XML_Respuesta))
                                XmlHacienda.Add(new XmlHacienda()
                                {
                                    Tipo = "Anulacion Respuesta",
                                    XmlUrl = Anulada.XML_Respuesta
                                });

                            txt_ConsecutivoNotaCredito.Text = Anulada.Clave;
                            txt_FechaNotaCredito.Text = Anulada.Fecha_Emision_Documento.ToString();
                            DetalleNomtaCreditoPanel.Visibility = Visibility.Visible;
                        }
                    }
                }

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

        private void AnularFactura(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(txt_MotivoAnulacion.Text))
                {
                    MessageBox.Show("Favor ingresar el motivo de la aunlacion antes de continuar", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (MessageBox.Show("Esta seguro de anular esta factura?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                int casaMatriz = 1;
                int PuntoVenta = 1;
                DateTime FechaEmicionDocumento = DateTime.Now;


                Factura Anulacion = new Factura()
                {
                    Codigo_Moneda = "CRC",
                    CondicionVenta = fac.CondicionVenta,
                    Email_Enviado = false,
                    CasaMatriz = casaMatriz,
                    PuntoVenta = PuntoVenta,
                    Emisor_CorreoElectronico = fac.Emisor_CorreoElectronico,
                    Emisor_Identificacion_Numero = fac.Emisor_Identificacion_Numero,
                    Emisor_Identificacion_Tipo = fac.Emisor_Identificacion_Tipo,
                    Emisor_Nombre = fac.Emisor_Nombre,
                    Emisor_NombreComercial = fac.Emisor_NombreComercial,
                    Emisor_Telefono_Codigo = fac.Emisor_Telefono_Codigo,
                    Emisor_Telefono_Numero = fac.Emisor_Telefono_Numero,
                    Emisor_Ubicacion_Barrio = fac.Emisor_Ubicacion_Barrio,
                    Emisor_Ubicacion_Canton = fac.Emisor_Ubicacion_Canton,
                    Emisor_Ubicacion_Distrito = fac.Emisor_Ubicacion_Distrito,
                    Emisor_Ubicacion_Provincia = fac.Emisor_Ubicacion_Provincia,
                    Emisor_Ubicacion_OtrasSenas = fac.Emisor_Ubicacion_OtrasSenas,
                    Fecha_Emision_Documento = FechaEmicionDocumento,
                    Estado = (int)EstadoComprobante.Enviado,
                    Id_Contribuyente = fac.Id_Contribuyente,
                    Id_TipoDocumento = (int)Tipo_documento.Nota_de_crédito_electrónica,
                    MedioPago = fac.MedioPago,
                    Receptor_CorreoElectronico = fac.Receptor_CorreoElectronico,
                    Receptor_Identificacion_Numero = fac.Receptor_Identificacion_Numero,
                    Receptor_Identificacion_Tipo = fac.Receptor_Identificacion_Tipo,
                    Receptor_Nombre = fac.Receptor_Nombre,
                    Receptor_NombreComercial = fac.Receptor_NombreComercial,
                    Receptor_Telefono_Codigo = fac.Receptor_Telefono_Codigo,
                    Receptor_Telefono_Numero = fac.Receptor_Telefono_Numero,
                    Receptor_Ubicacion_Barrio = fac.Receptor_Ubicacion_Barrio,
                    Receptor_Ubicacion_Canton = fac.Receptor_Ubicacion_Canton,
                    Receptor_Ubicacion_Distrito = fac.Receptor_Ubicacion_Distrito,
                    Receptor_Ubicacion_OtrasSenas = fac.Receptor_Ubicacion_OtrasSenas,
                    Receptor_Ubicacion_Provincia = fac.Receptor_Ubicacion_Provincia,

                    TotalMercanciasExentas = fac.TotalMercanciasExentas,
                    TotalMercanciasGravadas = fac.TotalMercanciasGravadas,
                    TotalServExentos = fac.TotalServExentos,
                    TotalServGravados = fac.TotalServGravados,
                    TotalImpuesto = fac.TotalImpuesto,
                    TotalDescuentos = fac.TotalDescuentos,
                    TotalGravado = fac.TotalGravado,
                    TotalExento = fac.TotalExento,
                    TotalVenta = fac.TotalVenta,
                    TotalVentaNeta = fac.TotalVentaNeta,
                    TotalComprobante = fac.TotalComprobante,

                    InformacionReferencia_IdFactura = fac.Id_Factura, 
                    InformacionReferencia_Codigo = 1,
                    InformacionReferencia_FechaEmision = FechaEmicionDocumento,
                    InformacionReferencia_Numero = fac.Clave,
                    InformacionReferencia_Razon = txt_MotivoAnulacion.Text
                };
              

                Contribuyente_Consecutivos Consecutivo;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    List<Factura_Detalle> detalle = new List<Factura_Detalle>();
                    foreach (Factura_Detalle item in fac.Factura_Detalle)
                    {
                        Factura_Detalle newItem = (Factura_Detalle)item.Clone();
                        newItem.Factura = null;
                        List<Factura_Detalle_Impuesto> newimpuestos = new List<Factura_Detalle_Impuesto>();
                        foreach(Factura_Detalle_Impuesto detalleimpuesto in db.Factura_Detalle_Impuesto.AsNoTracking().Where(q => q.Id_Factura_Detalle == newItem.Id_Factura_Detalle))
                        {
                            Factura_Detalle_Impuesto newDetalleimpuesto = (Factura_Detalle_Impuesto)detalleimpuesto.Clone();
                            newDetalleimpuesto.Factura_Detalle = null;
                            newimpuestos.Add(newDetalleimpuesto);
                        }

                        if (newimpuestos.Count > 0)
                            newItem.Factura_Detalle_Impuesto = newimpuestos;

                        detalle.Add(newItem);
                    }

                    Anulacion.Factura_Detalle = detalle;

                    Consecutivo = db.Contribuyente_Consecutivos.First(q => q.Id_Contribuyente == RecursosSistema.Contribuyente.Id_Contribuyente);
                    Anulacion.NumeroConsecutivo = Consecutivo.Consecutivo_NotasCredito;

                    string ClaveHacienda = new GeneradorDeClavesHacienda(new GeneradorDeClavesHacienda()
                    {
                        ConsecutivoHacienda = new ConsecutivoHacienda(new ConsecutivoHacienda()
                        {
                            TipoDocumento = Tipo_documento.Nota_de_crédito_electrónica,
                            NumeracionConsecutiva = Consecutivo.Consecutivo_NotasCredito,
                            CasaMatriz = casaMatriz,
                            PuntoVenta = PuntoVenta
                        }),
                        FechaEmicion = FechaEmicionDocumento,
                        Identificacion_Contribuyente = Convert.ToInt64(RecursosSistema.Contribuyente.Identificacion_Numero),
                    }).ToString();

                    Anulacion.Clave = ClaveHacienda;
                    db.Factura.Add(Anulacion);
                    Consecutivo.Consecutivo_NotasCredito++;

                    Factura Original = db.Factura.First(q => q.Id_Factura == fac.Id_Factura);
                    Original.Estado = (int)EstadoComprobante.Anulando;


                    try
                    {
                            FacturaDB_ToNotaCredito Hacienda = new FacturaDB_ToNotaCredito(RecursosSistema.Contribuyente);
                            Hacienda.Convertir(Anulacion,fac.Fecha_Emision_Documento).CrearXml(Tipo_documento.Nota_de_crédito_electrónica).Enviar();
                            Anulacion.XML_Enviado = Hacienda.XML.InnerXml;
                           // new FacturaPDF.FacturaElectronicaPDF().CrearFactura(fac);
                   
                        db.SaveChanges();
                        RecursosSistema.WindosNotification("Factura", "La nota de crédito Clave [" + Anulacion.Clave + "] se envío para su valoración");
                        RecursosSistema.Servicio_AgregarFactura(Anulacion.Clave);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }                    
                }

               

            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al anular la factura", "Error", MessageBoxButton.OK , MessageBoxImage.Error);
            }
        }
    }
}
