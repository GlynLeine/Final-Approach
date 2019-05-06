using GLXEngine.Core;
using System.Collections.Generic;
using GLXEngine.Rendering;

namespace GLXEngine.Collision
{
    public class ComplexCollisionShape
    {
        public List<Vector2> m_points = new List<Vector2>();

        public ComplexCollisionShape() { }
        public ComplexCollisionShape(MeshComponent2D a_source)
        {
            foreach(Vertex2D vert in a_source.m_vertices)
                m_points.Add(vert.m_position);
        }

        public ComplexCollisionShape(PhysicsComponent a_source)
        {
            foreach(PhysicsPoint pp in a_source.m_points)
                m_points.Add(pp.m_position);
        }
    }
}
