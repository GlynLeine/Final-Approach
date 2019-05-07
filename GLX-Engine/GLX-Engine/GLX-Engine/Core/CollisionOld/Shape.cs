
namespace GLXEngine.Core
{
    public abstract class Shape
    {
        public Vector2 position = new Vector2();

        public float x { get { return position.x; } set { position.x = value; } }
        public float y { get { return position.y; } set { position.y = value; } }

        public abstract bool Contains(Vector2 a_point, out Vector2 o_mtv);

        public abstract bool Overlaps(Shape a_other, out Vector2 o_mtv);
    }
}
