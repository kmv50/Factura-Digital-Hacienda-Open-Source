using DataModel;
using DataModel.EF;
using DataModel.Hacienda_Comunication;
using FacturaDigital.Recursos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace FacturaDigital.Servicio_Consulta
{
    public class ConsultaRequest {
        public string Clave { set; get; }
        public DateTime FechaEmicion { set; get; }
        public string Token { set; get; }
        public int Errores { set; get; }
    }

    public class ServicioConsulta: ILog
    {
        Thread ServicioConsultaThread;
        BlockingCollection<ConsultaRequest> FacturasPendientes;
        DataModel.EF.Contribuyente ContribuyenteCopiaLocal;
        public ServicioConsulta() {
            ContribuyenteCopiaLocal = new DataModel.EF.Contribuyente();
            FacturasPendientes = new BlockingCollection<ConsultaRequest>();
            LeerFacturasPendientes();
            Iniciar();
        }

        void LeerFacturasPendientes() {
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                foreach (ConsultaRequest Row in db.Factura.Where(q => q.Estado == (int)EstadoComprobante.Enviado).Select(q => new ConsultaRequest (){ Clave = q.Clave , FechaEmicion = q.Fecha_Emision_Documento }))
                {
                    Row.Token = Guid.NewGuid().ToString();
                    FacturasPendientes.Add(Row);
                }
            }
        }

        public void Detener() {
            if(ServicioConsultaThread != null)
            {
                ServicioConsultaThread.Abort();
            }
        }

        public void Iniciar() {
            lock (ContribuyenteCopiaLocal)
            {
                ContribuyenteCopiaLocal = new DataModel.EF.Contribuyente()
                {
                    Id_Contribuyente = RecursosSistema.Contribuyente.Id_Contribuyente,
                    Certificado = RecursosSistema.Contribuyente.Certificado,
                    Contrasena_Certificado = RecursosSistema.Contribuyente.Contrasena_Certificado,
                    UsuarioHacienda = RecursosSistema.Contribuyente.UsuarioHacienda,
                    ContrasenaHacienda = RecursosSistema.Contribuyente.ContrasenaHacienda
                };
            }

            if (ServicioConsultaThread == null)
            {
                ServicioConsultaThread = new Thread(Loop);
                ServicioConsultaThread.Name = "ServicioConsultaThread";
                ServicioConsultaThread.Start();
            }
        }

        private  void ObtenerEstadoRespuestaHacienda(string xmlRespuestaHacienda , out int EstadoHacienda , out string Detalle ) {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlRespuestaHacienda);

                string NS_String = xml.DocumentElement.NamespaceURI;
                XmlNamespaceManager ns = new XmlNamespaceManager(xml.NameTable);
                ns.AddNamespace("ns", NS_String);


                int HaciendaEstatus = Convert.ToInt32(xml.SelectSingleNode("//ns:Mensaje", ns).InnerText);
                string HaciendaDetalle = xml.SelectSingleNode("//ns:DetalleMensaje", ns).InnerText;
                EstadoHacienda = HaciendaEstatus; // 1 bien 2 la aceptaron parcialmente 3 fallo 
                Detalle = HaciendaDetalle;

            }
            catch (Exception ex)
            {
                this.LogError(ex);
                Detalle = null;
                EstadoHacienda = (int)EstadoComprobante.ErrorAlDescomprimirXMLHacienda;
            }
        }

        internal void AgregarFactura(string clave)
        {
            FacturasPendientes.Add(new ConsultaRequest() {
                Clave = clave,
                FechaEmicion = DateTime.Now,
                Token = Guid.NewGuid().ToString()                
            });
        }

        void Loop() {
            string LastConsultaToken= null;
            while (true)
            {
                try
                {
                    ConsultaRequest Consulta = FacturasPendientes.Take();
                    try
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 10));
                        ///En teoria lo que esto deberia de hacer es que si una misma factura 
                        ///Se llama seguida espere un segundo. De esa forma si tengo una cola de 5 facturas
                        ///y de esas 5 una esta pegana no me detenga a las otras y a la vez para no afectar el rendimiento 
                        ///ya que seria peligroso que se encicle por una factura que esta en error o que hacienda dure mucho 
                        ///de todas formas al final hay un sleep de 10 seg por cualquier cosa
                        ///la otra cosa que hace es darle tiempo a hacienda a que termine de procesar
                        ///entonces con esto podemos decir que si tenemos 5 facturas las procese rapido por eje 
                        ///si de las 5 hay 2 con error las 3 buenas se van a procesar rapido 
                        ///las ultimas 2 van a quedar en el ciclo esperando un minuto teniendo en cuenta que esto es una cola y no una pila
                        if (Consulta.Token == LastConsultaToken)
                            Thread.Sleep(new TimeSpan(0, 1, 0));
                        else
                            LastConsultaToken = Consulta.Token;

                        RespuestaHaciendaModel Respuesta;
                        lock (ContribuyenteCopiaLocal)
                        {
                            Respuesta = new HaciendaComunication(ContribuyenteCopiaLocal).Consultar(Consulta.Clave);
                        }

                        if (Respuesta != null)
                        {
                            /*
                             recibido,procesando,aceptado,rechazado
                             */
                            if (Respuesta.ind_estado.ToLower().Equals("aceptado") || Respuesta.ind_estado.ToLower().Equals("rechazado"))
                            {
                                Respuesta.respuesta_xml = Encoding.UTF8.GetString(Convert.FromBase64String(Respuesta.respuesta_xml));
                                using (db_FacturaDigital db = new db_FacturaDigital())
                                {
                                    Factura fac = db.Factura.First(q => q.Clave == Consulta.Clave);
                                    fac.XML_Respuesta = Respuesta.respuesta_xml;
                                    int EstadoHacienda;
                                    string DetalleRespuestaHacienda;
                                    ObtenerEstadoRespuestaHacienda(Respuesta.respuesta_xml, out EstadoHacienda, out DetalleRespuestaHacienda);
                                    fac.Estado = EstadoHacienda;
                                    fac.HaciendaDetalle = DetalleRespuestaHacienda;

                                    if((Tipo_documento)fac.Id_TipoDocumento == Tipo_documento.Nota_de_crédito_electrónica)
                                    {
                                       Factura facoriginal = db.Factura.FirstOrDefault();
                                        if (EstadoHacienda == 1)
                                            facoriginal.Estado = (int)EstadoComprobante.Anulando;
                                        else if (EstadoHacienda == 3)
                                            facoriginal.Estado = (int)EstadoComprobante.ErrorAnulando;
                                    }

                                    db.SaveChanges();
                                    if (fac.Estado == 1 || fac.Estado == 2)
                                    {
                                        RecursosSistema.WindosNotification("Facturación", "Factura aceptada [" + Consulta.Clave + "]");
                                        new SendSmtp.SendSmtp(fac.Id_Factura).Enviar();
                                        fac.Email_Enviado = true;
                                    }
                                    else if (fac.Estado == 3)
                                        RecursosSistema.WindosNotification("Facturación", "Factura rechazada [" + Consulta.Clave + "]");

                                }
                            }
                            else if (Respuesta.ind_estado.ToLower().Contains("error"))
                            {
                                using (db_FacturaDigital db = new db_FacturaDigital())
                                {
                                    Factura fac = db.Factura.First(q => q.Clave == Consulta.Clave);
                                    fac.Estado = (int)EstadoComprobante.ErrorInternoHacienda;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                if (Consulta.FechaEmicion.AddHours(3) < DateTime.Now)
                                {
                                    using (db_FacturaDigital db = new db_FacturaDigital())
                                    {
                                        Factura fac = db.Factura.First(q => q.Clave == Consulta.Clave);
                                        fac.Estado = (int)EstadoComprobante.SinRespuestaDeHacienda;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    FacturasPendientes.Add(Consulta);//vuelva a ponerla en lista de espera si es la unica en teoria debera esprar un minuto 10 seg
                                }
                            }
                        }
                    }catch(Exception ex)
                    {
                        Consulta.Errores++;
                        if (Consulta.Errores >= 3)
                            RecursosSistema.WindosNotification("Error", "Error al procesar la respuesta de la factura [" + Consulta.Clave + "]");
                        else
                        {
                            this.LogError(ex);
                            FacturasPendientes.Add(Consulta);//vuelva a ponerla en lista de espera si es la unica en teoria debera esprar un minuto 10 seg                    
                        }
                        
                    }
                }catch(Exception ex)
                {
                    this.LogError(ex);
                }

            }
        }
        
    }
}
