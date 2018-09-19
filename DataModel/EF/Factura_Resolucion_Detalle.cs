using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    public class Factura_Resolucion_Detalle
    {
        public Factura_Resolucion_Detalle()
        {
            Factura_Resolucion_Detalle_Impuesto = new HashSet<Factura_Resolucion_Detalle_Impuesto>();
        }

        [Key]
        public int Id_Factura_Resolucion_Detalle { set; get; }

        [StringLength(20)]
        public string Codigo { get; set; }    

        public int Cantidad { set; get; }

        [Required]
        [StringLength(160)]
        public string ProductoServicio { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Monto_Total { get; set; }

        public decimal? Monto_Descuento { get; set; }

        [StringLength(80)]
        public string Naturaleza_Descuento { get; set; }

        public decimal SubTotal { get; set; }

        public bool Gravado { get; set; }

        public decimal? Impuesto_Monto { get; set; }

        public decimal Monto_Total_Linea { get; set; }

        public int Id_Factura_Resolucion { get; set; }
        public virtual Factura_Resolucion Factura_Resolucion { get; set; }
        public virtual ICollection<Factura_Resolucion_Detalle_Impuesto> Factura_Resolucion_Detalle_Impuesto { get; set; }
    }
}
