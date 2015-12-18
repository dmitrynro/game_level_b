using System;
using OpenTK.Graphics.OpenGL4;

namespace Client.OpenGL
{
    public sealed class Shader : GLObject
    {
        public ShaderType Type { get; private set; }

        public Shader(ShaderType type, string source, Action<string> loggerCallback = null) : base(false)
        {
            Type = type;

            CreateHandle();

            if (!Compile(source) && loggerCallback != null)
                loggerCallback(GetCompileLog());
        }

        protected override void CreateHandle()
        {
            Handle = GL.CreateShader(Type);
        }

        protected override void DeleteHandle()
        {
            GL.DeleteShader(Handle);
        }

        public override void Bind() { }

        public bool Compile(string source)
        {
            GL.ShaderSource(Handle, source);
            GL.CompileShader(Handle);

            int status;
            GL.GetShader(Handle, ShaderParameter.CompileStatus, out status);

            return status != 0;
        }

        public string GetCompileLog()
        {
            return GL.GetShaderInfoLog(Handle);
        }
    }
}
