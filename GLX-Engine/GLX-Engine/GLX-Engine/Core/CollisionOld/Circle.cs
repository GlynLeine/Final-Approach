using System;

namespace GLXEngine.Core
{
    public class Circle : Shape
    {
        public float radius;
        //------------------------------------------------------------------------------------------------------------------------
        //														Rectangle()
        //------------------------------------------------------------------------------------------------------------------------
        public Circle(float x, float y, float radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Properties()
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Contains(Vector2 a_point, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            if (Vector2.Distance(position, a_point) <= radius)
            {
                o_mtv = position - a_point;
                o_mtv.magnitude = radius - o_mtv.magnitude;
                return true;
            }
            return false;
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return Overlaps(a_other as Circle, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out o_mtv);
            }

            return false;
        }

        public bool Overlaps(Line a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            position -= a_other.start;
            position.angle -= a_other.m_angle;

            if (position.x >= 0 && position.x <= a_other.m_length && position.y >= -radius && position.y <= radius)
            {
                position.angle += a_other.m_angle;
                position += a_other.start;
                return true;
            }

            position.angle += a_other.m_angle;
            position += a_other.start;
            if (Contains(new Vector2(), out o_mtv))
                return true;
            else
                return Contains(new Vector2(a_other.m_length, 0), out o_mtv);
        }

        public bool Overlaps(Circle a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            if (position.Distance(a_other.position) < radius + a_other.radius)
            {
                o_mtv = position - a_other.position;
                o_mtv.magnitude = (radius + a_other.radius) - o_mtv.magnitude;
                return true;
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

        //------------------------------------------------------------------------------------------------------------------------
        //														ToString()
        //------------------------------------------------------------------------------------------------------------------------
        override public string ToString()
        {
            return (x + "," + y + "," + radius);
        }

    }
}

