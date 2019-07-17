using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Productos_Impuestos")]
    public partial class Producto_Impuesto
    {
        [Key]
        public int Id_Producto_Impuesto { get; set; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Impuesto_Codigo { get; set; }

        public bool Exento { set; get; }

        public decimal Impuesto_Tarifa { get; set; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Exoneracion_TipoDocumento { get; set; }

        [StringLength(17)]
        public string Exoneracion_NumeroDocumento { get; set; }

        [StringLength(100)]
        public string Exoneracion_NombreInstitucion { get; set; }

        public decimal? Exoneracion_MontoImpuesto { get; set; }

        public DateTime? Exoneracion_FechaEmision { get; set; }

        public int Exoneracion_PorcentajeCompra { get; set; }

        public int Id_Producto { set; get; }

        [StringLength(3)]
        [Column(TypeName = "char")]
        public string CodigoTarifa { get; set; }

        [ForeignKey("Id_Producto"), Required]
        public virtual Producto Producto { set; get; }

    }
}
