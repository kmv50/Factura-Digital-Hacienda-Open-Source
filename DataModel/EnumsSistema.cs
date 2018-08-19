using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public enum Tipo_documento
    {
        Factura_electrónica = 1,
        Nota_de_débito_electrónica = 2,
        Nota_de_crédito_electrónica = 3,
        Tiquete_Electrónico = 4,
        Confirmación_aceptación = 5,
        Confirmación_aceptación_parcial = 6,
        Confirmación_rechazo_comprobante = 7
    }

    public enum Situación_Comprobante
    {
        Normal = 1,
        Sin_Internet = 2,
        Contingencia = 3
    }

    public enum EstadoComprobante
    {
        //Estdos Hacienda
        Aceptado = 1,
        AceptadoParcialmente  = 2,
        Rechazado = 3,
        //Estados Sistema
        Enviado = 0,        
        ErrorEnviar = -1,
        Anulando = -2,
        ErrorAnulando = -3 
    }
}
