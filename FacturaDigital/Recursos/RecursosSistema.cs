using FacturaDigital.Servicio_Consulta;
using System;
using System.Windows;
using System.Windows.Controls;
using Windows.UI.Notifications;

namespace FacturaDigital.Recursos
{
    public static class RecursosSistema
    {
        public delegate void Contribuyente_Load();
        public static Contribuyente_Load OnContribuyente_Load;
        public static Frame MainConteiner { set; get; }
        public static DataModel.EF.Contribuyente Contribuyente { get; internal set; }
        private static ServicioConsulta Servicio;

        public static void LogError(Exception ex)
        {

        }

        public static void IniciarServicioConsulta() {
            try
            {
                Servicio = new ServicioConsulta();
                Servicio.Iniciar();
            }catch(Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Error al iniciar el servicio de consulta","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        public static void DetenerServicioConsulta()
        {
            try
            {
                if(Servicio != null)
                {
                    Servicio.Detener();
                }
            }catch(Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Error al detener el servicio de consulta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static void WindosNotification(string Titulo , string Mensaje)
        {
            try
            {
                string xml = @"<toast>
                            <visual>
                                <binding template=""ToastImageAndText04"">
                                    <image id=""1"" src=""file:///C:\meziantou.jpeg"" alt=""meziantou""/>
                                    <text id=""1"">{0}</text>
                                    <text id=""2"">{1}</text>
                                </binding>
                            </visual>
                        </toast>";

                Windows.Data.Xml.Dom.XmlDocument toastXml = new Windows.Data.Xml.Dom.XmlDocument();
                toastXml.LoadXml(string.Format(xml, Titulo, Mensaje));
                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier("FacturaDigital").Show(toast);
            }catch(Exception ex)
            {
                LogError(ex);
            }
        }

        internal static void Servicio_AgregarFactura(string clave)
        {
            try
            {
                if (Servicio != null)
                {
                    Servicio.AgregarFactura(clave);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Error al agregar factura a servicio de consulta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
