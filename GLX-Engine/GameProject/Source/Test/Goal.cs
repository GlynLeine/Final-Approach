using GLXEngine;
using GLXEngine.Core;
using System;

namespace GameProject
{
    public class Goal : GameObject
    {
        Sprite m_sprite;

        public Goal(Scene a_scene) : base(a_scene)
        {
            m_sprite = new Sprite("Textures/Rectangle.png");
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);

            Initialise();
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            if(a_collisionInfo.m_collider.m_owner.GetType() == typeof(TestPlayer))
            {
                m_scene.y -= 200;
            }
        }

        public override void Update(float a_dt)
        {
            base.Update(a_dt);
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_sprite);
        }
    }
}
