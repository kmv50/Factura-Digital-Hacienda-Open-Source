using DataModel;
using DataModel.EF;
using DataModel.Hacienda_Comunication;
using FacturaDigital.Recursos;
using FacturaElectronica_V_4_3;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace FacturaDigital.Faturacion
{
    /// <summary>
    /// Lógica de interacción para Comprobacion.xaml
    /// </summary>
    public partial class Comprobacion : Page, ILog
    {
        private Factura_Resolucion FacturaResolucion = null;

        public Comprobacion()
        {
            InitializeComponent();
        }

        private void BuscarXML(object sender, RoutedEventArgs e)
        {
            string FileUrl = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = false,
                    DefaultExt = "*.xml",
                    Filter = "Documentos Electronicos (*.xml) | *.xml;"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    FileUrl = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                FileUrl = null;
                this.LogError(ex);
            }


            if (string.IsNullOrEmpty(FileUrl))
            {
                MessageBox.Show("Error al obtener la direccion del archivo de XML", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            try
            {

                Tipo_documento typeDocument = Tipo_documento.Factura_electrónica;
                XmlDocument xml = new XmlDocument();
                xml.Load(FileUrl);
                string NS_String = xml.DocumentElement.NamespaceURI;

                if (NS_String != "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/facturaElectronica" && NS_String != "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/tiqueteElectronico")
                {
                    MessageBox.Show("El tipo de xml seleccionado es incorrecto", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (NS_String == "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/tiqueteElectronico")
                {
                    typeDocument = Tipo_documento.Tiquete_Electrónico;
                }

                if (typeDocument == Tipo_documento.Factura_electrónica)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FacturaElectronica));
                    FacturaElectronica Factura = null;
                    using (XmlReader reader = new XmlNodeReader(xml))
                    {
                        Factura = (FacturaElectronica)serializer.Deserialize(reader);
                    }

                    FacturaResolucion = new ConvertirFacturaXml_FacturaDB().Convertir(Factura, xml.InnerXml);

                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al abrir el archivo de XML", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool Encontrada = false;
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                Encontrada = db.Factura_Resolucion.Any(q => q.Clave == FacturaResolucion.Clave);
            }

            if (Encontrada)
            {
                MessageBox.Show("La factura seleccionada ya se encuentra en el sistema","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                return;
            }

            DataModel.EF.Contribuyente con = Recursos.RecursosSistema.Contribuyente;
            if (con.Identificacion_Numero != FacturaResolucion.Receptor_Identificacion)
            {
                MessageBoxResult result = MessageBox.Show("El  numero de identicacion de la factura no concuerda con el numero de identificacion del actual contribuyente Identificacion receptor factura[" + FacturaResolucion.Receptor_Identificacion + "] Identificacion del contribuyente actual [" + con.Identificacion_Numero + "] La factura esta a nombre de [" + FacturaResolucion.Receptor__Nombre + "]. Sabiendo lo anterior desea continuar ?", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            RenderFactura();
        }


        private void RenderFactura() {
            lb_ClaveDocumento.Text = FacturaResolucion.Clave;
            txt_ENombre.Text = FacturaResolucion.Emisor_Nombre;
            txt_ECorreo.Text = FacturaResolucion.Emisor_CorreoElectronico;
            txt_EIdentificacion.Text = FacturaResolucion.Emisor_Identificacion_Numero;
            txt_Etelefono.Text = FacturaResolucion.Emisor_Telefono_Numero;
            txt_Fecha.Text = FacturaResolucion.Fecha_Documento_Origen.ToString();
            txt_Descuento.Text = (FacturaResolucion.TotalDescuentos ?? 0).ToString();
            txt_Impuesto.Text = (FacturaResolucion.TotalImpuesto ?? 0).ToString();
            txt_SubTotal.Text = FacturaResolucion.TotalVentaNeta.ToString();
            txt_Total.Text = FacturaResolucion.TotalComprobante.ToString();
            txt_Moneda.Text = FacturaResolucion.Codigo_Moneda;
            dg_detalleFactura.ItemsSource = FacturaResolucion.Factura_Resolucion_Detalle;
            DetalleFactura.Visibility = Visibility.Visible;
        }

        public string FormatoConsecutivoAceptacion(int consecutivo, int Resolucion_Comprobante)
        {
            string CasaMatriz = "001";
            string TeminalPuntoVenta = "00001";
            string TipoComprobante = null;
            if (Resolucion_Comprobante == 1)
                TipoComprobante = Tipo_documento.Confirmación_aceptación.GetAttribute<XmlEnumAttribute>().Name;
            else if (Resolucion_Comprobante == 2)
                TipoComprobante = Tipo_documento.Confirmación_aceptación_parcial.GetAttribute<XmlEnumAttribute>().Name;
            else
                TipoComprobante = Tipo_documento.Confirmación_rechazo_comprobante.GetAttribute<XmlEnumAttribute>().Name;

            string Consecutivo = consecutivo.ToString("0000000000");

            return CasaMatriz + TeminalPuntoVenta + TipoComprobante + Consecutivo;
        }

        private void EnviarAHacienda(object sender, RoutedEventArgs e)
        {
            try
            {
                loadingDisplayer.Visibility = Visibility.Visible;
                FacturaResolucion.Resolucion = Convert.ToInt32(((ComboBoxItem)cb_Resolucion.SelectedItem).Tag);
                FacturaResolucion.DetalleResolucion = txt_DetalleResolucion.Text;
                Contribuyente_Consecutivos Consecutivo;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Consecutivo = db.Contribuyente_Consecutivos.First(q => q.Id_Contribuyente == RecursosSistema.Contribuyente.Id_Contribuyente);
                    FacturaResolucion.NumeroConsecutivo = FormatoConsecutivoAceptacion(Consecutivo.Consecutivo_Confirmacion, FacturaResolucion.Resolucion);
                    FacturaResolucion.Email_Enviado = false;
                    FacturaResolucion.Fecha_Documento = DateTime.Now;
                    db.Factura_Resolucion.Add(FacturaResolucion);
                    Consecutivo.Consecutivo_Confirmacion++;

                    try
                    {

                        new FacturaDB_ToMensajeReceptor(RecursosSistema.Contribuyente).Convertir(FacturaResolucion)
                            .CrearXml(Tipo_documento.Confirmación_aceptación)
                            .Enviar();
                        db.SaveChanges();

                        RecursosSistema.WindosNotification("Confirmacion", "El documento Clave [" + FacturaResolucion.Clave + "] se envío para su valoración");
                        RecursosSistema.Servicio_AgregarFactura(FacturaResolucion.Clave+"-"+ FacturaResolucion.NumeroConsecutivo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    loadingDisplayer.Visibility = Visibility.Collapsed;

                }
            }
            catch(Exception ex)
            {
                loadingDisplayer.Visibility = Visibility.Collapsed;
                this.LogError(ex);
                MessageBox.Show("Error al enviar la respuesta a Hacienda","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
