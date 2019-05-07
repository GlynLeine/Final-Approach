

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
                Vector2 halfLine = new Vector2(m_angle) * (m_length*0.5f);
                return position - halfLine;
            }

            set
            {
                Vector2 halfLine = value - position;
                m_length = halfLine.magnitude*2f;
                m_angle = halfLine.angle;
            }
        }

        public Vector2 end
        {
            get
            {
                Vector2 halfLine = new Vector2(m_angle) * (m_length*0.5f);
                return position + halfLine;
            }

            set
            {
                Vector2 halfLine = position - value;
                m_length = halfLine.magnitude*2f;
                m_angle = halfLine.angle;
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
