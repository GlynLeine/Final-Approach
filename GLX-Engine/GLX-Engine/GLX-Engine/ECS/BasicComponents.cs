using GLXEngine.Core;
using GLXEngine.ECS;

namespace GLXEngine
{
    public class TransformComponent : ECSComponent
    {
        //  îx    îy    îz    îw
        //  ĵx    ĵy    ĵz    ĵw
        //  k̂x    k̂y    k̂z    k̂w
        //  Tx    Ty    Tz    Tw
        public float[] m_matrix = new float[16] {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f };

        public float[] m_prevMatrix = new float[16] {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f };

        float m_rotation = 0.0f;
        float m_prevRotation = 0.0f;
        
        public Vector2 m_scale = new Vector2(1f, 1f);
        public Vector2 m_prevScale = new Vector2(1f, 1f);

        public Vector2 scale
        {
            get { return m_scale;}
            set { m_prevScale = m_scale; m_scale = value;}
        }

        public Vector2 prevScale
        {
            get { return m_prevScale;}
            private set { }
        }

        public Vector2 position {
			get { return new Vector2(m_matrix[12], m_matrix[13]);}
			set { m_prevMatrix[12] = m_matrix[12]; m_prevMatrix[13] = m_matrix[13]; m_matrix[12] = value.x; m_matrix[13] = value.y;}
		}

        public Vector2 prevPosition {
			get { return new Vector2(m_prevMatrix[12], m_prevMatrix[13]);}
			private set {}
		}

        public float rotation {
			get { return m_rotation; }
			set {
                m_prevRotation = m_rotation;
                m_prevMatrix[0] = m_matrix[0];
                m_prevMatrix[1] = m_matrix[1];
                m_prevMatrix[4] = m_matrix[4];
                m_prevMatrix[5] = m_matrix[5]; 

				m_rotation = value;
				float rotation = m_rotation * Mathf.PI / 180.0f;
				float cosine = Mathf.Cos (rotation);
				float sine = Mathf.Sin (rotation);
				m_matrix[0] = cosine;
				m_matrix[1] = sine;
				m_matrix[4] = -sine;
				m_matrix[5] = cosine;
			}
		}

        public float prevRotation {
			get { return m_prevRotation; }
			private set { }
		}
    }
}
