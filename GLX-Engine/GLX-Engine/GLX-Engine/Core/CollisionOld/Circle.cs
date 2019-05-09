using System;

namespace GLXEngine.Core
{
    public class Circle : Shape
    {
        public float radius;
        //------------------------------------------------------------------------------------------------------------------------
        //														Circle()
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

        public override bool Contains(Vector2 a_point)
        {
            return Vector2.Distance(position, a_point) <= radius;
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
            else if (otherType.IsAssignableFrom(typeof(AARectangle)))
            {
                return Overlaps(a_other as AARectangle);
            }

            return false;
        }

        public bool Overlaps(Line a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            position = (position - a_other.start).Rotate(-a_other.rotation);

            if (position.x >= 0 && position.x <= a_other.m_length && position.y >= -radius && position.y <= radius)
            {
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }


            if (Contains(new Vector2(), out o_mtv))
            {
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }
            else if (Contains(new Vector2(a_other.m_length, 0), out o_mtv))
            {
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }
            position = position.Rotate(a_other.rotation) + a_other.start;
            return false;
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
            position = (position - a_other.position).Rotate(-a_other.rotation);

            Vector2 min = new Vector2(a_other.p_left, a_other.p_top);
            Vector2 max = new Vector2(a_other.p_right, a_other.p_bottom);
            Vector2 closestPoint = Vector2.Clamp(position, min, max);

            if (Contains(closestPoint, out o_mtv))
            {
                position = position.Rotate(a_other.rotation) + a_other.position;
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
                return Overlaps(a_other as Circle, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(AARectangle)))
            {
                return Overlaps(a_other as AARectangle);
            }

            return false;
        }

        public bool Overlaps(AARectangle a_other)
        {
            float xDist = Mathf.Abs(a_other.x - x);
            float yDist = Mathf.Abs(a_other.y - y);

            float w = a_other.m_width;
            float h = a_other.m_height;

            float edges = Mathf.Pow(xDist - w, 2) + Mathf.Pow(yDist - h, 2);

            // no intersection
            if (xDist > (radius + w) || yDist > (radius + h))
                return false;

            // intersection within the circle
            if (xDist <= w || yDist <= h)
                return true;

            // intersection on the edge of the circle
            return edges <= radius*radius;
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

