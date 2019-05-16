using System.Collections.Generic;
using GLXEngine;
using GLXEngine.Core;
using static GLXEngine.Utils;

namespace GameProject
{
    class Border : GameObject
    {
        protected Rectangle m_bounds;
        int m_side;
        public Border(Scene a_scene, int a_side) : base(a_scene)
        {
            m_side = a_side % 4;

            float width = a_scene.width * 2;
            float height = a_scene.height * 2;

            switch (m_side)
            {
                case 0:
                    m_bounds = new Rectangle(a_scene.width-10 + width, height/2, width, height, this);
                    break;
                case 1:
                    m_bounds = new Rectangle(width/2, a_scene.height-10 + height, width, height, this);
                    break;
                case 2:
                    m_bounds = new Rectangle(10, height/2, width, height, this);
                    break;
                case 3:
                    m_bounds = new Rectangle(width/2, 10, width, height, this);
                    break;
            }
            Initialise();
        }

        protected override Collider createCollider()
        {
            List<CollisionShape> collisionShapes = new List<CollisionShape>();

            int xSplit = Mathf.Ceiling(m_bounds.m_width / MAX_COL_WIDTH);
            int ySplit = Mathf.Ceiling(m_bounds.m_height / MAX_COL_WIDTH);

            if (m_side % 2 == 0)
                xSplit = 1;
            else
                ySplit = 1;

            Vector2[] points = new Vector2[xSplit * ySplit];

            float rWidth = m_bounds.m_width / xSplit;
            float rHeight = m_bounds.m_height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    collisionShapes.Add(new Rectangle(i * rWidth + rWidth / 2f - m_bounds.m_width + m_bounds.x, j * rHeight + rHeight / 2f - m_bounds.m_height + m_bounds.y, rWidth, rHeight, this));

            return new Collider(this, collisionShapes);
        }

    }
}
