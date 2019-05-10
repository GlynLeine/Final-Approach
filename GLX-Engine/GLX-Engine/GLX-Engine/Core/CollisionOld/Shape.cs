
namespace GLXEngine.Core
{
    public abstract class Shape : GameObject
    {
        public abstract bool Contains(Vector2 a_point, out Vector2 o_mtv);
        public abstract bool Contains(Vector2 a_point);

        public abstract bool Overlaps(Shape a_other, out Vector2 o_mtv);
        public abstract bool Overlaps(Shape a_other);
    }
}
