using GLXEngine;
using GLXEngine.Core;
using static GLXEngine.Utils;

namespace GameProject
{
    class WallTile : BoundsObject
    {
        Sprite m_sprite;

        public WallTile(Scene a_scene, Sprite a_sprite) : base(a_scene, a_sprite.width, a_sprite.height)
        {
            m_sprite = a_sprite;
            AddChild(m_sprite);
            SetOrigin(width/2, height/2);

        }

        public WallTile(Scene a_scene, Sprite a_sprite, float a_width, float a_height) : base(a_scene, a_width, a_height)
        {
            m_sprite = a_sprite;
            m_sprite.width = a_width;
            m_sprite.height = a_height;
            m_sprite.SetOrigin(a_width/2, a_height/2);
            AddChild(m_sprite);
            SetOrigin(a_width/2 + MAX_COL_WIDTH/2, a_height);
        }

        protected override Collider createCollider()
        {
            Collider ret = base.createCollider();
            ret.m_ignoreList.Add(GetType());
            return ret;
        }

    }
}
