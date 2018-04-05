using System;
using System.IO;
using System.Linq;
using pb = ProtoBuf;

namespace Hong.Common.Tools
{
    public static class ProtoBufSerializer
    {
        /// <summary>反序列化, 默认使用ProtoBuf序列化器
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object Deserialize(this byte[] data, Type target)
        {
            using (var stream = new MemoryStream(data))
            {
                return pb.Serializer.Deserialize(target, stream);
            }
        }

        /// <summary>反序列化, 默认使用ProtoBuf序列化器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this byte[] data)
        {
            object obj;

            using (var stream = new MemoryStream(data))
            {
                obj = pb.Serializer.Deserialize(typeof(T), stream);
            }

            if (obj == null)
            {
                return default(T);
            }

            return (T)obj;
        }

        /// <summary>序列化, 默认使用ProtoBuf序列化器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(this T value)
        {
            byte[] output = null;
            using (var stream = new MemoryStream())
            {
                pb.Serializer.Serialize(stream, value);
                output = stream.ToArray();
            }

            return output.ToArray();
        }
    }
}