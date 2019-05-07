using System;

namespace GLXEngine.Core
{
    public class Line : Shape
    {
        public float m_angle;
        public float m_length;

        public Vector2 start
        {
            get
            {
                Vector2 halfLine = new Vector2(m_angle) * (m_length * 0.5f);
                return position - halfLine;
            }

            set
            {
                Vector2 halfLine = value - position;
                m_length = halfLine.magnitude * 2f;
                m_angle = halfLine.angle;
            }
        }

        public Vector2 end
        {
            get
            {
                Vector2 halfLine = new Vector2(m_angle) * (m_length * 0.5f);
                return position + halfLine;
            }

            set
            {
                Vector2 halfLine = position - value;
                m_length = halfLine.magnitude * 2f;
                m_angle = halfLine.angle;
            }
        }

        public override bool Contains(Vector2 a_point, out Vector2 o_mtv)
        {
            a_point -= start;
            a_point.angle -= m_angle;

            if (a_point.x >= 0 && a_point.x <= m_length && position.y == 0)
            {
                a_point.angle += m_angle;
                a_point += start;
                return true;
            }

            a_point.angle += m_angle;
            a_point += start;
            return false;
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv)
        {
            o_mtv = null;
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out o_mtv);
            }

            return false;
        }

        public bool Overlaps(Rectangle a_other, out Vector2 o_mtv)
        {
            position -= a_other.position;
            position.angle -= a_other.m_angle;

            Vector2 min = new Vector2(a_other.left, a_other.top);
            Vector2 max = new Vector2(a_other.right, a_other.bottom);
            Vector2 closestPoint = Vector2.Clamp(position, min, max);

            if (Contains(closestPoint, out o_mtv))
            {
                position.angle += a_other.m_angle;
                position += a_other.position;
                return true;
            }

            position.angle += a_other.m_angle;
            position += a_other.position;
            return false;
        }

        public bool Overlaps(Line a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            Vector2 line = end - start;
            Vector2 otherLine = a_other.end - a_other.start;

            float crossArea = line.x * otherLine.y - line.y * otherLine.x;
            float colScalarA = ((a_other.start.x - start.x) * otherLine.y - (a_other.start.y - start.y) * otherLine.x) / crossArea;
            float colScalarB = ((a_other.start.x - start.x) * line.y - (a_other.start.y - start.y) * line.x) / crossArea;

            return 0 <= colScalarB && colScalarB <= 1 && 0 <= colScalarA && colScalarA <= 1;
        }
    }
}
