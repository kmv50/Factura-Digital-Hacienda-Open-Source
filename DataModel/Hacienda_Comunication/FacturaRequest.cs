using Newtonsoft.Json;

namespace DataModel.Hacienda_Comunication
{
    public class FacturaClient
    {
        public string tipoIdentificacion { set; get; }
        public string numeroIdentificacion { set; get; }
    }

    public class FacturaRequest
    {
        public string clave { set; get; }
        public string fecha { set; get; }
        public FacturaClient emisor { set; get; }
        public FacturaClient receptor { set; get; }
        public string callbackUrl { set; get; }
        public string comprobanteXml { set; get; }
    }


    public class FacturaRequestMensajesConfirma : FacturaRequest
    {
        public string consecutivoReceptor { set; get; }
    }

    public enum FacturaEstados //valores tomados segun la documentacion de hacienda 
    {
        RECIBIDO,
        PROCESANDO,
        ACEPTADO,
        RECHAZADO,
        ERROR
    }

    public class RespuestaHaciendaModel
    {
        public string clave { set; get; }
        public string fecha { set; get; }
        [JsonProperty("ind-estado")]
        public string ind_estado { set; get; }
        [JsonProperty("respuesta-xml")]
        public string respuesta_xml { set; get; }
        public FacturaEstados Estado { set; get; }
    }
}