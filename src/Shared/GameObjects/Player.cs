using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Shared.GameObjects
{
    public class Player : PhysicObject
    {
        public Player(int id, World world) : base(id, world)
        {
            Vector2 halfSize = new Vector2(63, 90) / 2;

            Body Body = new Body(world);
            Shape Shape = new PolygonShape(new Vertices(new[]
              {
                new Vector2(-halfSize.X, -halfSize.Y),
                new Vector2( halfSize.X, -halfSize.Y),
                new Vector2( halfSize.X,  halfSize.Y),
                new Vector2(-halfSize.X,  halfSize.Y)
            }), 1.0f);
            Fixture = Body.CreateFixture(Shape);
            Body.FixedRotation = true;
            Fixture.Body.IsStatic = false;
        }
    }
}
