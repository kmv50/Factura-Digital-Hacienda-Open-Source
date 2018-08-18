using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Productos")]
    public partial class Producto
    {
        

        public Producto()
        {
            Producto_Impuesto = new HashSet<Producto_Impuesto>();       
        }

        [Key]
        public int Id_Producto { get; set; }

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

        public decimal? ImpuestosTarifaTotal { get; set; }

        public int Id_Contribuyente { get; set; }
        [ForeignKey("Id_Contribuyente")]
        public virtual Contribuyente Contribuyente { get; set; }

        public virtual ICollection<Producto_Impuesto> Producto_Impuesto { get; set; }
    }
}
