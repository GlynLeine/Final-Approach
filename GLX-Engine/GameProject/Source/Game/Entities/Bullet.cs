using GLXEngine.Core;
using GLXEngine;


namespace GameProject
{
    class Bullet : GameObject
    {
        Sprite m_sprite = new Sprite("Textures/bullet.png");

        public GameObject m_owner;
        public GameObject m_player;

        public float m_damage;

        public Bullet(Scene a_scene, GameObject a_shooter, GameObject a_player, float a_damage = 10) : base(a_scene)
        {
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);

            m_owner = a_shooter;
            m_player = a_player;
            m_damage = a_damage;
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_sprite);
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            if (a_collisionInfo.m_collider.m_owner is WallTile)
            {
                Destroy();
            }
        }

        protected override void OnDestroy()
        {
           // ((Overworld)m_scene).bullets.Remove(this);
            base.OnDestroy();
        }

        public override void Update(float a_dt)
        {
            if (screenPosition.x < -game.width * 0.5f || screenPosition.x > game.width * 1.5f || screenPosition.y < -game.height * 0.5f || screenPosition.y > game.height * 1.5f)
            {
                Destroy();
            }

            position += m_velocity * a_dt + (m_velocity.normal.Dot(m_player.m_velocity * a_dt) * m_velocity.normal);
            rotation = m_velocity.angle;
        }
    }
}
