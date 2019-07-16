using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.EF
{
    [Table("dbo.Contribuyente_ActividadesEconomicas")]
    public class Contribuyente_ActividadesEconomicas
    {
        [Key]
        [JsonProperty("codigo")]
        public int Codigo { set; get; }
        [Column(TypeName = "char")]
        [StringLength(3)]
        [JsonProperty("estado")]
        public string Estado { set; get; }
        [StringLength(250)]
        [JsonProperty("descripcion")]
        public string Descripcion { set; get; }        
    }
}
