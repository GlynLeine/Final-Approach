using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using GLXEngine.ECS;


namespace GLXEngine.Physics
{
    public class PhysicsSystem : ECSSystem
    {
        public PhysicsSystem(Scene a_scene) : base(new System.Type[] { typeof(PhysicsComponent), typeof(TransformComponent) })
        {
            a_scene.objectList.ForEach(gameObject => { Add(gameObject); });
        }

        public override void Update(float a_dt)
        {
            Dictionary<int, ECSComponentHandle<PhysicsComponent>> physicsComponents = GetComponents<PhysicsComponent>();
            Dictionary<int, ECSComponentHandle<TransformComponent>> transformComponents = GetComponents<TransformComponent>();

            foreach (int entityID in GetEntityList())
            {
                PhysicsComponent physicsComponent = physicsComponents[entityID].value;
                TransformComponent transformComponent = transformComponents[entityID].value;
                List<PhysicsPoint> points = physicsComponent.m_points;

                Vector2 origin = new Vector2();

                // calc velocity and update
                for (int i = 0; i < points.Count; i++)
                {
                    PhysicsPoint point = points[i];
                    Vector2 velocity = point.m_position - point.m_previousPosition;
                    point.m_position += velocity * a_dt;
                    point.m_previousPosition = point.m_position;
                    origin += point.m_position;
                }

                origin /= points.Count;

                // move transform according to physics update.
                transformComponent.position += origin;
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].m_position -= origin;
                    points[i].m_previousPosition -= origin;
                }

                // calculate difference in rotation from oldpos and new pos!
                //transform.rotation = (points[0].m_position.angle + points[1].m_position.angle) / 2f;

            }
        }

    }
}
