using FarseerPhysics.Dynamics;

using Vector2 = OpenTK.Vector2;

namespace Shared.GameObjects
{
    public class PhysicObject : GameObject
    {
        public World World { get; protected set; }

        public Fixture Fixture { get; protected set; }

        public new Vector2 Position
        {
            get { return Fixture.Body.Position.ToTKVector(); }
            set { Fixture.Body.Position = value.ToFPVector(); }
        }

        public Vector2 Velocity
        {
            get { return Fixture.Body.LinearVelocity.ToTKVector(); }
            set { Fixture.Body.LinearVelocity = value.ToFPVector(); }
        }

        public PhysicObject(int id, World world) : base(id)
        {
            World = world;
        }
    }
}
