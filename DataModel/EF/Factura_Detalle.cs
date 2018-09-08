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
    public partial class Factura_Detalle : ICloneable
    {
        public Factura_Detalle()
        {
            Factura_Detalle_Impuesto = new HashSet<Factura_Detalle_Impuesto>();
        }

        [Key]
        public int Id_Factura_Detalle { set; get; }

        [StringLength(20)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(15)]
        public string Unidad_Medida { get; set; }

        public int Cantidad { set; get; }

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

        public decimal? Impuesto_Monto { get; set; }

        public decimal Monto_Total_Linea { get; set; }

        public int Id_Factura { get; set; }
        public virtual Factura Factura { get; set; }
        public virtual ICollection<Factura_Detalle_Impuesto> Factura_Detalle_Impuesto { get; set; }

        public object Clone()
        {
            return (Factura_Detalle)this.MemberwiseClone();
        }
    }
}
