using TKVector = OpenTK.Vector2;
using FPVector = Microsoft.Xna.Framework.Vector2;

namespace Shared
{
    public static class ConversionExtensions
    {
        public static TKVector ToTKVector(this FPVector vector)
        {
            return new TKVector(vector.X, vector.Y);
        }

        public static FPVector ToFPVector(this TKVector vector)
        {
            return new FPVector(vector.X, vector.Y);
        }
    }
}
