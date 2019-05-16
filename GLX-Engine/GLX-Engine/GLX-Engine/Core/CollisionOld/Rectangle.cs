using System;
using System.Collections.Generic;
using static GLXEngine.Utils;

namespace GLXEngine.Core
{
    public class Rectangle : CollisionShape
    {
        public float m_width, m_height;

        //------------------------------------------------------------------------------------------------------------------------
        //														Rectangle()
        //------------------------------------------------------------------------------------------------------------------------
        public Rectangle(float a_x, float a_y, float a_width, float a_height, GameObject a_parent) : base(a_parent)
        {
            this.x = a_x;
            this.y = a_y;
            this.m_width = a_width;
            this.m_height = a_height;
        }

        public Rectangle(Rectangle a_source) : base(a_source.m_parent)
        {
            x = a_source.x;
            y = a_source.y;
            m_width = a_source.m_width;
            m_height = a_source.m_height;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Properties()
        //------------------------------------------------------------------------------------------------------------------------
        #region Transformed corners
        public Vector2 p_topLeft
        {
            get
            {
                Vector2 ret = -new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return m_parent.TransformPoint(ret + position);
            }
            set
            {
                Vector2 topLeft = p_topLeft;
                Vector2 target = InverseTransformPoint(value.x, value.y);
                float toRot = target.angle - topLeft.angle;
                rotation += toRot;
                topLeft.angle += toRot;
                Vector2 transl = target - topLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomLeft
        {
            get
            {
                Vector2 ret = new Vector2(-m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return m_parent.TransformPoint(ret + position);
            }
            set
            {
                Vector2 bottomLeft = p_bottomLeft;
                Vector2 target = InverseTransformPoint(value.x, value.y);
                float toRot = target.angle - bottomLeft.angle;
                rotation += toRot;
                bottomLeft.angle += toRot;
                Vector2 transl = target - bottomLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomRight
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return m_parent.TransformPoint(ret + position);
            }
            set
            {
                Vector2 bottomRight = p_bottomRight;
                Vector2 target = InverseTransformPoint(value.x, value.y);
                float toRot = target.angle - bottomRight.angle;
                rotation += toRot;
                bottomRight.angle += toRot;
                Vector2 transl = target - bottomRight;
                position += transl;
            }
        }

        public Vector2 p_topRight
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, -m_height * 0.5f);
                ret.Rotate(rotation);
                return m_parent.TransformPoint(ret + position);
            }
            set
            {
                Vector2 topRight = p_topRight;
                Vector2 target = InverseTransformPoint(value.x, value.y);
                float toRot = target.angle - topRight.angle;
                rotation += toRot;
                topRight.angle += toRot;
                Vector2 transl = target - topRight;
                position += transl;
            }
        }
        #endregion

        #region Untransformed corners
        public Vector2 p_topLeftUT
        {
            get
            {
                Vector2 ret = -new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 topLeft = p_topLeftUT;
                float toRot = value.angle - topLeft.angle;
                rotation += toRot;
                topLeft.angle += toRot;
                Vector2 transl = value - topLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomLeftUT
        {
            get
            {
                Vector2 ret = new Vector2(-m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 bottomLeft = p_bottomLeftUT;
                float toRot = value.angle - bottomLeft.angle;
                rotation += toRot;
                bottomLeft.angle += toRot;
                Vector2 transl = value - bottomLeft;
                position += transl;
            }
        }

        public Vector2 p_bottomRightUT
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 bottomRight = p_bottomRightUT;
                float toRot = value.angle - bottomRight.angle;
                rotation += toRot;
                bottomRight.angle += toRot;
                Vector2 transl = value - bottomRight;
                position += transl;
            }
        }

        public Vector2 p_topRightUT
        {
            get
            {
                Vector2 ret = new Vector2(m_width * 0.5f, -m_height * 0.5f);
                ret.Rotate(rotation);
                return ret + position;
            }
            set
            {
                Vector2 topRight = p_topRightUT;
                float toRot = value.angle - topRight.angle;
                rotation += toRot;
                topRight.angle += toRot;
                Vector2 transl = value - topRight;
                position += transl;
            }
        }
        #endregion

        #region Hull types
        public Vector2[] p_hull
        {
            get
            {
                Vector2[] ret = { p_topLeftUT, p_topRightUT, p_bottomRightUT, p_bottomLeftUT };
                return ret;
            }
            private set { }
        }

        public Vector2[] p_extends
        {
            get
            {
                Vector2[] ret = { p_topLeft, p_topRight, p_bottomRight, p_bottomLeft };
                return ret;
            }
            private set { }
        }
        #endregion

        #region Transformed sides
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
        #endregion

        public override Vector2 GetMaxReach()
        {
            Vector2 ret =  p_topLeft - m_parent.TransformPoint(position);
            return new Vector2(Mathf.Abs(ret.x), Mathf.Abs(ret.y));
        }

        public override Vector2 ScreenPos()
        {
            return m_parent.TransformPoint(position);
        }

        public override Vector2[] GetFindPoints()
        {
            int xSplit = Mathf.Ceiling(m_width / MAX_COL_WIDTH);
            int ySplit = Mathf.Ceiling(m_height / MAX_COL_WIDTH);

            Vector2[] points = new Vector2[xSplit * ySplit];

            float rWidth = m_width / xSplit;
            float rHeight = m_height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    points[(int)(i + j * xSplit)] = m_parent.TransformPoint(position + new Vector2(i * rWidth + rWidth / 2f - m_width / 2f, j * rHeight + rHeight / 2f - m_height / 2f));

            return points;
        }

        public override void ApplyForce(Vector2 a_force, Vector2 a_poi, out Vector2 o_correctionTransl, out float o_correctionRot)
        {
            Vector2[] hull = p_extends;
            float[] forceScalars = new float[hull.Length];

            a_poi -= m_parent.screenPosition + position;

            float poiAngle = a_poi.angle;
            for (int i = 0; i < hull.Length; i++)
            {
                hull[i] -= m_parent.screenPosition + position;
                forceScalars[i] = Mathf.Abs(hull[i].angle - poiAngle);
                if (forceScalars[i] > 180)
                    forceScalars[i] -= 360;
                forceScalars[i] = 180 - Mathf.Abs(forceScalars[i]);
            }

            float totalWeight = 0;
            foreach (float weight in forceScalars)
                totalWeight += weight;

            for (int i = 0; i < forceScalars.Length; i++)
            {
                //Vector2 temp1 = hull[i] + m_parent.screenPosition + position;
                //Vector2 temp2 = (hull[i] + new Vector2(forceScalars[i] + 90) * 50) + m_parent.screenPosition + position;
                //Game.main.UI.Stroke(0);
                //Game.main.UI.StrokeWeight(1);
                //Game.main.UI.Line(temp1.x, temp1.y, temp2.x, temp2.y);
                //Vector2 temp4 = (hull[i] + new Vector2(90) * 50) + m_parent.screenPosition + position;
                //Game.main.UI.Stroke(255);
                //Game.main.UI.Line(temp1.x, temp1.y, temp4.x, temp4.y);
                //Game.main.UI.Fill(255, 100);

                //float blah = forceScalars[i];
                //bool invert = false;
                //if (blah < 0)
                //    invert = true;

                //if (invert)
                //    Game.main.UI.Arc(temp1.x - 25, temp1.y - 25, 50, 50, 90 + blah, -blah);
                //else
                //    Game.main.UI.Arc(temp1.x - 25, temp1.y - 25, 50, 50, 90, blah);

                //Game.main.UI.NoFill();
                //Game.main.UI.StrokeWeight(4);

                forceScalars[i] /= totalWeight;
            }

            o_correctionTransl = a_force;
            a_force -= m_parent.GetScreenVelocity() * Time.deltaTime;

            //Game.main.UI.Fill(255);
            //Game.main.UI.Text(m_parent.m_velocity.ToString(), 300, 400);
            //Game.main.UI.NoFill();

            //Vector2 temp = m_parent.TransformPoint(0, 0);
            //Game.main.UI.Stroke(255);
            //Game.main.UI.Line(temp.x, temp.y, temp.x + a_force.x * 10, temp.y + a_force.y * 10);

            //o_correctionTransl = null;
            o_correctionRot = float.NegativeInfinity;
            Vector2[] newHull = new Vector2[hull.Length];
            for (int i = 0; i < hull.Length; i++)
            {
                Vector2 forceApplied = a_force * forceScalars[i];

                //if(o_correctionTransl == null)
                //    o_correctionTransl = forceApplied;
                //else if(forceApplied.magnitude > o_correctionTransl.magnitude)
                //    o_correctionTransl = forceApplied;

                newHull[i] = hull[i] + forceApplied;

                //Vector2 temp1 = hull[i] + m_parent.screenPosition + position;
                //Vector2 temp2 = (hull[i] + a_force * forceScalars[i] * 10) + m_parent.screenPosition + position;
                //Game.main.UI.Stroke(255, 255, 0);
                //Game.main.UI.Line(temp1.x, temp1.y, temp2.x, temp2.y);


                if (float.IsInfinity(o_correctionRot))
                    o_correctionRot = (newHull[i].angle - hull[i].angle);
                else
                    o_correctionRot = (o_correctionRot + (newHull[i].angle - hull[i].angle));
            }

            //temp = m_parent.TransformPoint(0, 0);
            //Vector2 temp3 = new Vector2(o_correctionRot);
            //Game.main.UI.Stroke(255, 0, 255);
            //Game.main.UI.Line(temp.x, temp.y, temp.x + temp3.x * 100, temp.y + temp3.y * 100);

            //Game.main.UI.Stroke(255, 0, 0);
            //temp = (m_parent.screenPosition + position + a_poi);
            //Game.main.UI.Ellipse(temp.x - 1.5f, temp.y - 1.5f, 3, 3);

            //o_correctionRot = 0;                    //============================================================================================================================
            //o_correctionTransl = new Vector2();
        }

        public override bool Contains(Vector2 a_point, out Vector2 o_mtv, out Vector2 o_poi)
        {
            o_mtv = new Vector2();
            o_poi = new Vector2();
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
            Vector2 topLeft = -new Vector2(m_width * 0.5f, m_height * 0.5f);
            Vector2 bottomRight = new Vector2(m_width * 0.5f, m_height * 0.5f);

            float angle = rotation + m_parent.GetScreenRotation();

            a_point -= m_parent.TransformPoint(position);
            a_point.Rotate(-angle);

            if (a_point.x >= topLeft.x && a_point.x <= bottomRight.x && a_point.y >= topLeft.y && a_point.y <= bottomRight.y)
            {
                a_point = a_point.Rotate(angle) + position;
                return true;
            }

            a_point = a_point.Rotate(angle) + m_parent.TransformPoint(position);
            return false;
        }

        public override bool Overlaps(Shape a_other, out Vector2 o_mtv, out Vector2 o_poi)
        {
            o_mtv = new Vector2();
            o_poi = new Vector2();
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out o_mtv, out o_poi);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out o_mtv, out o_poi);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return (a_other as Line).Overlaps(this, out o_mtv, out o_poi);
            }

