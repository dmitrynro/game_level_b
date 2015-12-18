using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using OpenTK;

using FPVector2 = Microsoft.Xna.Framework.Vector2;

namespace Shared.GameObjects
{
    public class Platform : PhysicObject
    {
        public Vector2 Size { get; protected set; }

        public Platform(int id, World world, Vector2 position, Vector2 size) : base(id, world)
        {
            Size = size;

            FPVector2 halfSize = size.ToFPVector() / 2;

            Body Body = new Body(world, position.ToFPVector());
            Shape Shape = new PolygonShape(new Vertices(new[]
              {
                new FPVector2(-halfSize.X, -halfSize.Y),
                new FPVector2( halfSize.X, -halfSize.Y),
                new FPVector2( halfSize.X,  halfSize.Y),
                new FPVector2(-halfSize.X,  halfSize.Y)
            }), 1.0f);
            Fixture = Body.CreateFixture(Shape);
            Fixture.Body.IsStatic = true;
        }
    }
}
