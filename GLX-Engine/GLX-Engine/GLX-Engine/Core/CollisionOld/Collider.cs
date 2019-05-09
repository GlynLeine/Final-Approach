using System.Collections.Generic;
using System;

namespace GLXEngine.Core
{
    public class Collider
    {
        public Vector2 m_minimumTranslationVec = new Vector2();

        public List<Shape> m_shapes = new List<Shape>();

        public List<Type> m_ignoreList;

        public GameObject m_owner;

        public Collider()
        {
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
            foreach (Shape shape in m_shapes)
                foreach (Shape otherShape in other.m_shapes)
                {
                    if (shape.Overlaps(otherShape, out tempMtv))
                    {
                        if (tempMtv.sqrMagnitude < m_minimumTranslationVec.sqrMagnitude)
                            m_minimumTranslationVec = tempMtv;
                        return true;
                    }
                    if (tempMtv.sqrMagnitude < m_minimumTranslationVec.sqrMagnitude)
                        m_minimumTranslationVec = tempMtv;
                }

            m_minimumTranslationVec = new Vector2();
            return false;
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
    }
}

