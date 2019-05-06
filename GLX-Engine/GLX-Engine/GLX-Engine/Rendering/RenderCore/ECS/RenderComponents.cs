using GLXEngine.Core;
using System.Collections.Generic;
using GLXEngine.ECS;

namespace GLXEngine.Rendering
{
    public class Vertex2D
    {
        public Vector2 m_position;
        public Vector2 m_texCoord;
    }

    public class MeshComponent2D : ECSComponent
    {
        public List<Vertex2D> m_vertices = new List<Vertex2D>();
    }
}
