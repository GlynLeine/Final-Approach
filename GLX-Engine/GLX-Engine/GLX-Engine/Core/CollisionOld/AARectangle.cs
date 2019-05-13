using System;

namespace GLXEngine.Core
{
    public class AARectangle : Shape
    {
        public float m_width, m_height;

        //------------------------------------------------------------------------------------------------------------------------
        //														AARectangle()
        //------------------------------------------------------------------------------------------------------------------------
        public AARectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.m_width = width;
            this.m_height = height;
        }

        public AARectangle(AARectangle a_source)
        {
            x = a_source.x;
            y = a_source.y;
            m_width = a_source.m_width;
            m_height = a_source.m_height;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Properties()
        //------------------------------------------------------------------------------------------------------------------------
        public float p_left { get { return x; } set { x = value; } }
        public float p_right { get { return x + m_width; } set { m_width = value - x; } }
        public float p_top { get { return y; } set { y = value; } }
        public float p_bottom { get { return y + m_height; } set { m_height = value - y; } }

        public override bool Contains(Vector2 a_point)
        {
            return a_point.x >= p_left && a_point.x <= p_right && a_point.y >= p_top && a_point.y <= p_bottom;
        }

        public bool Overlaps(AARectangle a_other)
        {
            return !(a_other.p_left > p_right || a_other.p_right < p_left || a_other.p_bottom < p_top || a_other.p_top > p_bottom);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ToString()
        //------------------------------------------------------------------------------------------------------------------------
        override public string ToString()
        {
            return (x + "," + y + "," + m_width + "," + m_height);
        }

        public override bool Overlaps(Shape a_other)
        {
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(AARectangle)))
            {
                return Overlaps(a_other as AARectangle);
            }

            throw new NotImplementedException();
        }
    }
}

