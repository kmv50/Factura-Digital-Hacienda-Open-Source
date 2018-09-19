using DataModel;
using DataModel.EF;
using DataModel.Hacienda_Comunication;
using FacturaElectronica_V_4_2;
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

            Factura_Resolucion fac = null;
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

                    fac = new ConvertirFacturaXml_FacturaDB().Convertir(Factura, xml.InnerXml);

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
                Encontrada = db.Factura_Resolucion.Any(q => q.Clave == fac.Clave);
            }

            if (Encontrada)
            {
                MessageBox.Show("La factura seleccionada ya se encuentra en el sistema","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                return;
            }

            DataModel.EF.Contribuyente con = Recursos.RecursosSistema.Contribuyente;
            if (con.Identificacion_Numero != fac.Receptor_Identificacion)
            {
                MessageBoxResult result = MessageBox.Show("El  numero de identicacion de la factura no concuerda con el numero de identificacion del actual contribuyente Identificacion receptor factura[" + fac.Receptor_Identificacion + "] Identificacion del contribuyente actual [" + con.Identificacion_Numero + "] La factura esta a nombre de [" + fac.Receptor__Nombre + "]. Sabiendo lo anterior desea continuar ?", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }



           

        }


    }
}
