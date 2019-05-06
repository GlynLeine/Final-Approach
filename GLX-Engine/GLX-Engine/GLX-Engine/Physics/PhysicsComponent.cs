using GLXEngine.Core;
using System.Collections.Generic;
using GLXEngine.ECS;
using GLXEngine.Rendering;

namespace GLXEngine
{

    public class PhysicsPoint
    {
        public Vector2 m_position = new Vector2();
        public Vector2 m_previousPosition = new Vector2();

        public PhysicsPoint(Vector2 a_position, Vector2 a_previousPosition) { m_position = a_position; m_previousPosition = a_previousPosition; }
        public PhysicsPoint(PhysicsPoint a_source) { m_position = a_source.m_position; m_previousPosition = a_source.m_previousPosition; }
    }

    public class PhysicsConstraint
    {

    }

    public class PhysicsComponent : ECSComponent
    {
        public List<PhysicsPoint> m_points = new List<PhysicsPoint>();
        public List<PhysicsConstraint> m_constraints = new List<PhysicsConstraint>();

        public PhysicsComponent(MeshComponent2D a_source)
        {
            foreach (Vertex2D vert in a_source.m_vertices)
                m_points.Add(new PhysicsPoint(vert.m_position, vert.m_position));
        }
    }
}
