using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;

namespace GameProject
{
    class Enemy : Boid
    {
        Player m_player;

        float m_animTimeBuffer = 0;
        float m_animFrameTime = 0.15f;
        AnimationSprite m_walkAnim = new AnimationSprite("Textures/enemyWalkAnim.png", 6, 1);

        Sprite m_deadSprite = new Sprite("Textures/enemyDead.png");

        Hp m_hp;
        EasyDraw m_canvas;

        Sound m_deathSound;
        SoundChannel m_deathSoundChannel;

        float m_reactionScalar;

        Gun m_gun;

        public Enemy(Scene a_scene, Player a_player, ref List<GameObject> a_enemies, EasyDraw a_canvas, float a_reactionScaler) : base(a_scene, ref a_enemies, a_canvas)
        {
            m_reactionScalar = a_reactionScaler;

            if (m_reactionScalar > 0.25)
            {
                m_walkAnim = new AnimationSprite("Textures/enemyBlackWalkAnim.png", 6, 1);
                m_deadSprite = new Sprite("Textures/enemyBlackDead.png");
                collider = new Collider(this, m_walkAnim);
            }

            m_walkAnim.SetOrigin(m_walkAnim.width / 2, m_walkAnim.height / 2);
            m_walkAnim.x -= 10;
            m_walkAnim.y -= 12;
            m_walkAnim.rotation = 50;
            AddChild(m_walkAnim);

            m_deadSprite.SetOrigin(m_deadSprite.width / 2, m_deadSprite.height / 2);
            m_deadSprite.x -= 10;
            m_deadSprite.y -= 12;
            m_deadSprite.rotation = 50;
            m_deadSprite.visible = false;
            AddChild(m_deadSprite);

            m_gun = new Carbine(a_scene, this, a_player);
            m_gun.SetActive(true);
            m_gun.y += 10;
            m_gun.x += 20;
            AddChild(m_gun);

            m_player = a_player;
            m_hp = new Hp();

            m_deathSound = new Sound("Audio/crash.wav");

            m_canvas = a_canvas;
            //(collider as BoxCollider).m_canvas = a_canvas;
        }

        protected override Collider createCollider()
        {
            return new Collider(this, m_walkAnim);
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            GameObject other = a_collisionInfo.m_collider.m_owner;
            if (other is Bullet)
            {
                Bullet bullet = other as Bullet;
                if (bullet.m_owner.GetType().Equals(typeof(Player)))
                {
                    other.Destroy();
                    m_hp.current -= bullet.m_damage;
                }
            }
            else if (!HasChild(other) && !(other is PickUp))
            {
                if (other is WallTile)
                {
                    Vector2 normal = (position - other.position).normal;
                    m_force += Seperate(new List<Vector2> { other.position + Vector2.RandomFromAngle(normal.angle + 90, normal.angle - 90) }, 100);
                }

                position += a_minimumTranslationVec;
            }
        }

        protected override void OnDestroy()
        {
            ((Overworld)m_scene).enemies.Remove(this);
            base.OnDestroy();
        }

        public override void Update(float a_dt)
        {
            if (m_hp.current <= 0)
            {
                if (m_deathSoundChannel == null)
                {
                    (game as Program).score += 1;
                    m_deathSoundChannel = m_deathSound.Play();
                }
                m_deadSprite.visible = true;
                m_walkAnim.visible = false;

                if (!m_deathSoundChannel.IsPlaying)
                    Destroy();
                return;
            }

            Behaviour();

            Dodge();

            Move(a_dt);

            m_animTimeBuffer += a_dt;
            if (m_animTimeBuffer >= m_animFrameTime)
            {
                m_animTimeBuffer -= m_animFrameTime;
                m_walkAnim.NextFrame();
            }
        }

        private void OffScreenCheck()
        {
            if (!game.InView(m_walkAnim.GetExtents()))
            {
                Vector2 screenEdge = screenPosition;
                screenEdge.x = Mathf.Clamp(screenEdge.x, 0, game.width);
                screenEdge.y = Mathf.Clamp(screenEdge.y, 0, game.height);

                Vector2 direction = (screenPosition - screenEdge).normal;
                direction.angle += 90;

                Vector2 tip = new Vector2(screenEdge.x,
                                          screenEdge.y);

                Vector2 left = new Vector2(screenEdge.x - (10f * direction.x) - (10f * direction.y),
                                           screenEdge.y + (10f * direction.x) - (10f * direction.y));

                Vector2 right = new Vector2(screenEdge.x + (10f * direction.x) - (10f * direction.y),
                                            screenEdge.y + (10f * direction.x) + (10f * direction.y));

                float playerDistance = Vector2.Distance(screenPosition, m_player.screenPosition);

                m_canvas.Stroke(0);
                m_canvas.StrokeWeight(1);
                if (HasChild(m_gun))
                    m_canvas.Fill(255, 0, 0);
                else
                    m_canvas.Fill(255, 255, 0);
                m_canvas.Triangle(tip.x, tip.y, left.x, left.y, right.x, right.y);

                string distanceString = (Mathf.Round(playerDistance / 10f)).ToString();

                m_canvas.TextSize(12);

                Vector2 textDim = new Vector2(m_canvas.TextWidth(distanceString) / 2,
                                              m_canvas.TextHeight(distanceString) / 2);
                Vector2 textPos = new Vector2(tip.x - ((35f + textDim.x) * direction.y),
                                              tip.y + ((35f + textDim.y) * direction.x));

                m_canvas.Fill(255);
                m_canvas.Stroke(0);
                m_canvas.Rect(textPos.x, textPos.y, textDim.x * 2.5f, textDim.y * 2.5f);

                m_canvas.Fill(0);
                m_canvas.Text(distanceString, textPos.x, textPos.y);

                if (direction.angle % 90 == 0)
                {
                    float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
                    Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);


                    left.x = screenEdge.x - (m_hp.current / 2 * direction.x) - (m_walkAnim.height / 2 * direction.y);
                    left.y = screenEdge.y + (m_walkAnim.height / 2 * direction.x) + (m_hp.current / 2 * direction.y);

                    right.x = screenEdge.x + (m_hp.current / 2 * direction.x) - (m_walkAnim.height / 2 * direction.y);
                    right.y = screenEdge.y + (m_walkAnim.height / 2 * direction.x) - (m_hp.current / 2 * direction.y);

                    Vector2 bottomLeft = new Vector2(left.x - (5f * direction.y), left.y + (5f * direction.x));
                    Vector2 bottomRight = new Vector2(right.x - (5f * direction.y), right.y + (5f * direction.x));

                    m_canvas.Fill(color.x, color.y, 0);
                    m_canvas.Quad(left.x, left.y, right.x, right.y, bottomRight.x, bottomRight.y, bottomLeft.x, bottomLeft.y);
                }


                Vector2 toScreen = screenEdge - screenPosition;
                Vector2 playerToEdge = screenEdge - m_player.screenPosition;
                if (m_velocity.angle <= toScreen.angle + 90 && m_velocity.angle >= toScreen.angle - 90)
                {
                    m_maxSpeed = 400;
                }
                else
                {
                    m_maxSpeed = 50;
                }
            }
            else
            {
                m_maxSpeed = 400;
            }
        }

