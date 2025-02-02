﻿using GLXEngine.Core;
using System.Collections.Generic;


namespace GLXEngine
{
    public class Boid : GameObject
    {
        protected Vector2 m_force;

        protected float m_maxSpeed = 500f;
        protected float m_maxForce = 10f;

        protected List<GameObject> m_others;

        EasyDraw m_debugDraw;

        public static bool DEBUG = false;

        public Boid(Scene a_scene, ref List<GameObject> a_others, EasyDraw a_canvas) : base(a_scene)
        {
            m_others = a_others;
            m_debugDraw = a_canvas;
        }

        public void Flock(float a_seperation, float a_alignment, float a_cohesian)
        {
            Vector2 seperation = Seperate(m_others, a_seperation);
            Vector2 alignment = Align(m_others, a_alignment);
            Vector2 cohesian = Cohesion(m_others, a_cohesian);

            seperation *= 2f;

            m_force += seperation + alignment + cohesian;
        }

        public void Flock(List<GameObject> a_objects, float a_seperation, float a_alignment, float a_cohesian)
        {
            Vector2 seperation = Seperate(a_objects, a_seperation);
            Vector2 alignment = Align(a_objects, a_alignment);
            Vector2 cohesian = Cohesion(a_objects, a_cohesian);

            seperation *= 2f;

            m_force += seperation + alignment + cohesian;
        }

        public Vector2 Cohesion(List<GameObject> a_objects, float a_cohesianDistance)
        {
            Vector2 sum = new Vector2();

            int count = 0;
            foreach (GameObject other in a_objects)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < a_cohesianDistance)
                {
                    sum += other.position;
                    count++;
                }
            }

            if (count > 0)
            {
                sum /= count;
                Vector2 cohesian = Seek(sum);
                //m_canvas.Stroke(System.Drawing.Color.Blue);
                //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + cohesian.x * 10, screenPosition.y + cohesian.y * 10);
                return cohesian;
            }
            else
            {
                return new Vector2();
            }
        }

        public Vector2 Seek(Vector2 target)
        {
            Vector2 desired = target - position;
            desired.magnitude = m_maxSpeed;
            Vector2 steer = desired - m_velocity;
            if (steer.magnitude > m_maxForce)
            {
                steer.magnitude = m_maxForce;
            }

            //m_canvas.Stroke(System.Drawing.Color.Yellow);
            //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 10, screenPosition.y + steer.y * 10);
            return steer;
        }

        public Vector2 Seperate(List<GameObject> a_objects, float a_seperationDistance, float a_seperationStrength = -1, float a_forceClamp = -1)
        {
            Vector2 steer = new Vector2();
            int count = 0;

            foreach (GameObject other in a_objects)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < a_seperationDistance)
                {
                    Vector2 diff = (position - other.position).normal;
                    diff /= distance;
                    steer += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                steer /= count;
            }

            if (steer.magnitude > 0)
            {
                if (a_seperationStrength < 0)
                    steer.magnitude = m_maxSpeed;
                else
                    steer.magnitude = a_seperationStrength;

                steer -= m_velocity;

                if (a_forceClamp < 0)
                {
                    if (steer.magnitude > m_maxForce)
                        steer.magnitude = m_maxForce;
                }
                else
                {
                    if (steer.magnitude > a_forceClamp)
                        steer.magnitude = a_forceClamp;
                }
            }

            //m_debugDraw.Stroke(System.Drawing.Color.Red);
            //m_debugDraw.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 15, screenPosition.y + steer.y * 15);
            return steer;
        }

        public Vector2 Seperate(List<Vector2> a_positions, float a_seperationDistance, float a_seperationStrength = -1, float a_forceClamp = -1)
        {
            Vector2 steer = new Vector2();
            int count = 0;

            foreach (Vector2 otherPos in a_positions)
            {
                float distance = Vector2.Distance(position, otherPos);
                if (distance > 0 && distance < a_seperationDistance)
                {
                    Vector2 diff = (position - otherPos).normal;
                    diff /= distance;
                    steer += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                steer /= count;
            }

            if (steer.magnitude > 0)
            {
                if (a_seperationStrength < 0)
                    steer.magnitude = m_maxSpeed;
                else
                    steer.magnitude = a_seperationStrength;

                steer -= m_velocity;

                if (a_forceClamp < 0)
                {
                    if (steer.magnitude > m_maxForce)
                        steer.magnitude = m_maxForce;
                }
                else
                {
                    if (steer.magnitude > a_forceClamp)
                        steer.magnitude = a_forceClamp;
                }
            }

            //m_debugDraw.Stroke(System.Drawing.Color.Red);
            //m_debugDraw.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 15, screenPosition.y + steer.y * 15);
            return steer;
        }

        public Vector2 Align(List<GameObject> a_objects, float a_alignmentDistance)
        {
            Vector2 sum = new Vector2();

            int count = 0;
            foreach (GameObject other in a_objects)
            {
                float distance = Vector2.Distance(position, other.position);
                if (distance > 0 && distance < a_alignmentDistance)
                {
                    sum += other.m_velocity;
                    count++;
                }
            }

            if (count > 0)
            {
                sum /= count;
                sum.magnitude = m_maxSpeed;
                Vector2 steer = sum - m_velocity;
                if (steer.magnitude > m_maxForce)
                {
                    steer.magnitude = m_maxForce;
                }

                //m_canvas.Stroke(System.Drawing.Color.Green);
                //m_canvas.Line(screenPosition.x, screenPosition.y, screenPosition.x + steer.x * 10, screenPosition.y + steer.y * 10);

                return steer;
            }
            else
            {
                return new Vector2();
            }
        }
    }
}
