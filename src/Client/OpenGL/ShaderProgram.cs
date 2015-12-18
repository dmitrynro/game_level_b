using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Client.OpenGL
{
    public sealed class ShaderProgram : GLObject
    {
        public ShaderProgram(IEnumerable<Shader> shaders, Action<string> loggerCallback = null) : base()
        {
            if (!Link(shaders.ToArray()) && loggerCallback != null)
                loggerCallback(GetLinkLog());
        }

        protected override void CreateHandle()
        {
            Handle = GL.CreateProgram();
        }

        protected override void DeleteHandle()
        {
            GL.DeleteProgram(Handle);
        }

        public override void Bind()
        {
            GL.UseProgram(Handle);
        }

        public bool Link(params Shader[] shaders)
        {
            foreach (var shader in shaders)
                GL.AttachShader(Handle, shader.Handle);

            GL.LinkProgram(Handle);

            int status;
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out status);

            return status != 0;
        }

        public string GetLinkLog()
        {
            return GL.GetProgramInfoLog(Handle);
        }

        private Dictionary<string, int> UniformCache = new Dictionary<string, int>();

        public int GetUniformLocation(string name)
        {
            if (UniformCache.ContainsKey(name))
                return UniformCache[name];
            else
                return UniformCache[name] = GL.GetUniformLocation(Handle, name);
        }

        private static readonly Dictionary<Type, Action<int, object>> SetUniformRoute = new Dictionary<Type, Action<int, object>>
        {
            { typeof(int), (l, o) => GL.Uniform1(l, (int)o) },
            { typeof(float), (l, o) => GL.Uniform1(l, (float)o) },
            { typeof(double), (l, o) => GL.Uniform1(l, (double)o) },

            { typeof(Vector2), (l, o) => GL.Uniform2(l, (Vector2)o) },
            { typeof(Vector3), (l, o) => GL.Uniform3(l, (Vector3)o) },
            { typeof(Vector4), (l, o) => GL.Uniform4(l, (Vector4)o) },

            { typeof(Color4), (l, o) => GL.Uniform4(l, (Color4)o) },

            { typeof(Matrix4), (l, o) =>
                {
                    Matrix4 m = (Matrix4)o;
                    GL.UniformMatrix4(l, true, ref m);
                }
            },
        };

        public void SetUniform(int location, object value)
        {
            Bind();

            Type type = value.GetType();

            if (SetUniformRoute.ContainsKey(type))
                SetUniformRoute[type](location, value);
            else
                throw new ArgumentException("Type " + type + " not supported", "value");
        }

        public void SetUniform(string name, object value)
        {
            SetUniform(GetUniformLocation(name), value);
        }
    }
}
