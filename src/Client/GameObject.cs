using OpenTK;

namespace Client
{
    public class GameObject
    {
        public Vector2 Position = Vector2.Zero;

        protected GameObject() { }

        public virtual void Draw() { }
    }
}
