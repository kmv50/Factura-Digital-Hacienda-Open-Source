using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Factura_Resolucion_Detalle_Impuesto")]
    public class Factura_Resolucion_Detalle_Impuesto
    {
        [Key]
        [Column(Order = 1)]
        public int Id_Factura_Resolucion_Detalle_Impuesto { set; get; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Impuesto_Codigo { get; set; }

        public decimal Impuesto_Tarifa { get; set; }

        public decimal Impuesto_Monto { get; set; }

        public int Id_Factura_Resolucion_Detalle { get; set; }
        public virtual Factura_Resolucion_Detalle Factura_Resolucion_Detalle { get; set; }
    }
}
