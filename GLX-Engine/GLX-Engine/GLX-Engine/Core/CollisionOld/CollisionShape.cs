namespace GLXEngine.Core
{
    public abstract class CollisionShape : Shape
    {
        public GameObject m_parent;

        protected CollisionShape(GameObject a_parent)
        {
            m_parent = a_parent;
        }

        public abstract bool Contains(Vector2 a_point, out Vector2 o_mtv, out Vector2 o_poi);
        public abstract bool Overlaps(Shape a_other, out Vector2 o_mtv, out Vector2 o_poi);

        public abstract void ApplyForce(Vector2 a_force, Vector2 a_poi, out Vector2 o_correctionTransl, out float o_correctionRot);

        public abstract Vector2 GetMaxReach();

        public abstract Vector2[] GetFindPoints();

        public abstract Vector2 ScreenPos();
    }
}