            return false;
        }

        public bool Overlaps(Rectangle a_other, out Vector2 o_mtv, out Vector2 o_poi)
        {
            Vector2[] hullA = p_extends;
            Vector2[] hullB = a_other.p_extends;

            Game.main.UI.Stroke(0, 255, 0);
            Game.main.UI.Quad(hullA[0].x, hullA[0].y, hullA[1].x, hullA[1].y, hullA[2].x, hullA[2].y, hullA[3].x, hullA[3].y);
            Game.main.UI.Quad(hullB[0].x, hullB[0].y, hullB[1].x, hullB[1].y, hullB[2].x, hullB[2].y, hullB[3].x, hullB[3].y);

            float rotA = rotation + m_parent.TransformPoint(1, 0).angle;
            float rotB = a_other.rotation + a_other.m_parent.TransformPoint(1, 0).angle;
            if (rotA % 90 == 0 && rotB % 90 == 0)
            {
                rotation -= rotA;
                a_other.rotation -= rotB;
                o_mtv = new Vector2();
                o_poi = new Vector2();
                if (a_other.p_left <= p_right && a_other.p_right >= p_left && a_other.p_bottom >= p_top && a_other.p_top <= p_bottom)
                {
                    float mag = a_other.p_left - p_right;
                    bool horizontal = true;
                    if (Mathf.Abs(mag) > Mathf.Abs(a_other.p_right - p_left))
                    {
                        mag = a_other.p_right - p_left;
                    }
                    if (Mathf.Abs(mag) > Mathf.Abs(a_other.p_bottom - p_top))
                    {
                        horizontal = false;
                        mag = a_other.p_bottom - p_top;
                    }
                    if (Mathf.Abs(mag) > Mathf.Abs(a_other.p_top - p_bottom))
                    {
                        horizontal = false;
                        mag = a_other.p_top - p_bottom;
                    }
                    if (horizontal)
                    {
                        o_mtv.x = mag;
                        o_poi = -o_mtv.normal * m_width + position;
                        o_poi = m_parent.TransformPoint(o_poi);
                    }
                    else
                        o_mtv.y = mag;

                    rotation += rotA;
                    a_other.rotation += rotB;

                    return true;
                }

                rotation += rotA;
                a_other.rotation += rotB;
                return false;
            }

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

            o_poi = (m_parent.TransformPoint(position) + a_other.m_parent.TransformPoint(a_other.position)) / 2f;
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
                    o_poi = new Vector2();
                    return false;
                }

                float overlap = maxA < maxB ? -(maxA - minB) : (maxB - minA);
                if (Mathf.Abs(overlap) < o_mtv.magnitude)
                {
                    o_mtv = axes[i] * overlap;
                }
            }
            return true;
        }

        public override bool Overlaps(Shape a_other)
        {
            Vector2 temp;
            Type otherType = a_other.GetType();
            if (otherType.IsAssignableFrom(typeof(Rectangle)))
            {
                return Overlaps(a_other as Rectangle, out temp, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Circle)))
            {
                return (a_other as Circle).Overlaps(this, out temp, out temp);
            }
            else if (otherType.IsAssignableFrom(typeof(Line)))
            {
                return (a_other as Line).Overlaps(this, out temp, out temp);
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

