

namespace GLXEngine.Core
{
    public class Line : Shape
    {
        float m_angle;
        float m_length;

        Vector2 start
        {
            get
            {
                return null;
            }
        }

        Vector2 end
        {
            get
            {
                return null;
            }
        }

        public override bool Contains(Vector2 a_point)
        {
            throw new System.NotImplementedException();
        }

        public override bool Overlaps(Shape a_other)
        {
            throw new System.NotImplementedException();
        }
    }
}
