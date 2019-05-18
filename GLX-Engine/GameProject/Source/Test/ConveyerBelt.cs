using GLXEngine;
using GLXEngine.Core;
using System;
using static GLXEngine.Utils;


namespace GameProject
{
    public class ConveyerBelt : ForceApplier
    {
        Sprite m_sprite;

        float m_reach;

        public bool right = true;

        public ConveyerBelt(Scene a_scene, float a_width) : base(a_scene, new Type[] { typeof(Border), typeof(Magnet), typeof(Fan) })
        {
            m_sprite = new Sprite("Textures/conveyor_belt.png");
            m_sprite.height = a_width/m_sprite.width * m_sprite.height;
            m_sprite.width = a_width;
            m_sprite.SetOrigin(0, 0);
            AddChild(m_sprite);

            m_reach = m_sprite.width;

            SetForceCalcFunc(CalcForce);

            float w = m_reach;
            float h = 5;

            Rectangle aoe = new Rectangle(m_sprite.width / 2, -h / 2 + m_sprite.height*0.05f, w, h, this);
            SetAOE(new CollisionShape[] { aoe });

            m_mouseHandler.OnMouseDownOnTarget += OnMouseEvent;
            m_mouseHandler.OnMouseUp += OnMouseEvent;
        }

        public new void Update(float a_dt)
        {
            base.Update(a_dt);
        }

        protected override Collider createCollider()
        {
            float w = m_sprite.width*0.97f;
            float h = m_sprite.height*0.9f;
            Rectangle aoe = new Rectangle(m_sprite.x + m_sprite.width / 2, m_sprite.y + m_sprite.height/ 2, w, h, this);
            Collider ret = new Collider(this, aoe);
            ret.m_static = true;
            return ret;
        }

        public Vector2 CalcForce(GameObject a_other)
        {
            if (right)
            {
                return new Vector2(1, 0);
            }
            return new Vector2(-1, 0);
        }
    }
}
