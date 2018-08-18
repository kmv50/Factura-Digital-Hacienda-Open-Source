using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Facturas_Detalles")]
    public partial class Factura_Detalle
    {
        [Key]
        public int Id_Factura_Detalle { set; get; }

        [StringLength(20)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(15)]
        public string Unidad_Medida { get; set; }

        [Required]
        [StringLength(160)]
        public string ProductoServicio { get; set; }

        public decimal PrecioUnitario { get; set; }

        public bool Tipo { get; set; }

        public decimal Monto_Total { get; set; }

        public decimal? Monto_Descuento { get; set; }

        [StringLength(80)]
        public string Naturaleza_Descuento { get; set; }

        public decimal SubTotal { get; set; }

        public bool Gravado { get; set; }

        public decimal Monto_Total_Linea { get; set; }

        public int Id_Factura { get; set; }
        [ForeignKey("Id_Factura")]
        public virtual Factura Factura { get; set; }

    }
}
