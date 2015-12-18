using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Client.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Client
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexP2T2
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public const int Size = 4 * sizeof(float);

        public const int PositionOffset = 0 * sizeof(float);
        public const int TexCoordOffset = 2 * sizeof(float);

        public VertexP2T2(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }

    public sealed class Renderer
    {
        public Renderer()
        {
            GL.ClearColor(Color4.Black);

            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
        }

        public void Resize(int w, int h)
        {
            GL.Viewport(0, 0, w, h);
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private Dictionary<Texture, List<VertexP2T2>> VertexLists = new Dictionary<Texture, List<VertexP2T2>>();
        private ShaderProgram Shader;

        public void BeginPass(ShaderProgram shader)
        {
            VertexLists.Clear();
            Shader = shader;
        }

        public void DrawRectangle(Vector2 position, Vector2 size, Sprite sprite)
        {
            if (!VertexLists.ContainsKey(sprite.Texture))
                VertexLists[sprite.Texture] = new List<VertexP2T2>(6);

            VertexLists[sprite.Texture].AddRange(new[]
            {
                new VertexP2T2(position, sprite.UVMin),
                new VertexP2T2(new Vector2(position.X + size.X, position.Y), new Vector2(sprite.UVMax.X, sprite.UVMin.Y)),
                new VertexP2T2(new Vector2(position.X + size.X, position.Y + size.Y), new Vector2(sprite.UVMax.X, sprite.UVMax.Y)),

                new VertexP2T2(position, sprite.UVMin),
                new VertexP2T2(new Vector2(position.X + size.X, position.Y + size.Y), new Vector2(sprite.UVMax.X, sprite.UVMax.Y)),
                new VertexP2T2(new Vector2(position.X, position.Y + size.Y), new Vector2(sprite.UVMin.X, sprite.UVMax.Y))
            });
        }

        public void EndPass()
        {
            Shader.Bind();

            VertexP2T2[] data = VertexLists.Values.SelectMany(list => list).ToArray();

            if (data.Length > 0)
                using (var VertexBuffer = new VertexBufferObject())
                using (VertexArrayObject VertexArray = new VertexArrayObject())
                {
                    VertexBuffer.SetData(data, VertexP2T2.Size, BufferUsageHint.DynamicDraw);

                    VertexArray.AttachVertexBuffer(0, VertexBuffer, 2, VertexAttribPointerType.Float, VertexP2T2.Size, VertexP2T2.PositionOffset);
                    VertexArray.AttachVertexBuffer(1, VertexBuffer, 2, VertexAttribPointerType.Float, VertexP2T2.Size, VertexP2T2.TexCoordOffset);

                    int listOffset = 0;

                    foreach (var kv in VertexLists)
                    {
                        kv.Key.Bind();
                        VertexArray.Draw(listOffset, kv.Value.Count + listOffset);
                        listOffset += kv.Value.Count;
                    }
                }

            GL.Flush();
        }
    }
}
