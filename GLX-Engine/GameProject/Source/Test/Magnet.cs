using GLXEngine;
using GLXEngine.Core;
using System;


namespace GameProject
{
    public class Magnet : ForceApplier
    {
        Sprite m_sprite;

        float m_reach;

        bool on = true;
        float time = 0;

        public Magnet(Scene a_scene) : base(a_scene, new Type[] { typeof(Border), typeof(Magnet), typeof(Fan) })
        {
            m_sprite = new Sprite("Textures/Magnet.png");
            m_sprite.width = 64;
            m_sprite.height = 64;
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            AddChild(m_sprite);

            m_reach = a_scene.width / 2;

            SetForceCalcFunc(CalcForce);

            float w = m_reach;
            float h = 64;

            Rectangle aoe = new Rectangle(w / 2 + m_sprite.width / 2, 0, w, h, this);
            SetAOE(new CollisionShape[] { aoe });

            m_mouseHandler.OnMouseDownOnTarget += OnMouseEvent;
            m_mouseHandler.OnMouseUp += OnMouseEvent;
        }

        public override void OnMouseEvent(GameObject a_target, MouseEventType a_eventType, Vector2 a_mousePos)
        {
            if (a_eventType == MouseEventType.MouseDownOnTarget)
                drag = true;
            else
                drag = false;
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            if (a_collisionInfo.m_collider.m_owner.GetType() == typeof(TestPlayer) && on)
            {
                on = false;
            }
        }

        public new void Update(float a_dt)
        {
            if (drag)
            {
                QuadTree.Point snapPoint = SnapLocation.FindPoint(new Circle(x, y, 32, null));
                Vector2 snapLoc = snapPoint.position;

                if (Vector2.Distance(snapLoc, new Vector2(Input.mouseX, Input.mouseY)) < 32)
                {
                    if ((snapPoint.data as SnapLocation).m_containedObject == null || (snapPoint.data as SnapLocation).m_containedObject == this)
                    {
                        position = snapLoc;
                        (snapPoint.data as SnapLocation).m_containedObject = this;
                    }
                    else
                    {
                        x = Input.mouseX;
                        y = Input.mouseY;
                    }
                }
                else
                {
                    if (snapLoc != null)
                        if ((snapPoint.data as SnapLocation).m_containedObject == this)
                            (snapPoint.data as SnapLocation).m_containedObject = null;
                    x = Input.mouseX;
                    y = Input.mouseY;
                }
            }

            if (time < 1f)
            {
                if (!drag)
                    base.Update(a_dt);
            }
            else
                m_detector.Clear();

            if (!on)
            {
                time += a_dt;
            }

            if (time > 5)
            {
                time = 0;
                on = true;
            }
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_sprite);
        }

        public Vector2 CalcForce(GameObject a_other)
        {
            Vector2 toMe = screenPosition - a_other.screenPosition;
            float force = Mathf.Pow(Mathf.Max((m_reach - toMe.magnitude), 15f), 2f) / (m_reach * m_reach * 2);
            return toMe.normal * Mathf.Min(force, 1f)*0.4f;
        }
    }
}
