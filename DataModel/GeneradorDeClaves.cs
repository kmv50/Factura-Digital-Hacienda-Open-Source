using System;

namespace DataModel
{
    public class ConsecutivoHacienda
    {
        public int CasaMatriz { set; get; }
        public int PuntoVenta { set; get; }
        public Tipo_documento TipoDocumento { set; get; }
        public int NumeracionConsecutiva { set; get; }

        public override string ToString()
        {
            return CasaMatriz.ToString("000")
                   + PuntoVenta.ToString("00000")
                   + ((int)TipoDocumento).ToString("00")
                   + NumeracionConsecutiva.ToString("0000000000");
        }

        public ConsecutivoHacienda()
        {
            CasaMatriz = 1;
            PuntoVenta = 1;
            TipoDocumento = Tipo_documento.Factura_electrónica;
        }

        public ConsecutivoHacienda(ConsecutivoHacienda conse)
        {
            CasaMatriz = conse.CasaMatriz;
            PuntoVenta = conse.PuntoVenta;
            TipoDocumento = conse.TipoDocumento;
            NumeracionConsecutiva = conse.NumeracionConsecutiva;

            if (CasaMatriz == 0 || PuntoVenta == 0 || TipoDocumento == 0 || NumeracionConsecutiva == 0)
                throw new ArgumentException("Los datos del consecutivo estan incompletos");
        }

        public ConsecutivoHacienda(string CosecutivoFormatoHacienda)
        {
            if (CosecutivoFormatoHacienda.Length != 20)
            {
                throw new Exception("Error el formato del consecutivo ingresado no es valido");
            }

            CasaMatriz = Convert.ToInt32(CosecutivoFormatoHacienda.Substring(0, 3));
            PuntoVenta = Convert.ToInt32(CosecutivoFormatoHacienda.Substring(3, 5));
            TipoDocumento = (Tipo_documento)Convert.ToInt32(CosecutivoFormatoHacienda.Substring(8, 2));
            NumeracionConsecutiva = Convert.ToInt32(CosecutivoFormatoHacienda.Substring(10, 10));
        }
    }

    public class GeneradorDeClavesHacienda
    {
        public DateTime FechaEmicion { set; get; }
        public Situación_Comprobante Situación_Comprobante { set; get; }
        public int CodigoPais { set; get; }
        public int Identificacion_Contribuyente { set; get; }
        private int codigoSeguridad;
        public int CodigoSeguridad {
            get {
                return codigoSeguridad;
            }
        }
        public ConsecutivoHacienda ConsecutivoHacienda { set; get; }

        public GeneradorDeClavesHacienda() {
            CodigoPais = 506;
            Situación_Comprobante = Situación_Comprobante.Normal;
        }

        public GeneradorDeClavesHacienda(GeneradorDeClavesHacienda datos) {
            if(datos.CodigoPais == 0 || datos.Identificacion_Contribuyente == 0 || datos.ConsecutivoHacienda == null)
                throw new ArgumentException("Los datos de la clave estan incompletos");

            FechaEmicion = datos.FechaEmicion;
            Situación_Comprobante = datos.Situación_Comprobante;
            CodigoPais = datos.CodigoPais;
            ConsecutivoHacienda = datos.ConsecutivoHacienda;
            Identificacion_Contribuyente = datos.Identificacion_Contribuyente;
        }

        public override string ToString()
        {
            codigoSeguridad = new Random().Next(1, 99999999); 

            return CodigoPais.ToString("000")
                   + FechaEmicion.Day.ToString("00")
                   + FechaEmicion.Month.ToString("00")
                   + FechaEmicion.ToString("yy")
                   + Identificacion_Contribuyente.ToString("000000000000")
                   + ConsecutivoHacienda.ToString()
                   + ((int)Situación_Comprobante).ToString()
                   + codigoSeguridad.ToString("00000000");
        }

        public GeneradorDeClavesHacienda(string ClaveHacienda)
        {
            if (ClaveHacienda.Length != 50)
            {
                throw new Exception("Error el formato de la clave ingresada no es valido");
            }
        }
    }
}
