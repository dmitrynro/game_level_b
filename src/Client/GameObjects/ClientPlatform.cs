using FarseerPhysics.Dynamics;
using OpenTK;
using Shared.GameObjects;

namespace Client.GameObjects
{
    public sealed class ClientPlatform : Platform, IDrawable
    {
        public ClientPlatform(int id, World world, Vector2 position, Vector2 size) : base(id, world, position, size) { }

        public void Draw()
        {
            Vector2 position = Position - Size / 2 + new Vector2(Program.Window.Width / 2, Program.Window.Height / 2);

            for (int i = 0; i < Size.X / 16; i++)
                for (int j = 0; j < Size.Y / 16; j++)
                    Program.Renderer.DrawRectangle(position + new Vector2(i, j) * 16, new Vector2(16, 16), Program.Content.Get<Sprite>("Sprites\\Brick"));
        }
    }
}
