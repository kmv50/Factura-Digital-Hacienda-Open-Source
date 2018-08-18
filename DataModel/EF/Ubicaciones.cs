using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Ubicaciones")]
    public partial class Ubicacion
    {
        [Key]
        public int Id_Ubicacion { get; set; }

        public int Id_Provincia { get; set; }

        [Required]
        [StringLength(50)]
        public string Provincia { get; set; }

        public int Id_Canton { get; set; }

        [Required]
        [StringLength(50)]
        public string Canton { get; set; }

        public int Id_Distrito { get; set; }

        [Required]
        [StringLength(50)]
        public string Distrito { get; set; }

        public int Id_Barrio { get; set; }

        [Required]
        [StringLength(50)]
        public string Barrio { get; set; }
    }
}
