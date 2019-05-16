using System;

namespace GLXEngine.Core
{
    public class Circle : CollisionShape
    {
        public float radius;
        //------------------------------------------------------------------------------------------------------------------------
        //														Circle()
        //------------------------------------------------------------------------------------------------------------------------
        public Circle(float x, float y, float radius, GameObject a_parent) : base(a_parent)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public override void ApplyForce(Vector2 a_force, Vector2 a_poi, out Vector2 o_correctionTransl, out float o_correctionRot)
        {
            a_poi.magnitude = radius;
            o_correctionTransl = a_force;
            o_correctionRot = 0;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Properties()
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Contains(Vector2 a_point, out Vector2 o_mtv, out Vector2 o_poi)
        {
            o_mtv = new Vector2();
            o_poi = new Vector2();
            if (Vector2.Distance(position, a_point) <= radius)
            {
                o_mtv = position - a_point;
                o_mtv.magnitude = radius - o_mtv.magnitude;
                o_poi = position-(o_mtv.normal * radius);
                return true;
            }
            return false;
        }

        public override bool Contains(Vector2 a_point)
        {
            return Vector2.Distance(position, a_point) <= radius;
        }

        public override Vector2[] GetFindPoints()
        {
            if(radius <= 32)
                return new Vector2[]{ m_parent.TransformPoint(position)};

            float circumference = Mathf.PI*radius*2;
            int pointCount = Mathf.Ceiling(circumference/32);

            Vector2[] points = new Vector2[pointCount];
            for(int i = 0; i < pointCount; i++)
            {
                points[i] = m_parent.TransformPoint(position + new Vector2(i*(360/pointCount)));
            }

            return points;
        }

        public override Vector2 GetMaxReach()
        {
            return new Vector2(radius, radius);
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv, out Vector2 o_poi)
        {
            o_mtv = new Vector2();
            o_poi = new Vector2();
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv, out o_poi);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return Overlaps(a_other as Circle, out o_mtv, out o_poi);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out o_mtv, out o_poi);
            }
            else if (otherType.IsAssignableFrom(typeof(AARectangle)))
            {
                return Overlaps(a_other as AARectangle);
            }

            return false;
        }

        public bool Overlaps(Line a_other, out Vector2 o_minTranslVec, out Vector2 o_pointOfCollision)
        {
            o_minTranslVec = new Vector2();
            o_pointOfCollision = new Vector2();
            position = (position - a_other.start).Rotate(-a_other.rotation);

            if (position.x >= 0 && position.x <= a_other.m_length && position.y >= -radius && position.y <= radius)
            {
                o_pointOfCollision = new Vector2(position.x, 0).Rotate(a_other.rotation) + a_other.start;
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }


            if (Contains(new Vector2(), out o_minTranslVec, out o_pointOfCollision))
            {
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }
            else if (Contains(new Vector2(a_other.m_length, 0), out o_minTranslVec, out o_pointOfCollision))
            {
                position = position.Rotate(a_other.rotation) + a_other.start;
                return true;
            }
            position = position.Rotate(a_other.rotation) + a_other.start;
            return false;
        }

        public bool Overlaps(Circle a_other, out Vector2 o_mtv, out Vector2 o_pointOfCollision)
        {
            o_mtv = new Vector2();
            o_pointOfCollision = new Vector2();
            if (position.Distance(a_other.position) < radius + a_other.radius)
            {
                o_mtv = position - a_other.position;
                o_mtv.magnitude = (radius + a_other.radius) - o_mtv.magnitude;

                o_pointOfCollision = position-(o_mtv.normal * radius);
                return true;
            }
            return false;
        }

        public bool Overlaps(Rectangle a_other, out Vector2 o_mtv, out Vector2 o_pointOfCollision)
        {
            position = (position - a_other.position).Rotate(-a_other.rotation);

            Vector2 min = new Vector2(a_other.p_left, a_other.p_top);
            Vector2 max = new Vector2(a_other.p_right, a_other.p_bottom);
            Vector2 closestPoint = Vector2.Clamp(position, min, max);

            if (Contains(closestPoint, out o_mtv, out o_pointOfCollision))
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
                return Overlaps(a_other as Rectangle, out temp, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return Overlaps(a_other as Circle, out temp, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return Overlaps(a_other as Line, out temp, out temp);
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
            return edges <= radius * radius;
        }

        public override Vector2 ScreenPos()
        {
            return m_parent.TransformPoint(position);
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

