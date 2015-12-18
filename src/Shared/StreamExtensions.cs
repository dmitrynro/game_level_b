using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;

namespace Shared
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, byte[] array)
        {
            stream.Write(array, 0, array.Length);
        }

        public static void Write(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream stream, float value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream stream, Vector2 value)
        {
            stream.Write(BitConverter.GetBytes(value.X));
            stream.Write(BitConverter.GetBytes(value.Y));
        }

        private static readonly Dictionary<Type, Action<Stream, object>> WriteRoute = new Dictionary<Type, Action<Stream, object>>
        {
            { typeof(byte), (s, o) => { s.WriteByte((byte)o); } },
            { typeof(byte[]), (s, o) => { s.Write((byte[])o); } },
            { typeof(int), (s, o) => { s.Write((int)o); } },
            { typeof(float), (s, o) => { s.Write((float)o); } },
            { typeof(Vector2), (s, o) => { s.Write((Vector2)o); } },
        };

        public static void Write(this Stream stream, params object[] args)
        {
            foreach (var arg in args)
            {
                Type type = arg.GetType();

                if (WriteRoute.ContainsKey(type))
                    WriteRoute[type](stream, arg);
                else
                    throw new ArgumentException("Type " + type + " not supported", "args");
            }
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        public static int ReadInt32(this Stream stream)
        {
            return BitConverter.ToInt32(stream.ReadBytes(4), 0);
        }

        public static float ReadSingle(this Stream stream)
        {
            return BitConverter.ToSingle(stream.ReadBytes(4), 0);
        }

        public static Vector2 ReadVector2(this Stream stream)
        {
            float x = stream.ReadSingle();
            float y = stream.ReadSingle();
            return new Vector2(x, y);
        }
    }
}
