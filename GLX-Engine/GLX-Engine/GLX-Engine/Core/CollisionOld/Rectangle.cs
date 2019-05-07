using System;
using System.Collections.Generic;

namespace GLXEngine.Core
{
    public class Rectangle : Shape
    {
        public float m_width, m_height;
        public float m_angle;

        //------------------------------------------------------------------------------------------------------------------------
        //														Rectangle()
        //------------------------------------------------------------------------------------------------------------------------
        public Rectangle(float a_x, float a_y, float a_width, float a_height)
        {
            this.x = a_x;
            this.y = a_y;
            this.m_width = a_width;
            this.m_height = a_height;
        }

        public Rectangle(Rectangle a_source)
        {
            x = a_source.x;
            y = a_source.y;
            m_width = a_source.m_width;
            m_height = a_source.m_height;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Properties()
        //------------------------------------------------------------------------------------------------------------------------
        public float left { get { return x; } set { x = value; } }
        public float right { get { return x + m_width; } set { m_width = value - x; } }
        public float top { get { return y; } set { y = value; } }
        public float bottom { get { return y + m_height; } set { m_height = value - y; } }

        public override bool Contains(Vector2 a_point, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            a_point -= position;
            a_point.angle -= m_angle;

            Vector2 min = new Vector2(left, top);
            Vector2 max = new Vector2(right, bottom);
            Vector2 closestPoint = Vector2.Clamp(position, min, max);

            if (a_point.x >= left && a_point.x <= right && a_point.y >= top && a_point.y <= bottom)
            {
                if(Mathf.Abs(left - a_point.x) < Mathf.Abs(right - a_point.x))
                    o_mtv.x = left - a_point.x;
                else
                    o_mtv.x = right - a_point.x;

                if(Mathf.Abs(top - a_point.y) < Mathf.Abs(bottom - a_point.y))
                    o_mtv.y = top - a_point.y;
                else
                    o_mtv.y = bottom - a_point.y;

                a_point.angle += m_angle;
                a_point += position;
                return true;
            }

            a_point.angle += m_angle;
            a_point += position;
            return false;
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            Type otherType = a_other.GetType();
            if(otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv);
            }
            else if(otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out o_mtv);
            }
            else if(otherType.IsAssignableFrom(typeof(Line)))
            {
                return (a_other as Line).Overlaps(this, out o_mtv);
            }

            return false;
        }

        public bool Overlaps(Rectangle a_other, out Vector2 o_mtv)
        {
            Vector2[] hullA = { new Vector2(left, top), new Vector2(right, top), new Vector2(right, bottom), new Vector2(left, bottom) };
            Vector2[] hullB = { new Vector2(a_other.left, a_other.top), new Vector2(a_other.right, a_other.top), new Vector2(a_other.right, a_other.bottom), new Vector2(a_other.left, a_other.bottom) };

            List<Vector2> axes = new List<Vector2>();

            #region Get Axes
            for (int i = 0; i < hullA.Length; i++)
            {
                Vector2 a = hullA[i];
                Vector2 b = hullA[(i + 1) % hullA.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);

                if (!axes.Contains(normal) && !axes.Contains(-normal))
                    axes.Add(normal);
            }

            for (int i = 0; i < hullB.Length; i++)
            {
                Vector2 a = hullB[i];
                Vector2 b = hullB[(i + 1) % hullB.Length];
                Vector2 normal = (b - a).normal;
                normal = new Vector2(normal.y, -normal.x);

                if (!axes.Contains(normal) && !axes.Contains(-normal))
                    axes.Add(normal.Normalize());
            }
            #endregion

            o_mtv = new Vector2(float.MaxValue, float.MaxValue);
            //o_poc = null;

            for (int i = 0; i < axes.Count; i++)
            {
                float minA = float.MaxValue, maxA = float.MinValue, minB = float.MaxValue, maxB = float.MinValue;
                for (int j = 0; j < hullA.Length; j++)
                {
                    float projection = (hullA[j]).Dot(axes[i]);
                    if (projection < minA)
                        minA = projection;
                    if (projection > maxA)
                        maxA = projection;
                }
                for (int j = 0; j < hullB.Length; j++)
                {
                    float projection = (hullB[j]).Dot(axes[i]);
                    if (projection < minB)
                        minB = projection;
                    if (projection > maxB)
                        maxB = projection;
                }

                if (!(minA <= maxB && maxA >= minB))
                {
                    o_mtv = new Vector2();
                    return false;
                }

                float overlap = maxA < maxB ? -(maxA - minB) : (maxB - minA);
                if (Mathf.Abs(overlap) < o_mtv.magnitude)
                {
                    o_mtv = axes[i] * overlap;
                }

                //if (maxA < maxB)
                //{
                //    if (o_poc == null)
                //    {
                //        o_poc = axes[i] * (maxA + (overlap / 2f));
                //    }
                //    else
                //    {
                //        o_poc += axes[i] * (maxA + (overlap / 2f));
                //        o_poc /= 2f;
                //    }
                //}
            }

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ToString()
        //------------------------------------------------------------------------------------------------------------------------
        override public string ToString()
        {
            return (x + "," + y + "," + m_width + "," + m_height);
        }

    }
}

