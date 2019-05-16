using System;
using System.Collections.Generic;
using System.Linq;
using GLXEngine.Core;

namespace GLXEngine
{
    public class QuadTree
    {
        public struct Point
        {
            public Vector2 position;
            public object data;

            public float x { get { return position.x; } set { position.x = value; } }
            public float y { get { return position.y; } set { position.y = value; } }

            public Point(Vector2 a_pos, object a_data)
            {
                position = a_pos;
                data = a_data;
            }
        }

        List<Point> m_points;

        List<Point> m_allContained;

        public AARectangle m_boundary;
        public int m_capacity;
        bool m_divided;

        public int Count
        {
            get
            {
                return m_allContained.Count;
            }
            private set { }

        }

        QuadTree m_northeast;
        QuadTree m_northwest;
        QuadTree m_southeast;
        QuadTree m_southwest;

        public QuadTree(AARectangle a_boundary, int a_capacity = 4)
        {
            m_capacity = a_capacity;
            m_boundary = a_boundary;

            m_points = new List<Point>();
            m_allContained = new List<Point>();
            m_divided = false;
        }

        public QuadTree(QuadTree a_source)
        {
            m_capacity = a_source.m_capacity;
            m_boundary = new AARectangle(a_source.m_boundary);
            m_points = new List<Point>(a_source.m_points);
            m_allContained = new List<Point>(a_source.m_allContained);
            m_divided = a_source.m_divided;
            if (a_source.m_divided)
            {
                m_northeast = new QuadTree(a_source.m_northeast);
                m_northwest = new QuadTree(a_source.m_northwest);
                m_southeast = new QuadTree(a_source.m_southeast);
                m_southwest = new QuadTree(a_source.m_southwest);
            }
        }

        public bool Contains(Point a_point)
        {
            return m_allContained.Contains(a_point);
        }

        public void SubDivide()
        {
            float w = m_boundary.m_width / 2;
            float h = m_boundary.m_height / 2;
            float x = m_boundary.x;
            float y = m_boundary.y;

            AARectangle ne = new AARectangle(x + w, y, w, h);
            m_northeast = new QuadTree(ne, m_capacity);
            AARectangle nw = new AARectangle(x, y, w, h);
            m_northwest = new QuadTree(nw, m_capacity);
            AARectangle se = new AARectangle(x + w, y + h, w, h);
            m_southeast = new QuadTree(se, m_capacity);
            AARectangle sw = new AARectangle(x, y + h, w, h);
            m_southwest = new QuadTree(sw, m_capacity);

            foreach (Point p in m_points)
            {
                m_northeast.Insert(p);
                m_northwest.Insert(p);
                m_southeast.Insert(p);
                m_southwest.Insert(p);
            }

            m_points.Clear();

            m_divided = true;
        }

        public bool Insert(Point a_point)
        {
            if (!m_boundary.Contains(a_point.position))
            {
                return false;
            }

            m_allContained.Add(a_point);

            if (m_points.Count < m_capacity && !m_divided)
            {
                m_points.Add(a_point);
                return true;
            }

            if (!m_divided)
            {
                SubDivide();
            }

            return (m_northeast.Insert(a_point) || m_northwest.Insert(a_point) ||
              m_southeast.Insert(a_point) || m_southwest.Insert(a_point));
        }

        public List<Point> Query(Shape range, ref List<Point> found, int color = 255)
        {
            //Game.main.UI.NoFill();
            //Game.main.UI.Stroke(color, 0, 255 - color);
            //Game.main.UI.StrokeWeight(5);
            //Game.main.UI.Rect(m_boundary.x, m_boundary.y, m_boundary.m_width, m_boundary.m_height);
            if (!range.Overlaps(m_boundary))
            {
                return found;
            }

            foreach (Point point in m_points)
            {
                if (range.Contains(point.position))
                {
                    found.Add(point);
                }
            }
            if (m_divided)
            {
                m_northwest.Query(range, ref found, Math.Max(0, color - 50));
                m_northeast.Query(range, ref found, Math.Max(0, color - 50));
                m_southwest.Query(range, ref found, Math.Max(0, color - 50));
                m_southeast.Query(range, ref found, Math.Max(0, color - 50));
            }

            return found;
        }

        public List<Point> Closest(Point a_point, int a_count = 1, float a_startingSize = 1)
        {
            // Limit to number of points in this QuadTree
            if (Count == 0)
            {
                return new List<Point>();
            }
            if (Count < a_count)
            {
                return m_points;
            }

            // optimized, expanding binary search
            // start with a small circle, rapidly expand, slowly shrink
            float radius = a_startingSize;
            float limit = 16;
            while (true)
            {
                Circle range = new Circle(a_point.x, a_point.y, radius, null);
                List<Point> points = new List<Point>();
                points = Query(range, ref points, 255);
                if (points.Count == a_count)
                {
                    return points; // Return when we hit the right size
                }
                else if (points.Count < a_count)
                {
                    radius *= 2;
                }
                else if (limit-- <= 0)
                {
                    return new List<Point>(points.Take(a_count));
                }
                else
                {
                    radius /= 3;
                }
            }
        }

        public void ForEach(Action<Point> fn)
        {
            m_points.ForEach(fn);
            if (m_divided)
            {
                m_northeast.ForEach(fn);
                m_northwest.ForEach(fn);
                m_southeast.ForEach(fn);
                m_southwest.ForEach(fn);
            }
        }


    }
}