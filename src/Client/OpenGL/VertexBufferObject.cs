using System;
using OpenTK.Graphics.OpenGL4;

namespace Client.OpenGL
{
    public sealed class VertexBufferObject : GLObject
    {
        public VertexBufferObject() : base() { }

        protected override void CreateHandle()
        {
            Handle = GL.GenBuffer();
        }

        protected override void DeleteHandle()
        {
            GL.DeleteBuffer(Handle);
        }

        public override void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        }

        public void SetData<T>(T[] data, int elementSize, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * elementSize), data, hint);
        }
    }
}
