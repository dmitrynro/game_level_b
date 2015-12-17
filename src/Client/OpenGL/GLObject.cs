using System;
using OpenTK.Graphics;

namespace Client.OpenGL
{
    public class GLObject : IDisposable
    {
        public int Handle { get; protected set; } = -1;

        protected GLObject(bool createHandle = true)
        {
            if (createHandle)
                CreateHandle();
        }

        protected virtual void CreateHandle() { }

        protected virtual void DeleteHandle() { }

        public virtual void Bind() { }

        public override string ToString()
        {
            return Handle.ToString();
        }

        public bool Disposed { get; protected set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
                    DeleteHandle();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLObject()
        {
            Dispose(false);
        }
    }
}
