using OpenTK.Graphics.OpenGL4;

namespace Client.OpenGL
{
    public sealed class VertexArrayObject : GLObject
    {
        public VertexArrayObject() : base() { }

        protected override void CreateHandle()
        {
            Handle = GL.GenVertexArray();
        }

        protected override void DeleteHandle()
        {
            GL.DeleteVertexArray(Handle);
        }

        public override void Bind()
        {
            GL.BindVertexArray(Handle);
        }

        public void AttachVertexBuffer(int index, VertexBufferObject vertexBuffer, int elementsPerVertex, VertexAttribPointerType ptrType, int stride, int offset)
        {
            Bind();
            vertexBuffer.Bind();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, elementsPerVertex, ptrType, false, stride, offset);
        }

        public void Draw(int start, int count)
        {
            Bind();
            GL.DrawArrays(PrimitiveType.Triangles, start, count);
        }
    }
}
