using System.Collections.Generic;
using System;
using static GLXEngine.Utils;

namespace GLXEngine.Core
{
    public class Collider
    {
        public Vector2 m_minimumTranslationVec = new Vector2();
        public Vector2 m_pointOfImpact = new Vector2();

        public List<CollisionShape> m_shapes = new List<CollisionShape>();

        public List<Type> m_ignoreList = new List<Type>();

        private List<Vector2> m_findPoints = new List<Vector2>();

        public GameObject m_owner;

        public bool m_usePhysics = false;

        public float m_restitution = 1;

        public bool m_hitTest = true;
        public bool m_overlap = true;
        public bool m_static = false;

        private Vector2 m_position;
        private float m_rotation;

        public List<Vector2> p_findPoints
        {
            get
            {
                foreach (CollisionShape collisionShape in m_shapes)
                {

                }
                return m_findPoints;
            }
        }

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

        public Collider(GameObject a_owner, bool a_usePhysics = false, float a_restitution = 1)
        {
            m_owner = a_owner;
            m_usePhysics = a_usePhysics;
            m_restitution = a_restitution;
        }

        public Collider(GameObject a_owner, BoundsObject a_source, bool a_usePhysics = false, float a_restitution = 1)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_source.GetBounds());
            m_usePhysics = a_usePhysics;
            m_restitution = a_restitution;
        }

        public Collider(GameObject a_owner, CollisionShape a_shape, bool a_usePhysics = false, float a_restitution = 1)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>();
            m_shapes.Add(a_shape);
            m_usePhysics = a_usePhysics;
            m_restitution = a_restitution;
        }

        public Collider(GameObject a_owner, CollisionShape[] a_collisionShapes, bool a_usePhysics = false, float a_restitution = 1)
        {
            m_owner = a_owner;
            m_shapes = new List<CollisionShape>(a_collisionShapes);
            m_usePhysics = a_usePhysics;
            m_restitution = a_restitution;
        }

        public Collider(GameObject a_owner, List<CollisionShape> a_collisionShapes, bool a_usePhysics = false, float a_restitution = 1)
        {
            m_owner = a_owner;
            m_shapes = a_collisionShapes;
            m_usePhysics = a_usePhysics;
            m_restitution = a_restitution;
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

                        m_pointOfImpact = tempPoi;
                        ret = true;
                    }
                }
            if (!ret)
            {
                m_minimumTranslationVec = new Vector2();
                m_pointOfImpact = null;
            }
            return ret;
        }

        public virtual bool HitTest(ref Collider a_other, int a_shapeIndex)
        {
            if (!m_hitTest && !m_overlap && !a_other.m_hitTest && !a_other.m_overlap)
                return false;

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

                    if (m_hitTest && a_other.m_hitTest && !m_owner.drag && !a_other.m_owner.drag)
                        if (m_usePhysics)
                        {
                            shape.ApplyForce(tempMtv, tempPoi, out Vector2 correctionTransl, out float correctionRot);
                            m_owner.position += correctionTransl * (1f - m_restitution);
                            m_owner.m_velocity += correctionTransl * m_restitution;// / Time.deltaTime;
                            m_owner.rotation += correctionRot * (1f - m_restitution);
                            m_owner.m_angularVelocity += correctionRot * m_restitution;
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
            Vector2 maxReach = new Vector2();
            Vector2 pos = null;
            foreach (CollisionShape collisionShape in m_shapes)
            {
                if (pos == null)
                    pos = collisionShape.position;
                else
                    pos = (pos + collisionShape.position) / 2f;

                Vector2 reach = collisionShape.GetMaxReach();
                if (reach.magnitude > maxReach.magnitude)
                    maxReach = reach;
            }

            Vector2 center = m_owner.TransformPoint(pos);

            Game.main.UI.Stroke(0, 255, 0);

            if (m_static)
            {
                Game.main.UI.Rect(center.x - maxReach.x - MAX_COL_WIDTH / 2, center.y - maxReach.y - MAX_COL_WIDTH / 2, maxReach.x * 2 + MAX_COL_WIDTH, maxReach.y * 2 + MAX_COL_WIDTH);
                return new AARectangle(center.x - maxReach.x - MAX_COL_WIDTH / 2, center.y - maxReach.y - MAX_COL_WIDTH / 2, maxReach.x * 2 + MAX_COL_WIDTH, maxReach.y * 2 + MAX_COL_WIDTH);
            }

            Vector2 vel = m_owner.GetScreenVelocity() * Time.deltaTime;
            center += vel * 0.5f;
            float radius = maxReach.magnitude + vel.magnitude * 0.5f + MAX_COL_WIDTH;

            Game.main.UI.Ellipse(center.x - radius, center.y - radius, radius * 2, radius * 2);

            return new Circle(center.x, center.y, radius, null);
        }
    }
}

