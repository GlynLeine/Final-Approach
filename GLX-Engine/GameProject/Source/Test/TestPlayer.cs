using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    public class TestPlayer : GameObject
    {
        const float m_speed = 300;
        const float m_angularAcceleration = 5f;

        Vector2 m_direction = new Vector2();

        Vector2 m_movementDirection = new Vector2();

        public Sprite m_sprite;

        public TestPlayer(Scene a_scene) : base(a_scene)
        {
            m_sprite = new Sprite("Textures/Rectangle.png");
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            m_sprite.rotation += 45;
            AddChild(m_sprite);
            Initialise();
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_sprite);
        }

        public void MoveForward(float a_value, List<int> a_controllerID)
        {
            m_movementDirection.y -= a_value;
        }

        public void MoveRight(float a_value, List<int> a_controllerIDs)
        {
            m_movementDirection.x += a_value;
        }

        public void FaceForward(float a_value, List<int> a_controllerID)
        {
            m_direction.y -= a_value;
        }

        public void FaceRight(float a_value, List<int> a_controllerID)
        {
            m_direction.x += a_value;
        }

        public void OnCollision(GameObject other, Vector2 a_mtv)
        {
            if (!HasChild(other))
            {
                game.UI.Stroke(255, 0, 0);
                game.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + a_mtv.x, screenPosition.y + a_mtv.y);
                position += a_mtv;
            }
        }

        void Update(float a_dt)
        {
            if (m_direction.sqrMagnitude > 0)
                rotation = m_direction.angle;
            m_direction *= 0;

            if (m_movementDirection.sqrMagnitude > 0)
            {
                m_movementDirection.SetMagnitude(m_speed);
            }

            m_velocity = m_movementDirection;

            position += m_velocity * a_dt;

            Game.main.UI.StrokeWeight(4);
            Game.main.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_velocity.x, screenPosition.y + m_velocity.y);
            Game.main.UI.Stroke(0, 255, 0);
            Game.main.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_velocity.x * a_dt, screenPosition.y + m_velocity.y * a_dt);

            m_movementDirection *= 0;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            Game.main.UI.Text(position.ToString(), 300, 300);
        }
    }
}
