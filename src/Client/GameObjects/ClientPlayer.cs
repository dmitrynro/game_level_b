using Client.OpenGL;
using OpenTK;
using Shared.GameObjects;

namespace Client.GameObjects
{
    public sealed class ClientPlayer : Player, IDrawable
    {
        public ClientPlayer(int id) : base(id) { }

        public void Draw()
        {
            Vector2 size = new Vector2(63, 90);
            Vector2 position = Position - size / 2 + new Vector2(Program.Window.Width / 2, Program.Window.Height / 2);
            Program.Renderer.DrawRectangle(position, size, new Sprite(Program.Content.Get<Texture>("SusekaTexture")));
        }
    }
}
