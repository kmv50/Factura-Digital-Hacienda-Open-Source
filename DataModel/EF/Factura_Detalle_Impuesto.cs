using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Factura_Detalles_Impuestos")]
    public partial class Factura_Detalle_Impuesto
    {
        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Impuesto_Codigo { get; set; }

        public decimal Impuesto_Tarifa { get; set; }

        public decimal Impuesto_Monto { get; set; }

        public bool Exento { set; get; }


        [StringLength(2)]
        [Column(TypeName = "char")]
        public string Exoneracion_TipoDocumento { get; set; }

        [StringLength(17)]
        [Column(TypeName = "varchar")]
        public string Exoneracion_NumeroDocumento { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string Exoneracion_NombreInstitucion { get; set; }

        public decimal? Exoneracion_MontoImpuesto { get; set; }

        public DateTime? Exoneracion_FechaEmision { get; set; }

        [StringLength(3)]
        [Column(TypeName = "char")]
        public string Exoneracion_PorcentajeCompra { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Id_Factura_Detalle { get; set; }
        [ForeignKey("Id_Factura_Detalle")]
        public virtual Factura_Detalle Factura_Detalle { get; set; }
    }
}
