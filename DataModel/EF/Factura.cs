using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Facturas")]
    public partial class Factura
    {
        public Factura() {
            Factura_Detalle = new HashSet<Factura_Detalle>();
        }

        [Key]
        public int Id_Factura { get; set; }

        public int Estado { get; set; }

        [Required]
        [StringLength(50)]
        public string Clave { get; set; }

        public int CasaMatriz { set; get; }

        public int PuntoVenta { set; get; }

        public int NumeroConsecutivo { get; set; }

        public DateTime Fecha_Emision_Documento { get; set; } 

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

        public int? Emisor_Ubicacion_Provincia { get; set; }

        public int? Emisor_Ubicacion_Canton { get; set; }

        public int? Emisor_Ubicacion_Distrito { get; set; }

        public int? Emisor_Ubicacion_Barrio { get; set; }

        [StringLength(160)]
        public string Emisor_Ubicacion_OtrasSenas { get; set; }

        public int? Emisor_Telefono_Codigo { get; set; }

        public int? Emisor_Telefono_Numero { get; set; }

        [Required]
        [StringLength(60)]
        public string Emisor_CorreoElectronico { get; set; }

        [Required]
        [StringLength(80)]
        public string Receptor_Nombre { get; set; }

        [Column(TypeName = "char")]
        [StringLength(2)]
        public string Receptor_Identificacion_Tipo { get; set; }

        [StringLength(20)]
        public string Receptor_Identificacion_Numero { get; set; }

        [StringLength(80)]
        public string Receptor_NombreComercial { get; set; }

        public int? Receptor_Ubicacion_Provincia { get; set; }

        public int? Receptor_Ubicacion_Canton { get; set; }

        public int? Receptor_Ubicacion_Distrito { get; set; }

        public int? Receptor_Ubicacion_Barrio { get; set; }

        [StringLength(160)]
        public string Receptor_Ubicacion_OtrasSenas { get; set; }

        public int? Receptor_Telefono_Codigo { get; set; }

        public int? Receptor_Telefono_Numero { get; set; }


        [StringLength(60)]
        public string Receptor_CorreoElectronico { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [StringLength(2)]
        public string CondicionVenta { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [StringLength(2)]
        public string MedioPago { get; set; }

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

        public int Id_TipoDocumento { get; set; }

        public bool Email_Enviado { set; get; }

        [Column(TypeName = "text")]
        [StringLength(16777215)]
        public string XML_Enviado { get; set; }

        [Column(TypeName = "text")]
        [StringLength(16777215)]
        public string XML_Respuesta { get; set; }

        [StringLength(2500)]
        public string Log_Envio_Api { set; get; }

        [StringLength(2500)]
        public string Ultimo_Log_Consulta_Api { set; get; }


        public int Id_Contribuyente { get; set; }
        [ForeignKey("Id_Contribuyente")]
        public virtual Contribuyente Contribuyente { get; set; }

        public virtual ICollection<Factura_Detalle> Factura_Detalle { get; set; }

    }
}
