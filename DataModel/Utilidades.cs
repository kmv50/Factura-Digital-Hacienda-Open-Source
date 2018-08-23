using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataModel
{
    public interface ISerializeToXML { }
    public static class SerializeToXMLExtencion
    {
        public static XmlDocument SerializeToXML<T>(this ISerializeToXML p , object obj, string defNamespace = "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/facturaElectronica")
        {
            try
            {
                var serialiseToDocument = new XmlDocument();
                serialiseToDocument.PreserveWhitespace = true;
                var serializer = new XmlSerializer(typeof(T), defNamespace);
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, obj);
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    serialiseToDocument.Load(stream);
                }
                return serialiseToDocument;
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                return null;
            }
        }
    }
    
}
