using System;

namespace GLXEngine.Core
{
    public class Line : Shape
    {
        public float m_length;

        public Vector2 start
        {
            get
            {
                Vector2 halfLine = new Vector2(rotation) * (m_length * 0.5f);
                return position - halfLine;
            }

            set
            {
                Vector2 halfLine = value - position;
                m_length = halfLine.magnitude * 2f;
                rotation = halfLine.angle;
            }
        }

        public Vector2 end
        {
            get
            {
                Vector2 halfLine = new Vector2(rotation) * (m_length * 0.5f);
                return position + halfLine;
            }

            set
            {
                Vector2 halfLine = position - value;
                m_length = halfLine.magnitude * 2f;
                rotation = halfLine.angle;
            }
        }

        public override bool Contains(Vector2 a_point, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            a_point = (a_point - start).Rotate(-rotation);

            if (a_point.x >= 0 && a_point.x <= m_length && position.y == 0)
            {
                o_mtv = new Vector2(rotation + 90);
                a_point = a_point.Rotate(rotation) + start;
                return true;
            }

            a_point = a_point.Rotate(rotation) + start;
            return false;
        }

        public override bool Contains(Vector2 a_point)
        {
            a_point = (a_point - start).Rotate(-rotation);

            if (a_point.x >= 0 && a_point.x <= m_length && position.y == 0)
            {
                a_point = a_point.Rotate(rotation) + start;
                return true;
            }

            a_point = a_point.Rotate(rotation) + start;
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
            a_other.position = (a_other.position - start).Rotate(-rotation);

            Vector2 min = new Vector2(0, 0);
            Vector2 max = new Vector2(m_length, 0);
            Vector2 closestPoint = Vector2.Clamp(a_other.position, min, max);

            if (a_other.Contains(closestPoint, out o_mtv))
            {
                a_other.position = a_other.position.Rotate(rotation) + start;
                return true;
            }

            a_other.position = a_other.position.Rotate(rotation) + start;
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

            if (0 <= colScalarB && colScalarB <= 1 && 0 <= colScalarA && colScalarA <= 1)
            {
                if(colScalarA*line.magnitude < colScalarB*otherLine.magnitude)
                {
                    o_mtv = line*colScalarA;
                    return true;
                }
                o_mtv = -(otherLine*colScalarB);
                return true;
            }
            return false;
        }

        public override bool Overlaps(Shape a_other)
        {
            Vector2 temp;
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out temp);
            }

            return false;
        }
    }
}
