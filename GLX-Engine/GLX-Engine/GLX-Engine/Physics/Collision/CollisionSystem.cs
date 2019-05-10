using GLXEngine.Core;
using static GLXEngine.CollisionAlgorithms;
using System.Collections.Generic;
using GLXEngine.ECS;

namespace GLXEngine.Collision
{
    public class CollisionSystem : ECSSystem
    {
        private QuadTree m_colliderTree;

        public CollisionSystem(Scene a_scene) : base(new System.Type[] { typeof(CollisionComponent), typeof(TransformComponent) })
        {
            a_scene.objectList.ForEach(gameObject => { Add(gameObject); });
        }

        public override void Update(float a_dt)
        {
            Dictionary<int, ECSComponentHandle<CollisionComponent>> collisionComponents = GetComponents<CollisionComponent>();
            Dictionary<int, ECSComponentHandle<TransformComponent>> transformComponents = GetComponents<TransformComponent>();

            m_colliderTree = new QuadTree(m_colliderTree.m_boundary, m_colliderTree.m_capacity);

            List<int> entityList = GetEntityList();

            for (int i = 0; i < entityList.Count; i++)
            {
                m_colliderTree.Insert(new QuadTree.Point(transformComponents[i].value.position, entityList[i]));
            }

            for (int i = entityList.Count - 1; i >= 0; i--)
            {
                int entityID = entityList[i];
                if (i >= entityList.Count) continue;

                CollisionComponent collisionComponent = collisionComponents[entityID].value;
                TransformComponent transformComponent = transformComponents[entityID].value;

                Vector2 position = transformComponent.position;
                Vector2 prevPosition = transformComponent.prevPosition;

                List<QuadTree.Point> foundEntities = new List<QuadTree.Point>();

                if (collisionComponent.m_useComplex)
                {
                    m_colliderTree.Query(CalcBroadphaseAreaComplex(position, prevPosition, collisionComponent.m_complexCollisionShape), ref foundEntities, 255);

                    for (int j = foundEntities.Count - 1; j >= 0; j--)
                    {

                        if (j >= foundEntities.Count) continue; //fix for removal in loop

                        int otherID = (int)(foundEntities[j].data);
                        if (entityID != otherID)
                        {
                            CollisionComponent otherCollisionComponent = collisionComponents[otherID].value;
                            TransformComponent otherTransformComponent = transformComponents[otherID].value;
                            Vector2 mtv = new Vector2();
                            Vector2 poc = new Vector2();

                            if (otherCollisionComponent.m_useComplex)
                            {
                                Vector2[] hull = collisionComponent.m_complexCollisionShape.m_points.ToArray();
                                Vector2[] otherHull = otherCollisionComponent.m_complexCollisionShape.m_points.ToArray();

                                Vector2 colPos;
                                Vector2 otherColPos;

                                if (SweepCheck(hull, position, prevPosition, otherHull, otherTransformComponent.position, otherTransformComponent.prevPosition, out colPos, out otherColPos))
                                {

                                    if (SAT(hull, colPos, otherHull, otherColPos, out mtv, out poc))
                                    {
                                        collisionComponent.m_collisionCommand.Invoke(mtv, poc-colPos);
                                        otherCollisionComponent.m_collisionCommand.Invoke(-mtv, poc-otherColPos);

                                        if(collisionComponent.m_usePhysics)
                                        {
                                            //m_owner.
                                        }

                                        if(otherCollisionComponent.m_usePhysics)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
