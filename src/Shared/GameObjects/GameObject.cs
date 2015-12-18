using OpenTK;

namespace Shared.GameObjects
{
    public class GameObject
    {
        public int Id;
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;

        protected GameObject(int id)
        {
            Id = id;
        }

        public virtual void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
            Velocity *= 0.9f;
        }
    }
}
