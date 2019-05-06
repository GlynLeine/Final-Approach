using GLXEngine.Core;
using System.Collections.Generic;
using GLXEngine.ECS;
using GLXEngine.Collision;

namespace GLXEngine
{
    public delegate void CollsionDelegate(Vector2 mtv, Vector2 poi);
    public class CollisionComponent : ECSComponent
    {
        public SimpleCollisionShape m_simpleCollisionShape;
        public ComplexCollisionShape m_complexCollisionShape;

        public CollsionDelegate m_collisionCommand;

        public bool m_useComplex = false;
        public bool m_usePhysics = true;

        public Vector2 m_origin = null;

        // minimum translation vector to translate out of a collision.
        public Vector2 m_mtv = null;
    }
}
