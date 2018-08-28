using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.SMTP")]
    public partial class SMTP
    {
        [Key]
        public int Id_SMPT{ get; set; }

        [Required]
        [StringLength(150)]
        public string Url_Servidor { set; get; }

        [Required]
        [StringLength(150)]
        public string Usuario { set; get; }

        [Required]
        [StringLength(150)]
        public string Contrasena { set; get; }

        public int Puerto { set; get; }

        public bool SSL { set; get; }

        public int Id_Contribuyente { get; set; }
        [ForeignKey("Id_Contribuyente")]
        public virtual Contribuyente Contribuyente { get; set; }
    }
}
