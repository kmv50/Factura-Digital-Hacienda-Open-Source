using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Contribuyentes")]
    public partial class Contribuyente
    {
        [Key]
        public int Id_Contribuyente { get; set; }

        [Required]
        [StringLength(80)]
        public string Nombre { get; set; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Identificacion_Tipo { get; set; }

        [StringLength(20)]
        public string Identificacion_Numero { get; set; }


        [StringLength(80)]
        public string NombreComercial { get; set; }

        [StringLength(60)]
        public string CorreoElectronico { get; set; }

        public int Provincia { get; set; }

        public int Canton { get; set; }

        public int Distrito { get; set; }

        public int Barrio { get; set; }


        [StringLength(160)]
        public string OtrasSenas { get; set; }

        [DataType("int(3)")]
        public int Telefono_Codigo { get; set; }

        [DataType("int(8)")]
        public int Telefono_Numero { get; set; }

        [StringLength(250)]
        public string UsuarioHacienda { get; set; }

        [StringLength(250)]
        public string ContrasenaHacienda { get; set; }

        [Column(TypeName = "blob")]
        public byte[] Certificado { get; set; }

        [StringLength(20)]
        public string Contrasena_Certificado { get; set; }

        public DateTime? FechaExpiracion { get; set; }
    }
}