        private void Move(float a_dt)
        {
            m_velocity += m_force;

            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity.magnitude = m_maxSpeed;

            position += m_velocity * a_dt;

            m_force *= 0;
        }

        private void Behaviour()
        {
            if (HasChild(m_gun))
            {
                Flock(100, 0, 400);
            }

            if (m_player.m_hp.current > 0)
            {
                Vector2 screenEdge = screenPosition + ((screenPosition - m_player.screenPosition).normal * game.width);
                screenEdge.x = Mathf.Clamp(screenEdge.x, 0, game.width);
                screenEdge.y = Mathf.Clamp(screenEdge.y, 0, game.height);

                float screenDistance = Vector2.Distance(screenEdge, m_player.screenPosition);

                if (HasChild(m_gun))
                {
                    Flock(new List<GameObject> { m_player }, screenDistance * 0.65f, 0, float.MaxValue);

                    int rng = Utils.Random(0, (int)Mathf.Max(10, 200 - (m_scene as Overworld).difficulty));
                    if (rng <= 1 && Vector2.Distance(position, m_player.position) < 900)
                    {
                        m_gun.Shoot(false);
                    }

                    rotation = (m_player.position - position).angle;
                }
                else
                {
                    Flock(new List<GameObject> { m_player }, screenDistance * 1.5f, 0, float.MaxValue);
                    rotation = (position - m_player.position).angle;
                }
            }
        }

        private void Dodge()
        {

            List<GameObject> bullets = ((Overworld)m_scene).bullets.FindAll(a_bullet => { return ((Bullet)a_bullet).m_owner.GetType().Equals(typeof(Player)); });
            bullets = bullets.FindAll(a_bullet =>
            {
                Bullet bullet = (Bullet)a_bullet;
                float angle = Mathf.Abs(bullet.m_velocity.angle - (position - bullet.position).angle);
                return angle < 90 || (angle < 360 && angle > 270);
            });

            bullets.ForEach(a_bullet =>
            {
                Bullet bullet = (Bullet)a_bullet;
                Vector2 bulletDirection = bullet.m_velocity.normal;
                Vector2 bulletToMe = (position - bullet.position);

                Vector2 strafeDirection = new Vector2(-bulletDirection.y, bulletDirection.x).normal;
                float theta = Mathf.Abs(strafeDirection.angle - bulletToMe.angle);
                if (!(theta < 90 || (theta < 360 && theta > 270)))
                    strafeDirection *= -1;

                float projectedDistanceX = bulletDirection.Dot(bulletToMe);
                float projectedDistanceY = strafeDirection.Dot(bullet.position - position);
                if (projectedDistanceX < (200f * m_reactionScalar) && projectedDistanceX > 0 && projectedDistanceY < 0 && projectedDistanceY > -50)
                    m_force += strafeDirection * m_maxForce * 10;
            });
        }

        protected override void RenderSelf(GLContext glContext)
        {
            OffScreenCheck();

            float percentage = Mathf.Clamp(m_hp.current / m_hp.max, 0f, 1f);
            Vector2i color = new Vector2i(Mathf.Round(255 * (1 - percentage)), Mathf.Round(255 * percentage)).SetMagnitude(255);

            m_canvas.Stroke(0);
            m_canvas.Fill(color.x, color.y, 0);
            m_canvas.StrokeWeight(1);

            Vector2 left = new Vector2();
            Vector2 right = new Vector2();
            float hp = Mathf.Max(m_hp.current, 0f);
            left.x = screenPosition.x - (hp / 2);
            left.y = screenPosition.y + (m_walkAnim.height / 2);

            right.x = screenPosition.x + (hp / 2);
            right.y = screenPosition.y + (m_walkAnim.height / 2);

            Vector2 bottomLeft = new Vector2(left.x, left.y + 5f);
            Vector2 bottomRight = new Vector2(right.x, right.y + 5f);

            m_canvas.Quad(left.x, left.y, right.x, right.y, bottomRight.x, bottomRight.y, bottomLeft.x, bottomLeft.y);
        }


    }
}
