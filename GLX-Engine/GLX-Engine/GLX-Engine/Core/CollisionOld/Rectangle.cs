using System;
using System.Collections.Generic;

namespace GLXEngine.Core
{
    public class Rectangle : Shape
    {
        public float m_width, m_height;

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
        public Vector2 p_topLeft
        {
            get
            {
                Vector2 ret =  -new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 topLeft = p_topLeft;
                float toRot = value.angle - topLeft.angle;
                rotation += toRot;
                topLeft.angle += toRot;
                Vector2 transl = value - topLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomLeft
        {
            get
            {
                Vector2 ret = new Vector2(-m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 bottomLeft = p_bottomLeft;
                float toRot = value.angle - bottomLeft.angle;
                rotation += toRot;
                bottomLeft.angle += toRot;
                Vector2 transl = value - bottomLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomRight
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 bottomRight = p_bottomRight;
                float toRot = value.angle - bottomRight.angle;
                rotation += toRot;
                bottomRight.angle += toRot;
                Vector2 transl = value - bottomRight;
                position += transl;
            }
        }

        public Vector2 p_topRight
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, -m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 topRight = p_topRight;
                float toRot = value.angle - topRight.angle;
                rotation += toRot;
                topRight.angle += toRot;
                Vector2 transl = value - topRight;
                position += transl;
            }
        }

        public Vector2[] p_hull
        {
            get
            {
                Vector2[] ret = { p_topLeft, p_topRight, p_bottomRight, p_bottomLeft };
                return ret;
            }
            private set { }
        }

        public float p_left
        {
            get
            {
                return p_topLeft.x;
            }
            set
            {
                Vector2 tl = p_topLeft;
                tl.x = value;
                p_topLeft = tl;
            }
        }

        public float p_right
        {
            get
            {
                return p_bottomRight.x;
            }
            set
            {
                Vector2 br = p_bottomRight;
                br.x = value;
                p_bottomRight = br;
            }
        }
        public float p_top
        {
            get
            {
                return p_topLeft.y;
            }
            set
            {
                Vector2 tl = p_topLeft;
                tl.y = value;
                p_topLeft = tl;
            }
        }
        public float p_bottom
        {
            get
            {
                return p_bottomRight.y;
            }
            set
            {
                Vector2 br = p_bottomRight;
                br.y = value;
                p_bottomRight = br;
            }
        }

        public override bool Contains(Vector2 a_point, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            a_point = (a_point - position).Rotate(-rotation);

            Vector2 topLeft = p_topLeft;
            Vector2 bottomRight = p_bottomRight;
            Vector2 closestPoint = Vector2.Clamp(position, topLeft, bottomRight);

            if (a_point.x >= topLeft.x && a_point.x <= bottomRight.x && a_point.y >= topLeft.y && a_point.y <= bottomRight.y)
            {
                if (Mathf.Abs(topLeft.x - a_point.x) < Mathf.Abs(bottomRight.x - a_point.x))
                    o_mtv.x = topLeft.x - a_point.x;
                else
                    o_mtv.x = bottomRight.x - a_point.x;

                if (Mathf.Abs(topLeft.y - a_point.y) < Mathf.Abs(bottomRight.y - a_point.y))
                    o_mtv.y = topLeft.y - a_point.y;
                else
                    o_mtv.y = bottomRight.y - a_point.y;

                a_point = a_point.Rotate(rotation) + position;
                return true;
            }

            a_point = a_point.Rotate(rotation) + position;
            return false;
        }

        public override bool Contains(Vector2 a_point)
        {
            Vector2 topLeft = p_topLeft;
            Vector2 bottomRight = p_bottomRight;

            if (rotation % 90 == 0)
                return (a_point.x >= topLeft.x && a_point.x <= bottomRight.x && a_point.y >= topLeft.y && a_point.y <= bottomRight.y);

            a_point = (a_point - position).Rotate(-rotation);

            if (a_point.x >= topLeft.x && a_point.x <= bottomRight.x && a_point.y >= topLeft.y && a_point.y <= bottomRight.y)
            {
                a_point = a_point.Rotate(rotation) + position;
                return true;
            }

            a_point = a_point.Rotate(rotation) + position;
            return false;
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv)
        {
            o_mtv = new Vector2();
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out o_mtv);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return (a_other as Line).Overlaps(this, out o_mtv);
            }

            return false;
        }

        public bool Overlaps(Rectangle a_other, out Vector2 o_mtv)
        {
            if (rotation % 90 == 0)
                if (a_other.rotation % 90 == 0)
                {
                    o_mtv = new Vector2();
                    if (a_other.p_left < p_right && a_other.p_right > p_left && a_other.p_bottom > p_top && a_other.p_top < p_bottom)
                    {
                        o_mtv.x = a_other.p_left - p_right;
                        if (Mathf.Abs(o_mtv.x) > Mathf.Abs(a_other.p_right - p_left))
                            o_mtv.x = a_other.p_right - p_left;
                        if (Mathf.Abs(o_mtv.x) > Mathf.Abs(a_other.p_bottom - p_top))
                            o_mtv = new Vector2(0, a_other.p_bottom - p_top);
                        if (Mathf.Abs(o_mtv.y) > Mathf.Abs(a_other.p_top - p_bottom))
                            o_mtv.y = a_other.p_top - p_bottom;
                        return true;
                    }
                    return false;
                }

            Vector2[] hullA = p_hull;
            Vector2[] hullB = a_other.p_hull;

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

        public override bool Overlaps(Shape a_other)
        {
            Vector2 temp;
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return (a_other as Line).Overlaps(this, out temp);
            }

            return false;
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

