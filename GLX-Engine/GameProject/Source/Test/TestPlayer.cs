using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using static GLXEngine.Utils;

namespace GameProject
{
    public class TestPlayer : GameObject
    {
        const float m_speed = 3;
        const float m_angularAcceleration = 5f;

        Vector2 m_movementDirection = new Vector2();

        public Sprite m_sprite;

        

        public TestPlayer(Scene a_scene) : base(a_scene)
        {
            m_sprite = new Sprite("Textures/robot64.png");
            m_sprite.SetOrigin(m_sprite.width / 2, m_sprite.height / 2);
            //m_sprite.rotation += 1;
            AddChild(m_sprite);
            Initialise();

            m_mouseHandler.OnMouseDownOnTarget += OnMouseEvent;
            m_mouseHandler.OnMouseUp += OnMouseEvent;
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_sprite, true, 0.5f);
        }

        public void MoveForward(float a_value, List<int> a_controllerID)
        {
            m_movementDirection.y += a_value;
        }

        public void MoveRight(float a_value, List<int> a_controllerIDs)
        {
            m_movementDirection.x -= a_value;
        }

        public void FaceRight(float a_value, List<int> a_controllerID)
        {
            //rotation += a_value*0.2f;
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            GameObject other = a_collisionInfo.m_collider.m_owner;
            if (!HasChild(other))
            {
                if(other.GetType() == typeof(Goal))
                {
                    Destroy();
                }
            }
        }

        public override void OnMouseEvent(GameObject a_target, MouseEventType a_eventType, Vector2 a_mousePos)
        {
            if (a_eventType == MouseEventType.MouseDownOnTarget)
                drag = true;
            else
                drag = false;
        }

        public override void Update(float a_dt)
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

                m_velocity *= 0;
                m_angularVelocity = 0;
            }


            if (m_movementDirection.sqrMagnitude > 0)
            {
                m_movementDirection.SetMagnitude(m_speed);
            }

            m_velocity += new Vector2(0, 70) * a_dt;

            m_velocity += m_movementDirection * a_dt;
            if (m_velocity.magnitude > MAX_COL_WIDTH - 10)
                m_velocity.magnitude = MAX_COL_WIDTH - 10;
            position += m_velocity;
            m_velocity -= m_movementDirection;

            m_velocity *= 0.97f;

            rotation += m_angularVelocity;
            m_angularVelocity *= 0.9f;

            //Game.main.UI.StrokeWeight(4);
            //Game.main.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_velocity.x, screenPosition.y + m_velocity.y);
            //Game.main.UI.Stroke(0, 255, 0);
            //Game.main.UI.Line(screenPosition.x, screenPosition.y, screenPosition.x + m_velocity.x * a_dt, screenPosition.y + m_velocity.y * a_dt);

            m_movementDirection *= 0;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            Game.main.UI.Text(position.ToString(), 0, 25);
            Game.main.UI.Text(m_velocity.ToString(), 0, 50);
        }
    }
}
