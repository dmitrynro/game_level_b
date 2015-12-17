using System;
using Client.OpenGL;
using OpenTK;

namespace Client
{
    public sealed class Sprite
    {
        public Texture Texture;
        public Vector2 UVMin, UVMax;

        public Sprite(Texture texture, Vector2 uvMin, Vector2 uvMax)
        {
            Texture = texture;
            UVMax = uvMax;
            UVMin = uvMin;
        }

        public Sprite(Texture texture) : this(texture, Vector2.Zero, Vector2.One) { }
    }
}
