using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;


namespace GameProject
{
    class SnapLocation : GameObject
    {
        private static QuadTree m_pointTree;
        private static bool refreshed = true;

        public GameObject m_containedObject;

        private QuadTree.Point m_qpoint;

        AnimationSprite m_sprite;

        public SnapLocation(Scene a_scene) : base(a_scene)
        {
            m_sprite = new AnimationSprite("Textures/snap box animation.png", 3, 4, 3*4-2);
            m_sprite.frameTime = 1f/10f;
            AddChild(m_sprite);

            m_pointTree = new QuadTree(a_scene.RenderRange, 4);
            m_qpoint = new QuadTree.Point(position, this);
            Initialise();
        }

        public override void Update(float a_dt)
        {
            refreshed = false;
            m_qpoint.position = position;
            Game.main.UI.Ellipse(position.x - 5, position.y - 5, 10, 10);
        }

        public static QuadTree.Point FindPoint(Shape a_range)
        {
            List<QuadTree.Point> foundPoints = new List<QuadTree.Point>();
            m_pointTree.Query(a_range, ref foundPoints);

            Vector2 closest = null;
            QuadTree.Point ret = new QuadTree.Point(null, null);
            foreach (QuadTree.Point point in foundPoints)
            {
                if (closest == null)
                {
                    closest = point.position;
                    ret = point;
                }
                else if (Vector2.Distance(point.position, a_range.position) < Vector2.Distance(closest, a_range.position))
                {
                    closest = point.position;
                    ret = point;
                }
            }
            return ret;
        }

        public override void Render(GLContext glContext)
        {
            base.Render(glContext);
            if (!refreshed)
            {
                refreshed = true;
                m_pointTree = new QuadTree(m_pointTree.m_boundary, m_pointTree.m_capacity);
            }

            if (!m_pointTree.Contains(m_qpoint))
            {
                Game.main.UI.Ellipse(position.x - 1.5f, position.y - 1.5f, 3, 3);
                m_pointTree.Insert(m_qpoint);
            }
        }

    }
}
