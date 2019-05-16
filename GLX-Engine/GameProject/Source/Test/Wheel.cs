using GLXEngine;
using GLXEngine.Core;
using System.Collections.Generic;
using static GLXEngine.Utils;

namespace GameProject
{
    public class Wheel : GameObject
    {
        Sprite m_base;
        Sprite m_leftFork;
        public Wheel(Scene a_scene) : base(a_scene)
        {
            m_base = new AnimationSprite("Textures/tileSheet.png", 13, 6);
            m_base.width = 600;
            m_base.height = 60;
            AddChild(m_base);

            m_leftFork = new AnimationSprite("Textures/tileSheet.png", 13, 6);
            m_leftFork.x -= 270;
            m_leftFork.y -= 60;
            m_leftFork.width = 60;
            m_leftFork.height = 60;
            AddChild(m_leftFork);

            Initialise();
        }

        //public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        //{
        //    if (!HasChild(other))
        //    {
        //        game.UI.Stroke(255, 0, 0);
        //        game.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + a_mtv.x, screenPosition.y + a_mtv.y);
        //        //m_velocity += a_mtv;
        //    }
        //}

        public override void Update(float a_dt)
        {
            //position += m_velocity;
            //m_velocity *= 0.9f;

            //rotation += m_angularVelocity;
            //m_angularVelocity *= 0.9f;
        }

        protected override Collider createCollider()
        {
            List<CollisionShape> collisionShapes = new List<CollisionShape>();

            int xSplit = Mathf.Ceiling(m_base.width / MAX_COL_WIDTH);
            int ySplit = Mathf.Ceiling(m_base.height / MAX_COL_WIDTH);

            float rWidth = m_base.width / xSplit;
            float rHeight = m_base.height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    collisionShapes.Add(new Rectangle(i * rWidth + rWidth / 2f - m_base.width / 2f + m_base.x, j * rHeight + rHeight / 2f - m_base.height / 2f + m_base.y, rWidth, rHeight, this));

            xSplit = Mathf.Ceiling(m_leftFork.width / MAX_COL_WIDTH);
            ySplit = Mathf.Ceiling(m_leftFork.height / MAX_COL_WIDTH);

            rWidth = m_leftFork.width / xSplit;
            rHeight = m_leftFork.height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    collisionShapes.Add(new Rectangle(i * rWidth + rWidth / 2f - m_leftFork.width / 2f + m_leftFork.x, j * rHeight + rHeight / 2f - m_leftFork.height / 2f + m_leftFork.y, rWidth, rHeight, this));

            return new Collider(this, collisionShapes);
        }
    }
}
