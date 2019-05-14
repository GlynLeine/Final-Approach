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

        public bool m_usePhysics = false;

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

        public Collider(GameObject a_owner, bool a_usePhysics = false)
        {
            m_owner = a_owner;
            m_usePhysics = a_usePhysics;
        }

        public Collider(GameObject a_owner, BoundsObject a_source, bool a_usePhysics = false)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_source.GetBounds());
            m_usePhysics = a_usePhysics;
        }

        public Collider(GameObject a_owner, CollisionShape a_shape, bool a_usePhysics = false)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_shape);
            m_usePhysics = a_usePhysics;
        }

        public Collider(GameObject a_owner, CollisionShape[] a_collisionShapes, bool a_usePhysics = false)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>(a_collisionShapes);
            m_usePhysics = a_usePhysics;
        }

        public Collider(GameObject a_owner, List<CollisionShape> a_collisionShapes, bool a_usePhysics = false)
        {
            m_owner = a_owner;
            m_shapes = a_collisionShapes;
            m_usePhysics = a_usePhysics;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public virtual bool HitTest(ref Collider a_other)
        {
            m_minimumTranslationVec = new Vector2();
            if (m_ignoreList != null)
                if (m_ignoreList.Contains(a_other.m_owner.GetType()))
                    return false;

            if (a_other.m_ignoreList != null)
                if (a_other.m_ignoreList.Contains(m_owner.GetType()))
                    return false;

            m_minimumTranslationVec = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 tempMtv;
            Vector2 tempPoi;
            bool ret = false;
            foreach (CollisionShape shape in m_shapes)
                foreach (CollisionShape otherShape in a_other.m_shapes)
                {
                    if (shape.Overlaps(otherShape, out tempMtv, out tempPoi))
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

        public virtual bool HitTest(ref Collider a_other, int a_shapeIndex)
        {
            m_minimumTranslationVec = new Vector2();
            if (m_ignoreList != null)
                if (m_ignoreList.Contains(a_other.m_owner.GetType()))
                    return false;

            if (a_other.m_ignoreList != null)
                if (a_other.m_ignoreList.Contains(m_owner.GetType()))
                    return false;

            m_minimumTranslationVec = new Vector2(float.MaxValue, float.MaxValue);
            bool ret = false;
            foreach (CollisionShape shape in m_shapes)
            {
                if (shape.Overlaps(a_other.m_shapes[a_shapeIndex], out Vector2 tempMtv, out Vector2 tempPoi))
                {
                    if (tempMtv.sqrMagnitude < m_minimumTranslationVec.sqrMagnitude)
                        m_minimumTranslationVec = tempMtv;

                    if (m_usePhysics)
                    {
                        shape.ApplyForce(tempMtv, tempPoi, out Vector2 correctionTransl, out float correctionRot);
                        m_owner.position += correctionTransl;
                        m_owner.rotation += correctionRot;
                    }

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

            Vector2 vel = m_owner.m_velocity * Time.deltaTime;
            Vector2 center = m_owner.position + vel * 0.5f;
            float radius = maxReach * 2 + vel.magnitude * 0.5f;

            Game.main.UI.Ellipse(center.x - radius, center.y - radius, radius * 2, radius * 2);

            return new Circle(center.x, center.y, radius, null);
        }
    }
}

