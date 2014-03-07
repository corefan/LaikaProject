using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtoBuf;

namespace Laika.Serialize.ProtobufNet
{
    /// <summary>
    /// Proto.Net utility
    /// </summary>
    public class ProtobufNetUtility
    {
        /// <summary>
        /// serialize
        /// </summary>
        /// <typeparam name="T">Serialize Type</typeparam>
        /// <param name="instance">instance</param>
        /// <returns>bytes data</returns>
        public static byte[] Serialize<T>(T instance)
        {
            if (instance == null)
                return null;

            using (MemoryStream inputStream = new MemoryStream())
            {
                Serializer.Serialize<T>(inputStream, instance);
                return inputStream.ToArray();
            }
        }
        /// <summary>
        /// deserialize
        /// </summary>
        /// <typeparam name="T">Deserialize Type</typeparam>
        /// <param name="serializedData">serialized data</param>
        /// <returns>instance</returns>
        public static T Deserialize<T>(byte[] serializedData)
        {
            if (serializedData == null)
                return default(T);

            using (MemoryStream outputStream = new MemoryStream(serializedData))
            {
                return Serializer.Deserialize<T>(outputStream);
            }
        }
        /// <summary>
        /// Get proto string
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>string</returns>
        public static string GetProtoString<T>()
        {
            return Serializer.GetProto<T>();
        }
    }
}
