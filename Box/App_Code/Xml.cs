using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Definitif
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Serializes object to XML representation.
        /// </summary>
        /// <returns>XML representation of object.</returns>
        public static string ToXml(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var ser = new DataContractSerializer(obj.GetType());
                ser.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserializes string to object of given type.
        /// </summary>
        /// <typeparam name="T">Type to deserealize.</typeparam>
        /// <returns>Deserialized object.</returns>
        public static T FromXml<T>(this string str)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(str.ToCharArray())))
            {
                var ser = new DataContractSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }
    }
}
