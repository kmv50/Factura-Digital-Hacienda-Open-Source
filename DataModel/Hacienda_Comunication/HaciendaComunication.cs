using DataModel.EF;
using DataModel.Hacienda_Comunication.IDP;
using FacturaElectronica_V_4_2;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataModel.Hacienda_Comunication
{
    public  class HaciendaComunication : ILog , ISerializeToXML
    {
        protected Contribuyente Contribuyente { set; get; }
        protected object DocumentoElectronico { set; get; }
        public XmlDocument XML { set; get; }
        protected FacturaRequest requestData;

        public HaciendaComunication(Contribuyente Contribuyente) {
            this.Contribuyente = Contribuyente;
        }

        public HaciendaComunication CrearXml(Tipo_documento tipo) {
            XmlDocument doc = this.SerializeToXML<FacturaElectronica>(DocumentoElectronico, tipo.GetAttribute<NameSpaceAttribute>().Description);

            if (Contribuyente.Certificado == null || doc == null)
                throw new Exception("Datos invalidos el certificado o el documento xml no son validos");

            X509Certificate2 cert = new X509Certificate2(Contribuyente.Certificado, Contribuyente.Contrasena_Certificado, X509KeyStorageFlags.Exportable);
            if (cert == null)
                throw new Exception("Error Al abrir el certificadp x509");

            XML = XmlMerge(doc, cert);
            return this;
        }

        public void Enviar()
        {            
            requestData.comprobanteXml = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(XML.InnerXml));
            ApiResponse response = new ApiClient().POST(new ApiRequest()
            {
                Token = GetTokenAutentification().id_token,
                Route = "recepcion",
                Request = requestData
            });


            if (response == null)
            {
                throw new Exception("Error de comunicacion con Hacienda intente mas tarde");
            }

            if (response.Code != System.Net.HttpStatusCode.OK && response.Code != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception("Error Hacienda respondio con un codigo de error");
            }
        }
        

        private XmlDocument XmlMerge(XmlDocument xmlDoc, X509Certificate2 certificate)
        {

            XadesService xadesService = new XadesService();
            SignatureParameters parametros = new SignatureParameters();


            parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
            parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
            parametros.SignaturePolicyInfo.PolicyHash = "Ohixl6upD6av8N7pEvDABhEL6hM=";
            parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
            parametros.InputMimeType = "text/xml";
            parametros.SignerRole = new SignerRole();
            parametros.SignerRole.ClaimedRoles.Add("emisor");

            using (parametros.Signer = new Signer(certificate))
            {
                var docFirmado = xadesService.Sign(xmlDoc, parametros);
                return docFirmado.Document;
            }

        }

        private TokenIdp GetTokenAutentification()
        {
            ApiResponse response = null;
            try
            {
                string urlIDP = ConfigurationManager.AppSettings.Get("HaciendaIDP");
                string client_Id = ConfigurationManager.AppSettings.Get("HaciendaIDP_client_id");

                response = new ApiClient(5, urlIDP).IDP_Post(new ApiRequest()
                {
                    Request = new TokenRequest()
                    {
                        client_id = client_Id,
                        client_secret = "",
                        grant_type = "password",
                        username = Contribuyente.UsuarioHacienda,
                        password = Contribuyente.ContrasenaHacienda,
                        scope = ""
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (response == null)
            {
                throw new Exception("Error al obtener el token del IDP");
            }

            if (response.Code == System.Net.HttpStatusCode.Unauthorized)
                throw new Exception("Credenciales del usuario invalidas");

            try
            {
                return response.mapObjet<TokenIdp>();
            }catch(Exception ex)
            {                
                this.LogError(new Exception(response.Response,ex));
                throw new Exception("Respuesta invalida de Hacienda al solicitar token de autenticación");
            }

        }
    }
}
