using DataModel.EF;
using FacturaElectronica_V_4_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Conversores
{
    public class FacturaDB_ToFacturaElectronica
    {
        public FacturaElectronica Convertir(Factura facturaDB)
        {
            FacturaElectronica facturaHacienda = new FacturaElectronica()
            {

            };


            return facturaHacienda;
        }
    }
}
