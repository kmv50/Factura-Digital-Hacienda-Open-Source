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
    }

    public class Impuestos
    {
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
        public static List<Impuestos> Impuestos
        {
            get {
                return new List<Impuestos>()
                {
                    new Impuestos(){ Value = "01" , Text = "Impuesto General sobre las Ventas" },
                    new Impuestos(){ Value = "02" , Text = "Impuesto Selectivo de Consumo" },
                    new Impuestos(){ Value = "03" , Text = "Impuesto Único a los combustibles" },
                    new Impuestos(){ Value = "04" , Text = "Impuesto específico de Bebidas Alcohólicas" },
                    new Impuestos(){ Value = "05" , Text = "Impuesto Específico sobre las bebidas envasadas sin contenido alcohólico y jabones de tocador" },
                    new Impuestos(){ Value = "06" , Text = "Impuesto a los Productos de Tabaco" },
                    new Impuestos(){ Value = "07" , Text = "Servicio" },
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
                new UnidadMedida(){Value = "m" ,Text = "Metro"},
                new UnidadMedida(){Value = "kg" ,Text = "Kilogramo"},
                new UnidadMedida(){Value = "s" ,Text = "Segundo"},
                new UnidadMedida(){Value = "A" ,Text = "Ampere"},
                new UnidadMedida(){Value = "K" ,Text = "Kelvin"},
                new UnidadMedida(){Value = "mol" ,Text = "Mol"},
                new UnidadMedida(){Value = "cd" ,Text = "Candela"},
                new UnidadMedida(){Value = "m²" ,Text = "Metro Cuadrado"},
                new UnidadMedida(){Value = "m³" ,Text = "Metro Cúbico"},
                new UnidadMedida(){Value = "m/s" ,Text = "Metro por Segundo"},
                new UnidadMedida(){Value = "m/s²" ,Text = "Metro por Segundo Cuadrado"},
                new UnidadMedida(){Value = "1/m" ,Text = "1 por Metro"},
                new UnidadMedida(){Value = "kg/m³" ,Text = "Kilogramo por Metro Cúbico"},
                new UnidadMedida(){Value = "A/m²" ,Text = "Ampere por Metro Cuadrado"},
                new UnidadMedida(){Value = "A/m" ,Text = "Ampere por Metro"},
                new UnidadMedida(){Value = "mol/m³" ,Text = "Mol por Metro Cúbico"},
                new UnidadMedida(){Value = "cd/m²" ,Text = "Candela por Metro Cuadrado"},
                new UnidadMedida(){Value = "1" ,Text = "Uno (Índice de Refracción)"},
                new UnidadMedida(){Value = "rad" ,Text = "Radián"},
                new UnidadMedida(){Value = "sr" ,Text = "Estereorradián"},
                new UnidadMedida(){Value = "Hz" ,Text = "Hertz"},
                new UnidadMedida(){Value = "N" ,Text = "Newton"},
                new UnidadMedida(){Value = "Pa" ,Text = "Pascal"},
                new UnidadMedida(){Value = "J" ,Text = "Joule"},
                new UnidadMedida(){Value = "W" ,Text = "Watt"},
                new UnidadMedida(){Value = "C" ,Text = "Coulomb"},
                new UnidadMedida(){Value = "V" ,Text = "Volt"},
                new UnidadMedida(){Value = "F" ,Text = "Farad"},
                new UnidadMedida(){Value = "Ω" ,Text = "Ohm"},
                new UnidadMedida(){Value = "S" ,Text = "Siemens"},
                new UnidadMedida(){Value = "Wb" ,Text = "Weber"},
                new UnidadMedida(){Value = "T" ,Text = "Tesla"},
                new UnidadMedida(){Value = "H" ,Text = "Henry"},
                new UnidadMedida(){Value = "°C" ,Text = "Grado Celsius"},
                new UnidadMedida(){Value = "lm" ,Text = "Lumen"},
                new UnidadMedida(){Value = "lx" ,Text = "Lux"},
                new UnidadMedida(){Value = "Bq" ,Text = "Becquerel"},
                new UnidadMedida(){Value = "Gy" ,Text = "Gray"},
                new UnidadMedida(){Value = "Sv" ,Text = "Sievert"},
                new UnidadMedida(){Value = "kat" ,Text = "Katal"},
                new UnidadMedida(){Value = "Pa·s" ,Text = "Pascal Segundo"},
                new UnidadMedida(){Value = "N·m" ,Text = "Newton Metro"},
                new UnidadMedida(){Value = "N/m" ,Text = "Newton por Metro"},
                new UnidadMedida(){Value = "rad/s" ,Text = "Radián por Segundo"},
                new UnidadMedida(){Value = "rad/s²" ,Text = "Radián por Segundo Cuadrado"},
                new UnidadMedida(){Value = "W/m²" ,Text = "Watt por Metro Cuadrado"},
                new UnidadMedida(){Value = "J/K" ,Text = "Joule por Kelvin"},
                new UnidadMedida(){Value = "J/(kg·K)" ,Text = "Joule por Kilogramo Kelvin"},
                new UnidadMedida(){Value = "J/kg" ,Text = "Joule por Kilogramo"},
                new UnidadMedida(){Value = "W/(m·K)" ,Text = "Watt por Metro Kelvin"},
                new UnidadMedida(){Value = "J/m³" ,Text = "Joule por Metro Cúbico"},
                new UnidadMedida(){Value = "V/m" ,Text = "Volt por Metro"},
                new UnidadMedida(){Value = "C/m³" ,Text = "Coulomb por Metro Cúbico"},
                new UnidadMedida(){Value = "C/m²" ,Text = "Coulomb por Metro Cuadrado"},
                new UnidadMedida(){Value = "F/m" ,Text = "Farad por Metro"},
                new UnidadMedida(){Value = "H/m" ,Text = "Henry por Metro"},
                new UnidadMedida(){Value = "J/mol" ,Text = "Joule por Mol"},
                new UnidadMedida(){Value = "J/(mol·K)" ,Text = "Joule por Mol Kelvin"},
                new UnidadMedida(){Value = "C/kg" ,Text = "Coulomb por Kilogramo"},
                new UnidadMedida(){Value = "Gy/s" ,Text = "Gray por Segundo"},
                new UnidadMedida(){Value = "W/sr" ,Text = "Watt por Estereorradián"},
                new UnidadMedida(){Value = "W/(m²·sr)" ,Text = "Watt por Metro Cuadrado Estereorradián"},
                new UnidadMedida(){Value = "kat/m³" ,Text = "Katal por Metro Cúbico"},
                new UnidadMedida(){Value = "min" ,Text = "Minuto"},
                new UnidadMedida(){Value = "h" ,Text = "Hora"},
                new UnidadMedida(){Value = "d" ,Text = "Día"},
                new UnidadMedida(){Value = "º" ,Text = "Grado"},
                new UnidadMedida(){Value = "L" ,Text = "Litro"},
                new UnidadMedida(){Value = "t" ,Text = "Tonelada"},
                new UnidadMedida(){Value = "Np" ,Text = "Neper"},
                new UnidadMedida(){Value = "B" ,Text = "Bel"},
                new UnidadMedida(){Value = "eV" ,Text = "Electronvolt"},
                new UnidadMedida(){Value = "u" ,Text = "Unidad de Masa Atómica Unificada"},
                new UnidadMedida(){Value = "ua" ,Text = "Unidad Astronómica"},
                new UnidadMedida(){Value = "Unid" ,Text = "Unidad"},
                new UnidadMedida(){Value = "Gal" ,Text = "Galón"},
                new UnidadMedida(){Value = "g" ,Text = "Gramo"},
                new UnidadMedida(){Value = "Km" ,Text = "Kilometro"},
                new UnidadMedida(){Value = "ln" ,Text = "Pulgada"},
                new UnidadMedida(){Value = "cm" ,Text = "Centímetro"},
                new UnidadMedida(){Value = "mL" ,Text = "Mililitro"},
                new UnidadMedida(){Value = "mm" ,Text = "Milímetro"},
                new UnidadMedida(){Value = "Oz" ,Text = "Onzas"},
                new UnidadMedida(){Value = "Sp" ,Text = "Servicios Profesionales"},
                new UnidadMedida(){Value = "Otros" ,Text = "Otros"},
            };

                return lst.OrderBy(q => q.Text).ToList();
            }
        }
    }

}
