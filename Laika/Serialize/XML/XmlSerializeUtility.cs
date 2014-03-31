using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Laika.Serialize.XML
{
    /// <summary>
    /// Simple xml serialize, deserialize
    /// </summary>
    public class SimpleXmlSerializeUtility
    {
        /// <summary>
        /// serialize
        /// </summary>
        /// <param name="instance">instance</param>
        /// <returns>string</returns>
        public static string Serialize(object instance)
        {
            XmlSerializer x = new XmlSerializer(instance.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter writer = XmlTextWriter.Create(ms))
                {
                    x.Serialize(writer, instance);

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        /// <summary>
        /// deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="xml">xml string</param>
        /// <returns>instance</returns>
        public static T Deserialize<T>(string xml)
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                using (StreamReader reader = new StreamReader(ms))
                {
                    return (T)x.Deserialize(reader);
                }
            }
        }
    }
}
