using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Facturas_Resoluciones")]
    public class Factura_Resolucion
    {
        public Factura_Resolucion() {
            Factura_Resolucion_Detalle = new HashSet<Factura_Resolucion_Detalle>();
        }

        [Key]
        public int Id_Factura_Resolucion { get; set; }

        public int Estado { get; set; }

        [Required]
        [StringLength(50)]
        public string Clave { get; set; }

        public int NumeroConsecutivo { get; set; }

        public DateTime Fecha_Documento_Origen { get; set; }
        public DateTime Fecha_Documento { get; set; }

        [StringLength(30)]
        public string Receptor_Identificacion { get; set; }
        [StringLength(80)]
        public string Receptor__Nombre { get; set; }


        [Required]
        [StringLength(80)]
        public string Emisor_Nombre { get; set; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Emisor_Identificacion_Tipo { get; set; }

        [StringLength(20)]
        public string Emisor_Identificacion_Numero { get; set; }

        [StringLength(80)]
        public string Emisor_NombreComercial { get; set; }

        [StringLength(25)]
        public string Emisor_Telefono_Numero { get; set; }

        [Required]
        [StringLength(60)]
        public string Emisor_CorreoElectronico { get; set; }
     
        [Column(TypeName = "char")]
        [Required]
        [StringLength(2)]
        public string CondicionVenta { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [StringLength(2)]
        public string MedioPago { get; set; }

        public int TipoDocumentoOrigen { get; internal set; }

        public decimal TipoCambio { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [StringLength(3)]
        public string Codigo_Moneda { get; set; }

        public decimal? TotalServGravados { get; set; }

        public decimal? TotalServExentos { get; set; }

        public decimal? TotalMercanciasGravadas { get; set; }

        public decimal? TotalMercanciasExentas { get; set; }

        public decimal? TotalGravado { get; set; }

        public decimal? TotalExento { get; set; }

        public decimal TotalVenta { get; set; }

        public decimal? TotalDescuentos { get; set; }

        public decimal TotalVentaNeta { get; set; }

        public decimal? TotalImpuesto { get; set; }

        public decimal TotalComprobante { get; set; }      

        public bool Email_Enviado { set; get; }

        [Column(TypeName = "text")]
        [StringLength(16777215)]
        public string XML_Subido { get; set; }

        [Column(TypeName = "text")]
        [StringLength(16777215)]
        public string XML_Enviado { get; set; }

        [Column(TypeName = "text")]
        [StringLength(16777215)]
        public string XML_Respuesta { get; set; }

        public int Id_Contribuyente { get; set; }
        [ForeignKey("Id_Contribuyente")]
        public virtual Contribuyente Contribuyente { get; set; }

        public virtual ICollection<Factura_Resolucion_Detalle> Factura_Resolucion_Detalle { get; set; }

    }
}
