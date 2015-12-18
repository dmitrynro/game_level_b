using FarseerPhysics.Dynamics;
using OpenTK;
using Shared.GameObjects;

namespace Client.GameObjects
{
    public sealed class ClientPlayer : Player, IDrawable
    {
        public ClientPlayer(int id, World world) : base(id, world) { }

        public void Draw()
        {
            Vector2 size = new Vector2(63, 90);
            Vector2 position = Position - size / 2 + new Vector2(Program.Window.Width / 2, Program.Window.Height / 2);
            Program.Renderer.DrawRectangle(position, size, Program.Content.Get<Sprite>("Sprites\\Player"));
        }
    }
}
