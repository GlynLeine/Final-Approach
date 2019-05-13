using System.Collections.Generic;
using System;

namespace GLXEngine.Core
{
    public class Collider
    {
        public Vector2 m_minimumTranslationVec = new Vector2();

        public List<CollisionShape> m_shapes = new List<CollisionShape>();

        public List<Type> m_ignoreList = new List<Type>();

        public GameObject m_owner;

        private Vector2 m_position;
        private float m_rotation;

        public Vector2 position
        {
            get
            {
                return m_position;
            }
            set
            {
                Vector2 trans = value - m_position;
                foreach (CollisionShape shape in m_shapes)
                    shape.position += trans;
            }
        }

        public float rotation
        {
            get
            {
                return m_rotation;
            }
            set
            {
                float rot = value - m_rotation;
                foreach (CollisionShape shape in m_shapes)
                {
                    shape.position = shape.position.Rotate(rot);
                    shape.rotation += rot;
                }
            }
        }

        protected Collider() { }

        public Collider(GameObject a_owner)
        {
            m_owner = a_owner;
        }

        public Collider(GameObject a_owner, BoundsObject a_source)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_source.GetBounds());
        }

        public Collider(GameObject a_owner, CollisionShape a_shape)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_shape);
        }

        public Collider(GameObject a_owner, CollisionShape[] a_collisionShapes)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>(a_collisionShapes);
        }

        public Collider(GameObject a_owner, List<CollisionShape> a_collisionShapes)
        {
            m_owner = a_owner;
            m_shapes = a_collisionShapes;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public virtual bool HitTest(ref Collider other)
        {
            m_minimumTranslationVec = new Vector2();
            if (m_ignoreList != null)
                if (m_ignoreList.Contains(other.m_owner.GetType()))
                    return false;

            if (other.m_ignoreList != null)
                if (other.m_ignoreList.Contains(m_owner.GetType()))
                    return false;

            m_minimumTranslationVec = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 tempMtv;
            bool ret = false;
            foreach (CollisionShape shape in m_shapes)
                foreach (CollisionShape otherShape in other.m_shapes)
                {
                    if (shape.Overlaps(otherShape, out tempMtv))
                    {
                        if (tempMtv.sqrMagnitude < m_minimumTranslationVec.sqrMagnitude)
                            m_minimumTranslationVec = tempMtv;
                        ret = true;
                    }
                }
            if (!ret)
                m_minimumTranslationVec = new Vector2();
            return ret;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------	
        public bool HitTestPoint(float x, float y)
        {
            return HitTestPoint(new Vector2(x, y));
        }

        public virtual bool HitTestPoint(Vector2 a_point)
        {
            foreach (Shape shape in m_shapes)
                if (shape.Contains(a_point))
                    return true;
            return false;
        }

        public virtual Shape BroadPhase()
        {
            float maxReach = 0;
            foreach (CollisionShape collisionShape in m_shapes)
            {
                float reach = collisionShape.position.magnitude + collisionShape.GetMaxReach();
                if (reach > maxReach)
                    maxReach = reach;
            }

            Vector2 center = m_owner.position + m_owner.m_velocity * 0.5f;
            float radius = maxReach * 2 + m_owner.m_velocity.magnitude + 64;

            Game.main.UI.Ellipse(center.x - radius * 0.5f, center.y - radius * 0.5f, radius, radius);

            return new Circle(center.x, center.y, radius, null);
        }
    }
}

