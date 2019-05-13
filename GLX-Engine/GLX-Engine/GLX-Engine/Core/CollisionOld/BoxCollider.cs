//using System;
//using System.Collections.Generic;

//namespace GLXEngine.Core
//{
//    public class BoxCollider : Collider
//    {
//        public new BoundsObject m_owner;

//        //------------------------------------------------------------------------------------------------------------------------
//        //														BoxCollider()
//        //------------------------------------------------------------------------------------------------------------------------		
//        public BoxCollider(BoundsObject a_owner, Type[] a_ignoreList = null)
//        {
//            m_owner = a_owner;
//            if (a_ignoreList != null)
//                m_ignoreList = new List<Type>(a_ignoreList);
//            //m_canvas = a_canvas;
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														HitTest()
//        //------------------------------------------------------------------------------------------------------------------------		
//        public override bool HitTest(ref Collider other)
//        {
//            if (other is BoxCollider)
//            {
//                BoxCollider otherCollider = other as BoxCollider;

//                if (m_owner.parent == null)
//                    return false;

//                if (otherCollider.m_owner.parent == null)
//                    return false;

//                if (m_ignoreList != null)
//                    if (m_ignoreList.Contains(otherCollider.m_owner.parent.GetType()))
//                        return false;

//                if (otherCollider.m_ignoreList != null)
//                    if (otherCollider.m_ignoreList.Contains(m_owner.parent.GetType()))
//                        return false;

//                return m_owner.Overlaps(otherCollider.m_owner, out m_minimumTranslationVec);
//            }
//            else
//            {
//                return false;
//            }
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														HitTest()
//        //------------------------------------------------------------------------------------------------------------------------		
//        public override bool HitTestPoint(Vector2 a_point)
//        {
//            Vector2[] c = m_owner.GetExtents();
//            if (c == null) return false;
//            return pointOverlapsArea(a_point, c);
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														CircleBroadPhase()
//        //------------------------------------------------------------------------------------------------------------------------
//        private bool CircleBroadPhase(Vector2[] hullB, Vector2 positionA, Vector2 velocityA, float extendA, Vector2[] hullB, Vector2 positionB, Vector2 velocityB, float extendB)
//        {
//            float deltaTime = Time.deltaTime;
//            float radiusA = velocityA.magnitude * deltaTime * 0.5f + extendA;
//            float radiusB = velocityB.magnitude * deltaTime * 0.5f + extendB;

//            Vector2 midPointA = positionA - velocityA * deltaTime * 0.5f;
//            Vector2 midPointB = positionB - velocityB * deltaTime * 0.5f;
//            return Vector2.Distance(midPointA, midPointB) <= (radiusA + radiusB);
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														LSINarrowPhase()
//        //------------------------------------------------------------------------------------------------------------------------
//        private bool LSINarrowPhase(Vector2[] hullB, Vector2 positionA, Vector2 velocityA, Vector2[] hullB, Vector2 positionB, out Vector2 colPos)
//        {
//            Vector2 closestPointA = new Vector2();
//            foreach (Vector2 point in hullB)
//                if (closestPointA == new Vector2())
//                    closestPointA = point;
//                else if (Vector2.Distance(point, positionB) < Vector2.Distance(closestPointA, positionB))
//                    closestPointA = point;

//            Vector2 lineEndA = closestPointA + velocityA;

//            Vector2 closestPointB = new Vector2();
//            foreach (Vector2 point in hullB)
//                if (closestPointB == new Vector2())
//                    closestPointB = point;
//                else if (Vector2.Distance(point, positionA) < Vector2.Distance(closestPointB, positionA))
//                    closestPointB = point;

//            Vector2 lineEndB = closestPointB + velocityA;

//            Vector2 r = lineEndA - closestPointA;
//            Vector2 s = lineEndB - closestPointB;

//            float d = r.x * s.y - r.y * s.x;
//            if (d == 0)
//            {
//                colPos = null;
//                return false;
//            }
//            float u = ((closestPointB.x - closestPointA.x) * r.y - (closestPointB.y - closestPointA.y) * r.x) / d;
//            float t = ((closestPointB.x - closestPointA.x) * s.y - (closestPointB.y - closestPointA.y) * s.x) / d;

//            colPos = closestPointA + t * r;
//            return 0 <= u && u <= 1 && 0 <= t && t <= 1;
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														SATNarrowPhase()
//        //------------------------------------------------------------------------------------------------------------------------
//        private bool SATNarrowPhase(Vector2[] hullB, Vector2 positionA, Vector2[] hullB, Vector2 positionB)
//        {
//            List<Vector2> axes = new List<Vector2>();

//            #region Get Axes
//            for (int i = 0; i < hullB.Length; i++)
//            {
//                Vector2 a = hullB[i];
//                Vector2 b = hullB[(i + 1) % hullB.Length];
//                Vector2 normal = (b - a).normal;
//                normal = new Vector2(normal.y, -normal.x);

