using DataModel.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataModel.Hacienda_Comunication
{
    public class FacturaDB_ToMensajeReceptor : HaciendaComunication
    {

        public FacturaDB_ToMensajeReceptor(Contribuyente contribuyente) : base(contribuyente)
        {
        }
       

        public FacturaDB_ToMensajeReceptor Convertir(Factura_Resolucion facturaResolucion)
        {
            MensajeReceptor documento = new MensajeReceptor()
            {
                DetalleMensaje = facturaResolucion.DetalleResolucion,
                FechaEmisionDoc = facturaResolucion.Fecha_Documento,
                Mensaje = EnumUtils.SetTypeString<MensajeReceptorMensaje>(facturaResolucion.Resolucion.ToString()),
                TotalFactura = facturaResolucion.TotalComprobante,
                Clave = facturaResolucion.Clave,
                NumeroCedulaEmisor = facturaResolucion.Emisor_Identificacion_Numero.PadLeft(12).Replace(" ", "0"),
                NumeroCedulaReceptor = Contribuyente.Identificacion_Numero.PadLeft(12).Replace(" ", "0"),
                NumeroConsecutivoReceptor = facturaResolucion.NumeroConsecutivo
            };

            if (facturaResolucion.TotalImpuesto.HasValue)
            {
                documento.MontoTotalImpuestoSpecified = true;
                documento.MontoTotalImpuesto = facturaResolucion.TotalImpuesto.Value;
            }


            requestData = new FacturaRequest()
            {
                clave = facturaResolucion.Clave,
                emisor = new FacturaClient()
                {
                    numeroIdentificacion = Contribuyente.Identificacion_Numero,
                    tipoIdentificacion = EnumUtils.SetTypeString< FacturaElectronica_V_4_2.IdentificacionTypeTipo >(Contribuyente.Identificacion_Tipo).ToString()
                },
                fecha = facturaResolucion.Fecha_Documento.ToString("yyyy-MM-ddTHH:mm:ss"),
            };
        
            DocumentoElectronico = documento;
            return this;
        }
    }
}
