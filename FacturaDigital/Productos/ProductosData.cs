using DataModel.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaDigital.Productos
{
    public class UnidadMedida
    {
        public string Text { set; get; }
        public string Value { set; get; }
        public bool EsServicio { set; get; }
    }

    public class Impuestos
    {
        public string Text { set; get; }
        public string Value { set; get; }
    }

    public class TarifaCodigo {
        public string Text { set; get; }
        public string Value { set; get; }
        public decimal Tarifa { set; get; }
    }

    public class TiposDocumentosExoneracion {
        public string Text { set; get; }
        public string Value { set; get; }
    }


    public class Producto_ImpuestoSeleccionado : Producto_Impuesto
    {
        public string Nombre { get; internal set; }
        public decimal Monto { get; internal set; }
    }


    public static class ProductosData
    {
        public static List<TiposDocumentosExoneracion> TiposDocumentosExoneracion {
            get {
                return new List<TiposDocumentosExoneracion>()
                {
                    new TiposDocumentosExoneracion(){Text = "Seleccionar" , Value = null },
                    new TiposDocumentosExoneracion(){Text = "Compras autorizadas" , Value = "01" },
                    new TiposDocumentosExoneracion(){Text = "Ventas exentas a diplomáticos" , Value = "02" },
                    new TiposDocumentosExoneracion(){Text = "Autorizado por Ley especial" , Value = "03" },
                    new TiposDocumentosExoneracion(){Text = "Exenciones Dirección General de Hacienda" , Value = "04" },
                    new TiposDocumentosExoneracion(){Text = "Transitorio V" , Value = "06" },
                    new TiposDocumentosExoneracion(){Text = "Transitorio IX" , Value = "06" },
                    new TiposDocumentosExoneracion(){Text = "Transitorio XVII" , Value = "07" },
                    new TiposDocumentosExoneracion(){Text = "Otros" , Value = "99" },

                };
            }
        }

        public static List<TarifaCodigo> TarifaCodigo
        {
            get{
                return new List<TarifaCodigo>() {
                    new TarifaCodigo(){ Text = "Tarifa 0% (Exento)" , Value = "01"  , Tarifa = 0},
                    new TarifaCodigo(){ Text = "Tarifa reducida 1%" , Value = "02" , Tarifa = 1},
                    new TarifaCodigo(){ Text = "Tarifa reducida 2%" , Value = "03" , Tarifa = 2},
                    new TarifaCodigo(){ Text = "Tarifa reducida 4%" , Value = "04" , Tarifa = 4},
                    new TarifaCodigo(){ Text = "Transitorio 4%" , Value = "06" , Tarifa = 4},
                    new TarifaCodigo(){ Text = "Transitorio 0%" , Value = "05" , Tarifa = 0},
                    new TarifaCodigo(){ Text = "Transitorio 8%" , Value = "07" , Tarifa = 8},
                    new TarifaCodigo(){ Text = "Tarifa general 13%" , Value = "08" , Tarifa = 13},
                };
            }
        }

        public static List<Impuestos> Impuestos
        {
            get {
                return new List<Impuestos>()
                {
                    new Impuestos(){ Value = "01" , Text = "Impuesto al Valor Agregado" },
                    new Impuestos(){ Value = "02" , Text = "Impuesto Selectivo de Consumo" },
                    new Impuestos(){ Value = "03" , Text = "Impuesto Único a los combustibles" },
                    new Impuestos(){ Value = "04" , Text = "Impuesto específico de Bebidas Alcohólicas" },
                    new Impuestos(){ Value = "05" , Text = "Impuesto Específico sobre las bebidas envasadas sin contenido alcohólico y jabones de tocador" },
                    new Impuestos(){ Value = "06" , Text = "Impuesto a los Productos de Tabaco" },
                    //new Impuestos(){ Value = "07" , Text = "IVA (cálculo especial)" },
                    //new Impuestos(){ Value = "08" , Text = "IVA Régimen de Bienes Usados (Factor)" },
                    new Impuestos(){ Value = "12" , Text = "Impuesto Especifico al Cemento" },
                    new Impuestos(){ Value = "98" , Text = "Otros" }
                };
            }
        }

        public static List<UnidadMedida> UnidadesMedida
        {
            get
            {

            List<UnidadMedida> lst = new List<UnidadMedida>()
            {
                new UnidadMedida(){Value = "m" ,Text = "Metro", EsServicio = false},
                new UnidadMedida(){Value = "kg" ,Text = "Kilogramo", EsServicio = false},
                new UnidadMedida(){Value = "s" ,Text = "Segundo", EsServicio = true},
                new UnidadMedida(){Value = "A" ,Text = "Ampere", EsServicio = false},
                new UnidadMedida(){Value = "K" ,Text = "Kelvin", EsServicio = false},
                new UnidadMedida(){Value = "mol" ,Text = "Mol", EsServicio = false},
                new UnidadMedida(){Value = "cd" ,Text = "Candela", EsServicio = false},
                new UnidadMedida(){Value = "m²" ,Text = "Metro Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "m³" ,Text = "Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "m/s" ,Text = "Metro por Segundo", EsServicio = false},
                new UnidadMedida(){Value = "m/s²" ,Text = "Metro por Segundo Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "1/m" ,Text = "1 por Metro", EsServicio = false},
                new UnidadMedida(){Value = "kg/m³" ,Text = "Kilogramo por Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "A/m²" ,Text = "Ampere por Metro Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "A/m" ,Text = "Ampere por Metro", EsServicio = false},
                new UnidadMedida(){Value = "mol/m³" ,Text = "Mol por Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "cd/m²" ,Text = "Candela por Metro Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "1" ,Text = "Uno (Índice de Refracción)", EsServicio = false},
                new UnidadMedida(){Value = "rad" ,Text = "Radián", EsServicio = false},
                new UnidadMedida(){Value = "sr" ,Text = "Estereorradián", EsServicio = false},
                new UnidadMedida(){Value = "Hz" ,Text = "Hertz", EsServicio = false},
                new UnidadMedida(){Value = "N" ,Text = "Newton", EsServicio = false},
                new UnidadMedida(){Value = "Pa" ,Text = "Pascal", EsServicio = false},
                new UnidadMedida(){Value = "J" ,Text = "Joule", EsServicio = false},
                new UnidadMedida(){Value = "W" ,Text = "Watt", EsServicio = false},
                new UnidadMedida(){Value = "C" ,Text = "Coulomb", EsServicio = false},
                new UnidadMedida(){Value = "V" ,Text = "Volt", EsServicio = false},
                new UnidadMedida(){Value = "F" ,Text = "Farad", EsServicio = false},
                new UnidadMedida(){Value = "Ω" ,Text = "Ohm", EsServicio = false},
                new UnidadMedida(){Value = "S" ,Text = "Siemens", EsServicio = false},
                new UnidadMedida(){Value = "Wb" ,Text = "Weber", EsServicio = false},
                new UnidadMedida(){Value = "T" ,Text = "Tesla", EsServicio = false},
                new UnidadMedida(){Value = "H" ,Text = "Henry", EsServicio = false},
                new UnidadMedida(){Value = "°C" ,Text = "Grado Celsius", EsServicio = false},
                new UnidadMedida(){Value = "lm" ,Text = "Lumen", EsServicio = false},
                new UnidadMedida(){Value = "lx" ,Text = "Lux", EsServicio = false},
                new UnidadMedida(){Value = "Bq" ,Text = "Becquerel", EsServicio = false},
                new UnidadMedida(){Value = "Gy" ,Text = "Gray", EsServicio = false},
                new UnidadMedida(){Value = "Sv" ,Text = "Sievert", EsServicio = false},
                new UnidadMedida(){Value = "kat" ,Text = "Katal", EsServicio = false},
                new UnidadMedida(){Value = "Pa·s" ,Text = "Pascal Segundo", EsServicio = false},
                new UnidadMedida(){Value = "N·m" ,Text = "Newton Metro", EsServicio = false},
                new UnidadMedida(){Value = "N/m" ,Text = "Newton por Metro", EsServicio = false},
                new UnidadMedida(){Value = "rad/s" ,Text = "Radián por Segundo", EsServicio = false},
                new UnidadMedida(){Value = "rad/s²" ,Text = "Radián por Segundo Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "W/m²" ,Text = "Watt por Metro Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "J/K" ,Text = "Joule por Kelvin", EsServicio = false},
                new UnidadMedida(){Value = "J/(kg·K)" ,Text = "Joule por Kilogramo Kelvin", EsServicio = false},
                new UnidadMedida(){Value = "J/kg" ,Text = "Joule por Kilogramo", EsServicio = false},
                new UnidadMedida(){Value = "W/(m·K)" ,Text = "Watt por Metro Kelvin", EsServicio = false},
                new UnidadMedida(){Value = "J/m³" ,Text = "Joule por Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "V/m" ,Text = "Volt por Metro", EsServicio = false},
                new UnidadMedida(){Value = "C/m³" ,Text = "Coulomb por Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "C/m²" ,Text = "Coulomb por Metro Cuadrado", EsServicio = false},
                new UnidadMedida(){Value = "F/m" ,Text = "Farad por Metro", EsServicio = false},
                new UnidadMedida(){Value = "H/m" ,Text = "Henry por Metro", EsServicio = false},
                new UnidadMedida(){Value = "J/mol" ,Text = "Joule por Mol", EsServicio = false},
                new UnidadMedida(){Value = "J/(mol·K)" ,Text = "Joule por Mol Kelvin", EsServicio = false},
                new UnidadMedida(){Value = "C/kg" ,Text = "Coulomb por Kilogramo", EsServicio = false},
                new UnidadMedida(){Value = "Gy/s" ,Text = "Gray por Segundo", EsServicio = false},
                new UnidadMedida(){Value = "W/sr" ,Text = "Watt por Estereorradián", EsServicio = false},
                new UnidadMedida(){Value = "W/(m²·sr)" ,Text = "Watt por Metro Cuadrado Estereorradián", EsServicio = false},
                new UnidadMedida(){Value = "kat/m³" ,Text = "Katal por Metro Cúbico", EsServicio = false},
                new UnidadMedida(){Value = "min" ,Text = "Minuto", EsServicio = true},
                new UnidadMedida(){Value = "h" ,Text = "Hora", EsServicio = true},
                new UnidadMedida(){Value = "d" ,Text = "Día", EsServicio = true},
                new UnidadMedida(){Value = "º" ,Text = "Grado", EsServicio = false},
                new UnidadMedida(){Value = "L" ,Text = "Litro", EsServicio = false},
                new UnidadMedida(){Value = "t" ,Text = "Tonelada", EsServicio = false},
                new UnidadMedida(){Value = "Np" ,Text = "Neper", EsServicio = false},
                new UnidadMedida(){Value = "B" ,Text = "Bel", EsServicio = false},
                new UnidadMedida(){Value = "eV" ,Text = "Electronvolt", EsServicio = false},
                new UnidadMedida(){Value = "u" ,Text = "Unidad de Masa Atómica Unificada", EsServicio = false},
                new UnidadMedida(){Value = "ua" ,Text = "Unidad Astronómica", EsServicio = false},
                new UnidadMedida(){Value = "Unid" ,Text = "Unidad", EsServicio = false},
                new UnidadMedida(){Value = "Gal" ,Text = "Galón", EsServicio = false},
                new UnidadMedida(){Value = "g" ,Text = "Gramo", EsServicio = false},
                new UnidadMedida(){Value = "Km" ,Text = "Kilometro", EsServicio = false},
                new UnidadMedida(){Value = "ln" ,Text = "Pulgada", EsServicio = false},
                new UnidadMedida(){Value = "cm" ,Text = "Centímetro", EsServicio = false},
                new UnidadMedida(){Value = "mL" ,Text = "Mililitro", EsServicio = false},
                new UnidadMedida(){Value = "mm" ,Text = "Milímetro", EsServicio = false},
                new UnidadMedida(){Value = "Oz" ,Text = "Onzas", EsServicio = false},
                new UnidadMedida(){Value = "Os", Text = "Otro tipo de servicio", EsServicio = true},
                new UnidadMedida(){Value = "Sp", Text = "Servicios Profesionales", EsServicio = true},
                new UnidadMedida(){Value = "Spe", Text = "Servicios personales", EsServicio = true},
                new UnidadMedida(){Value = "St", Text = "Servicios técnicos", EsServicio = true},
                new UnidadMedida(){Value = "Otros" ,Text = "Otros", EsServicio = false},
            };

                return lst.OrderBy(q => q.Text).ToList();
            }
        }
    }

}
