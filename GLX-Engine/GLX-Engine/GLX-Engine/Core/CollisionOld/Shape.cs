
namespace GLXEngine.Core
{
    public abstract class Shape : Transformable
    {
        public abstract bool Contains(Vector2 a_point);

        public abstract bool Overlaps(Shape a_other);
    }
}
