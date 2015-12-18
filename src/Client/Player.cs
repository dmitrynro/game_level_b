using Client.OpenGL;
using OpenTK;

namespace Client
{
    public class Player : GameObject
    {
        public override void Draw()
        {
            Vector2 size = new Vector2(63, 90);
            Program.Renderer.DrawRectangle(Position - size / 2, size, new Sprite(Program.Content.Get<Texture>("SusekaTexture")));
        }
    }
}
