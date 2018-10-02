using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Contribuyente_Consecutivos")]
    public class Contribuyente_Consecutivos
    {
        public int Consecutivo_Facturas { set; get; }
        public int Consecutivo_NotasCredito { set; get; }
        public int Consecutivo_Tiquete_Electrónico { set; get; }
        public int Consecutivo_Confirmacion { set; get; }
        [Key]
        public int Id_Contribuyente { get; set; }
        [ForeignKey("Id_Contribuyente")]
        public virtual Contribuyente Contribuyente { get; set; }
    }
}
