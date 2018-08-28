using DataModel;
using DataModel.EF;
using FacturaDigital.FacturaPDF;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace FacturaDigital.SendSmtp
{
    public class SendSmtp : ILog
    {
        private SMTP emailInfo;
        private  int Id_Facura;
        private byte[] FacturaPdfArray;
        private string ReceptorEmail;
        private string ContribuyenteNombre;
        private string XmlEnviado;
        private string XmlRespuesta;

        public SendSmtp(int Id_Facura)
        {
            this.Id_Facura = Id_Facura;
            PrepareEmailSend();
        }

        private void PrepareEmailSend()
        {
            Factura fac = null;
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
                emailInfo = db.SMTP.FirstOrDefault(q => q.Id_Contribuyente == Recursos.RecursosSistema.Contribuyente.Id_Contribuyente);
                if (emailInfo == null)
                {
                    throw new Exception("Favor llenar los datos del email antes de continuar");
                }

                fac = db.Factura.FirstOrDefault(q => q.Id_Factura == Id_Facura);
                
                if (fac == null)
                {
                    throw new Exception("Factura no encontrada");
                }
            }

            XmlEnviado = fac.XML_Enviado;
            XmlRespuesta = fac.XML_Respuesta;
            ReceptorEmail = fac.Receptor_CorreoElectronico;
            ContribuyenteNombre = fac.Emisor_Nombre;
            string url =  new FacturaElectronicaPDF().CrearFactura(fac);
            if (string.IsNullOrEmpty(url))
                throw new Exception("Error al crear el Pdf de la factura");


            FacturaPdfArray = File.ReadAllBytes(url);
            if (FacturaPdfArray == null || FacturaPdfArray.Length == 0)
                throw new Exception("Error al serializar factura");
           
        }

        public void Enviar()
        {
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.UseDefaultCredentials = false;
                    client.Host = emailInfo.Url_Servidor;
                    client.Port = emailInfo.Puerto;
                    client.EnableSsl = emailInfo.SSL;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new System.Net.NetworkCredential(emailInfo.Usuario, emailInfo.Contrasena);
                    client.Timeout = 10000;


                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(emailInfo.Usuario, ContribuyenteNombre);
                    mail.To.Add(emailInfo.Usuario);
                    mail.To.Add(ReceptorEmail);
                  
                    mail.Subject = "Factura Electronica "+ ContribuyenteNombre;
                    mail.Body = emailInfo.Detalle_Email;
                    mail.IsBodyHtml = true;

                    mail.Attachments.Add(new Attachment(new MemoryStream(FacturaPdfArray), "Documento" + ".pdf"));
                    mail.Attachments.Add(Attachment.CreateAttachmentFromString(XmlEnviado, "Enviada.xml"));
                    if(!string.IsNullOrEmpty(XmlRespuesta))
                        mail.Attachments.Add(Attachment.CreateAttachmentFromString(XmlRespuesta, "Respuesta.xml"));
                    client.Send(mail);
                }
            
            }
            catch (Exception ex)
            {
                this.LogError(ex);

            }
        }
    }
}
