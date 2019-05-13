using GLXEngine.Core;
using GLXEngine.Collision;
using System.Collections.Generic;

namespace GLXEngine
{
    static class CollisionAlgorithms
    {
        public static AARectangle CalcBroadphaseAreaComplex(Vector2 a_position, Vector2 a_prevPosition, ComplexCollisionShape a_shape)
        {
            Vector2[] hullB = a_shape.m_points.ToArray();

            Vector2 velocityA = a_position - a_prevPosition;

            float extendA = 0;

            foreach (Vector2 point in hullB)
            {
                if (Mathf.Abs(point.x) > extendA)
                    extendA = point.x;
                if (Mathf.Abs(point.y) > extendA)
                    extendA = point.y;
            }

            float deltaTime = Time.deltaTime;
            float radius = velocityA.magnitude * deltaTime * 0.5f + extendA;

            Vector2 center = a_position - velocityA * deltaTime * 0.5f;

            return new AARectangle(a_position.x - 50, a_position.y - 50, 100, 100);
        }

        public static AARectangle CalcBroadphaseAreaSimple(Vector2 a_position, SimpleCollisionShape a_shape)
        {
            return null;
        }

        public static bool CircleCircle(Vector2 posA, float widthA, Vector2 posB, float widthB, out Vector2 mtv)
        {
            mtv = new Vector2();
            return false;
        }
        public static bool CircleLine()
        {
            return false;
        }

        public static bool CirclePoint()
        {
            return false;
        }

        public static bool BoxCircle()
        {
            return false;
        }

        public static bool BoxBox()
        {
            return false;
        }

        public static bool BoxLine()
        {
            return false;
        }

        public static bool BoxPoint()
        {
            return false;
        }

        public static bool AABB()
        {
            return false;
        }

        public static bool SweepCheck(Vector2[] hullA, Vector2 positionA, Vector2 prevPositionA, Vector2[] hullB, Vector2 positionB, Vector2 prevPositionB, out Vector2 colPosA, out Vector2 colPosB)
        {
            #region calculating prerequisites A
            Vector2 velocityA = positionA - prevPositionA;
            Vector2 velNormA = velocityA.normal;

            Vector2 closestPointA = new Vector2();
            foreach (Vector2 point in hullA)
            {
                Vector2 pointWorld = (point.Dot(velNormA) * velNormA) + prevPositionA;
                if (closestPointA == new Vector2())
                    closestPointA = pointWorld;
                else if (Vector2.Distance(pointWorld, prevPositionB) < Vector2.Distance(closestPointA, prevPositionB))
                    closestPointA = pointWorld;
            }

            Vector2 lineEndA = closestPointA + velocityA;
            Vector2 lineA = lineEndA - closestPointA;
            #endregion

            #region calculating prerequisites B
            Vector2 velocityB = positionB - prevPositionB;
            Vector2 velNormB = velocityB.normal;

            Vector2 closestPointB = new Vector2();
            foreach (Vector2 point in hullB)
            {
                Vector2 pointWorld = (point.Dot(velNormB) * velNormB) + prevPositionB;
                if (closestPointB == new Vector2())
                    closestPointB = pointWorld;
                else if (Vector2.Distance(pointWorld, prevPositionA) < Vector2.Distance(closestPointB, prevPositionA))
                    closestPointB = pointWorld;
            }

            Vector2 lineEndB = closestPointB - velocityB;
            Vector2 lineB = lineEndB - closestPointB;
            #endregion

            float crossArea = lineA.x * lineB.y - lineA.y * lineB.x;
            float colScalarA = ((closestPointB.x - closestPointA.x) * lineB.y - (closestPointB.y - closestPointA.y) * lineB.x) / crossArea;
            float colScalarB = ((closestPointB.x - closestPointA.x) * lineA.y - (closestPointB.y - closestPointA.y) * lineA.x) / crossArea;

            if (0 <= colScalarB && colScalarB <= 1 && 0 <= colScalarA && colScalarA <= 1)
            {
                float distanceScalar = Mathf.Min(colScalarB, colScalarA);
                colPosA = lineA * distanceScalar;
                colPosB = lineB * distanceScalar;
                return true;
            }
            colPosA = positionA;
            colPosB = positionB;
            return false;
        }

        public static bool SAT(Vector2[] hullA, Vector2 positionA, Vector2[] hullB, Vector2 positionB, out Vector2 o_mtv, out Vector2 o_poc)
        {
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
            o_poc = null;

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

                if (maxA < maxB)
                {
                    if (o_poc == null)
                    {
                        o_poc = axes[i] * (maxA + (overlap / 2f));
                    }
                    else
                    {
                        o_poc += axes[i] * (maxA + (overlap / 2f));
                        o_poc /= 2f;
                    }
                }
            }

            return true;
        }
    }
}
