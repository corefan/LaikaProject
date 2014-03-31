using System.IO;
using MsgPack.Serialization;

namespace Laika.Serialize.MessagePack
{
    public class MessagePackUtility
    {
        public static byte[] Serialize<T>(T instance)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = MessagePackSerializer.Create<T>();
                serializer.Pack(stream, instance);
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] serializedData)
        {
            using (var stream = new System.IO.MemoryStream(serializedData))
            {
                var serializer = MessagePackSerializer.Create<T>();
                return serializer.Unpack(stream);
            }
        }
    }
}
