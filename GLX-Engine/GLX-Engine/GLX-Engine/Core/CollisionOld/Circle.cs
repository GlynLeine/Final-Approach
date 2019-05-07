using System;
using System.Reflection;


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
        public override bool Contains(Vector2 a_point)
        {
            return Vector2.Distance(position, a_point) <= radius;
        }

        public override bool Overlaps(Shape a_other)
        {
            Type otherType = a_other.GetType();
            if(otherType.IsAssignableFrom(typeof(Circle)))
            {
                return Overlaps(a_other as Circle);
            }
            return false;
        }

        public bool Overlaps(Line a_other)
        {
            return false;
        }

        public bool Overlaps(Circle a_other)
        {
            return position.Dist(a_other.position) < radius + a_other.radius;
        }

        public bool Overlaps(Rectangle a_other)
        {
            float xDist = Mathf.Abs(a_other.x - x);
            float yDist = Mathf.Abs(a_other.y - y);

            float w = a_other.width;
            float h = a_other.height;

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

