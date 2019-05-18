using System.Collections.Generic;
using GLXEngine.Core;
using static GLXEngine.Utils;

namespace GLXEngine
{
    public class BoundsObject : GameObject
    {

        protected Rectangle m_bounds;

        public BoundsObject(Scene a_scene, float a_width = 0, float a_height = 0) : base(a_scene)
        {
            m_bounds = new Rectangle(a_width / 2, a_height / 2, a_width, a_height, this);
            Initialise();
        }

        protected BoundsObject() { }

        protected override Collider createCollider()
        {
            List<CollisionShape> collisionShapes = new List<CollisionShape>();

            int xSplit = Mathf.Ceiling(m_bounds.m_width / MAX_COL_WIDTH);
            int ySplit = Mathf.Ceiling(m_bounds.m_height / MAX_COL_WIDTH);

            Vector2[] points = new Vector2[xSplit * ySplit];

            float rWidth = m_bounds.m_width / xSplit;
            float rHeight = m_bounds.m_height / ySplit;

            for (int i = 0; i < xSplit; i++)
                for (int j = 0; j < ySplit; j++)
                    collisionShapes.Add(new Rectangle(i * rWidth + rWidth - m_bounds.m_width + m_bounds.x, j * rHeight + rHeight - m_bounds.m_height + m_bounds.y, rWidth, rHeight, this));

            return new Collider(this, collisionShapes);
        }

        public void SetBounds(float a_width, float a_height)
        {
            if (m_bounds != null)
            {
                m_bounds.m_width = a_width;
                m_bounds.m_height = a_height;
            }
            else
                m_bounds = new Rectangle(0, 0, a_width, a_height, this);
        }

        public Rectangle GetBounds()
        {
            return m_bounds;
        }

        public bool Overlaps(Shape a_other)
        {
            return m_bounds.Overlaps(a_other);
        }

        public bool Overlaps(Shape a_other, out Vector2 o_mtv, out Vector2 o_poi)
        {
            return m_bounds.Overlaps(a_other, out o_mtv, out o_poi);
        }

        public bool Overlaps(BoundsObject a_other)
        {
            return m_bounds.Overlaps(a_other.m_bounds);
        }

        public bool Overlaps(BoundsObject a_other, out Vector2 o_mtv, out Vector2 o_poi)
        {
            return m_bounds.Overlaps(a_other.m_bounds, out o_mtv, out o_poi);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's width in pixels.
        /// </summary>
        virtual public float width
        {
            get
            {
                if (m_bounds != null) return m_bounds.m_width * scaleX;
                return 0;
            }
            set
            {
                if (m_bounds != null && m_bounds.m_width != 0) scaleX = value / m_bounds.m_width;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's height in pixels.
        /// </summary>
        virtual public float height
        {
            get
            {
                if (m_bounds != null) return m_bounds.m_height * scaleY;
                return 0;
            }
            set
            {
                if (m_bounds != null && m_bounds.m_height != 0) scaleY = value / m_bounds.m_height;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetArea()
        //------------------------------------------------------------------------------------------------------------------------
        public Vector2[] GetHull()
        {
            return m_bounds.p_hull;
        }
        public float[] GetArea()
        {
            Vector2[] hull = m_bounds.p_hull;
            float[] ret = new float[hull.Length * 2];
            for (int i = 0; i < hull.Length; i++)
            {
                ret[i * 2] = hull[i].x;
                ret[i * 2 + 1] = hull[i].y;
            }
            return ret;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetExtents()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the four corners of this object as a set of 4 Vector2s.
        /// </summary>
        /// <returns>
        /// The extents.
        /// </returns>
        public Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(m_bounds.p_left, m_bounds.p_top);
            ret[1] = TransformPoint(m_bounds.p_right, m_bounds.p_top);
            ret[2] = TransformPoint(m_bounds.p_right, m_bounds.p_bottom);
            ret[3] = TransformPoint(m_bounds.p_left, m_bounds.p_bottom);
            return ret;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetOrigin()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the origin, the pivot point of this Sprite in pixels.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        public void SetOrigin(float x, float y)
        {
            Vector2 newPos = new Vector2(-x + width / 2, -y + height / 2);
            if (collider != null)
                collider.position = newPos;
            m_bounds.position = newPos;
        }

    }
}
