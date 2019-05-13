namespace GLXEngine.Core
{
    public abstract class CollisionShape : Shape
    {
        public GameObject m_parent;

        protected CollisionShape(GameObject a_parent)
        {
            m_parent = a_parent;
        }

        public abstract bool Contains(Vector2 a_point, out Vector2 o_mtv);
        public abstract bool Overlaps(Shape a_other, out Vector2 o_mtv);

        public abstract float GetMaxReach();

        public abstract Vector2 ScreenPos();
    }
}
