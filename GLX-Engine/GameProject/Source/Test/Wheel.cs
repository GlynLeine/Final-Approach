using GLXEngine;
using GLXEngine.Core;
using System.Collections.Generic;

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
            m_base.height = 10;
            AddChild(m_base);
            
            m_leftFork = new AnimationSprite("Textures/tileSheet.png", 13, 6);
            m_leftFork.x -= 295;
            m_leftFork.y -= 55;
            m_leftFork.width = 10;
            m_leftFork.height = 100;
            AddChild(m_leftFork);

            Initialise();
        }

        protected override Collider createCollider()
        {
            List<CollisionShape> collisionShapes = new List<CollisionShape>();
            int xSplit = Mathf.Ceiling(m_base.width / 64f);
            int ySplit = Mathf.Ceiling(m_base.height / 64f);

            float rWidth = m_base.width / xSplit;
            float rHeight = m_base.height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    collisionShapes.Add(new Rectangle(i * rWidth - m_base.width/2f + rWidth/2f, j * rHeight - m_base.height/2f + rHeight/2f, rWidth, rHeight, this));

            xSplit = Mathf.Ceiling(m_leftFork.width / 64f);
            ySplit = Mathf.Ceiling(m_leftFork.height / 64f);

            rWidth = m_leftFork.width / xSplit;
            rHeight = m_leftFork.height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                {
                    Vector2 pos = new Vector2();
                    pos.x = i * rWidth - m_leftFork.width/2f + rWidth/2f;
                    pos.y = j * rHeight - m_leftFork.height/2f + rHeight/2f;
                    pos.Rotate(m_leftFork.rotation);
                    pos+= m_leftFork.position;
                    collisionShapes.Add(new Rectangle(pos.x, pos.y, rWidth, rHeight, this));
                }


            return new Collider(this, collisionShapes);
        }
    }
}
