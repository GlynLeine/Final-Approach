using System.Collections.Generic;

namespace GLXEngine.Core
{
    public class Collider
    {
        public Vector2 m_minimumTranslationVec = new Vector2();

        public List<Shape> m_shapes = new List<Shape>();

        public Collider()
        {
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public virtual bool HitTest(ref Collider other)
        {
            foreach (Shape shape in m_shapes)
                foreach (Shape otherShape in other.m_shapes)
                    if (shape.Overlaps(otherShape))
                        return true;
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

