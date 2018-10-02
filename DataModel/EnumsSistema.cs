using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataModel
{
    public enum Tipo_documento
    {
        [Description("Factura Electrónica")]
        [NameSpace("https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/facturaElectronica")]       
        Factura_electrónica = 1,
        [Description("Noda Débito")]
        Nota_de_débito_electrónica = 2,
        [NameSpace("https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/notaCreditoElectronica")]
        [Description("Anulación")]
        Nota_de_crédito_electrónica = 3,
        [Description("Tiquete Electrónico")]
        Tiquete_Electrónico = 4,
        [NameSpace("https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/mensajeReceptor")]
        [Description("Confirmación Aceptación")]
        [XmlEnumAttribute("01")]
        Confirmación_aceptación = 5,
        [Description("Confirmación Aceptación Parcial")]
        [NameSpace("https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/mensajeReceptor")]
        [XmlEnumAttribute("02")]
        Confirmación_aceptación_parcial = 6,
        [Description("Rechazo Comprobante")]
        [NameSpace("https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/mensajeReceptor")]
        [XmlEnumAttribute("03")]
        Confirmación_rechazo_comprobante = 7
    }

    public class NameSpaceAttribute : Attribute
    {
        private string description;

        public string Description { get { return description; } }

        public NameSpaceAttribute(string description)
        {
            this.description = description;
        }
    }

    public enum Situación_Comprobante
    {
        Normal = 1,
        Sin_Internet = 2,
        Contingencia = 3
    }

    public enum EstadoComprobante
    {
        //Estdos Hacienda
        [Description("Aceptado")]
        Aceptado = 1,
        [Description("Aceptado Parcialmente")]
        AceptadoParcialmente  = 2,
        [Description("Rechazado")]
        Rechazado = 3,
        //Estados Sistema
        [Description("Enviado")]
        Enviado = 0,
        [Description("Error al Enviar")]
        ErrorEnviar = -1,
        [Description("Anulado")]
        Anulando = -2,
        [Description("Error Anulando")]
        ErrorAnulando = -3,
        [Description("Error de Hacienda")]
        ErrorInternoHacienda = -4,
        [Description("En espera")]
        SinRespuestaDeHacienda = -5,
        [Description("Error sistema")]
        ErrorAlDescomprimirXMLHacienda = -6
    }




    public static class EnumUtils
    {
        public static T GetAttribute<T>(this Enum EnumValue) 
        {
            FieldInfo fi = EnumValue.GetType().GetField(EnumValue.ToString());
            return (T)fi.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        public static string GetXmlValue<T>(this T value) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum) return null;

            MemberInfo member = enumType.GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return null;

            XmlEnumAttribute attribute = member.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault();
            if (attribute == null) return null;
            return attribute.Name;
        }

        public static T SetTypeString<T>(string NewValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception("The T type not is enum");

            string MemberName = null;
            foreach (MemberInfo member in enumType.GetMembers())
            {
                XmlEnumAttribute attribute = member.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    if (attribute.Name == NewValue)
                    {
                        MemberName = member.Name;
                        break;
                    }
                }
            }
            if (MemberName == null)
                MemberName = NewValue;

            return (T)Enum.Parse(typeof(T), MemberName);
        }
    }
}
