using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Definitif
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Serializes object to JSON representation.
        /// </summary>
        /// <returns>JSON representation of object.</returns>
        public static string ToJson(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(obj.GetType());
                ser.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserializes string to object of given type.
        /// </summary>
        /// <typeparam name="T">Type to deserealize.</typeparam>
        /// <returns>Deserialized object.</returns>
        public static T FromJson<T>(this string str)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(str.ToCharArray())))
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }
    }
}
