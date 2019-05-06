using GLXEngine.Core;
using System.Collections.Generic;

namespace GLXEngine.Collision
{
    public enum ShapeType
    {
        CIRCLE,
        BOX,
        LINE,
        POINT,
        MISC
    }

    public class SimpleCollisionShape
    {
        private List<SimpleCollisionShape> m_children = new List<SimpleCollisionShape>();
        private ShapeType m_shapeType = ShapeType.MISC;
        public float m_width, m_height;

        SimpleCollisionShape() { }

        SimpleCollisionShape(ShapeType a_shapeType, float a_width, float a_height = 0)
        {
            m_shapeType = a_shapeType;
            m_width = a_width;
            m_height = a_height;
        }

        public ShapeType shapeType
        {
            get
            {
                return m_shapeType;
            }
            set
            {
                if (m_children.Count == 0)
                    m_shapeType = value;
            }
        }

        public void AddChild(SimpleCollisionShape a_shape)
        {
            m_children.Add(a_shape);
            m_shapeType = ShapeType.MISC;
        }
    }
}