//                Game.main.UI.Stroke(0, 255, 0);
//                Game.main.UI.Line(positionA.x, positionA.y, positionA.x + normal.x * 50, positionA.y + normal.y * 50);
//                Game.main.UI.Stroke(0);

//                if (!axes.Contains(normal) && !axes.Contains(-normal))
//                    axes.Add(normal);
//            }
//            for (int i = 0; i < hullB.Length; i++)
//            {
//                Vector2 a = hullB[i];
//                Vector2 b = hullB[(i + 1) % hullB.Length];
//                Vector2 normal = (b - a).normal;
//                normal = new Vector2(normal.y, -normal.x);

//                if (!axes.Contains(normal) && !axes.Contains(-normal))
//                    axes.Add(normal.Normalize());
//            }
//            #endregion

//            Vector2 mtv = new Vector2(float.MaxValue, float.MaxValue);

//            for (int i = 0; i < axes.Count; i++)
//            {
//                float minA = float.MaxValue, maxA = float.MinValue, minB = float.MaxValue, maxB = float.MinValue;
//                for (int j = 0; j < hullB.Length; j++)
//                {
//                    float projection = (hullB[j] + positionA).Dot(axes[i]);
//                    if (projection < minA)
//                        minA = projection;
//                    if (projection > maxA)
//                        maxA = projection;
//                }
//                for (int j = 0; j < hullB.Length; j++)
//                {
//                    float projection = (hullB[j] + positionB).Dot(axes[i]);
//                    if (projection < minB)
//                        minB = projection;
//                    if (projection > maxB)
//                        maxB = projection;
//                }

//                if (!(minA <= maxB && maxA >= minB))
//                {
//                    m_minimumTranslationVec = new Vector2();
//                    return false;
//                }

//                float overlap = maxA < maxB ? -(maxA - minB) : (maxB - minA);
//                if (Mathf.Abs(overlap) < mtv.magnitude)
//                {
//                    mtv = axes[i] * overlap;
//                }

//            }

//            m_minimumTranslationVec = (m_minimumTranslationVec + mtv) / 2;


//            Game.main.UI.Stroke(0, 0, 255);
//            Game.main.UI.Line(positionA.x, positionA.y, positionA.x + mtv.x, positionA.y + mtv.y);
//            Game.main.UI.Stroke(0);


//            return true;
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														areaOverlap()
//        //------------------------------------------------------------------------------------------------------------------------
//        private bool areaOverlap(Vector2[] c, Vector2[] d)
//        {

//            float dx = c[1].x - c[0].x;
//            float dy = c[1].y - c[0].y;
//            float lengthSQ = (dy * dy + dx * dx);

//            if (lengthSQ == 0.0f) lengthSQ = 1.0f;

//            float t, minT, maxT;

//            t = ((d[0].x - c[0].x) * dx + (d[0].y - c[0].y) * dy) / lengthSQ;
//            maxT = t; minT = t;

//            t = ((d[1].x - c[0].x) * dx + (d[1].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            t = ((d[2].x - c[0].x) * dx + (d[2].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            t = ((d[3].x - c[0].x) * dx + (d[3].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            if ((minT >= 1) || (maxT < 0)) return false;

//            dx = c[3].x - c[0].x;
//            dy = c[3].y - c[0].y;
//            lengthSQ = (dy * dy + dx * dx);

//            if (lengthSQ == 0.0f) lengthSQ = 1.0f;

//            t = ((d[0].x - c[0].x) * dx + (d[0].y - c[0].y) * dy) / lengthSQ;
//            maxT = t; minT = t;

//            t = ((d[1].x - c[0].x) * dx + (d[1].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            t = ((d[2].x - c[0].x) * dx + (d[2].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            t = ((d[3].x - c[0].x) * dx + (d[3].y - c[0].y) * dy) / lengthSQ;
//            minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

//            if ((minT >= 1) || (maxT < 0)) return false;

//            return true;
//        }

//        //------------------------------------------------------------------------------------------------------------------------
//        //														pointOverlapsArea()
//        //------------------------------------------------------------------------------------------------------------------------
//        //ie. for hittestpoint and mousedown/up/out/over
//        private bool pointOverlapsArea(Vector2 p, Vector2[] c)
//        {

//            float dx = c[1].x - c[0].x;
//            float dy = c[1].y - c[0].y;
//            float lengthSQ = (dy * dy + dx * dx);

//            float t;

//            t = ((p.x - c[0].x) * dx + (p.y - c[0].y) * dy) / lengthSQ;

//            if ((t > 1) || (t < 0)) return false;

//            dx = c[3].x - c[0].x;
//            dy = c[3].y - c[0].y;
//            lengthSQ = (dy * dy + dx * dx);

//            t = ((p.x - c[0].x) * dx + (p.y - c[0].y) * dy) / lengthSQ;

//            if ((t > 1) || (t < 0)) return false;

//            return true;
//        }
//    }
//}


