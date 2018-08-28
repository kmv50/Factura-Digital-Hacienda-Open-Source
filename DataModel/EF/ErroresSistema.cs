using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Errores_Sistema")]
    public partial class Errores_Sistema
    {
        [Key]
        public DateTime Fecha { set; get; }

        [StringLength(16777215)]
        public string Mensaje { get; set; }

        [StringLength(16777215)]
        public string Stacktrace { get; set; }

        [StringLength(16777215)]
        public string Inner_Mensaje { get; set; }

        [StringLength(16777215)]
        public string Inner_Stacktrace { get; set; }
    }
}
