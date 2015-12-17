using System;
using System.IO;

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
    }
}
