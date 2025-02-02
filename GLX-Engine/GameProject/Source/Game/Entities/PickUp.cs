﻿using GLXEngine;
using GLXEngine.Core;

namespace GameProject
{
    class PickUp : BoundsObject
    {
        Sprite m_sprite;

        public PickUp(Scene a_scene, string a_textureFile, string a_name) : base(a_scene)
        {
            m_sprite = new Sprite(a_textureFile);
            SetBounds(m_sprite.width, m_sprite.height);
            m_sprite.SetOrigin(width / 2, height / 2);
            SetOrigin(width / 2, height / 2);
            AddChild(m_sprite);

            name = a_name;
        }

        public PickUp(Scene a_scene, Sprite a_sprite, string a_name) : base(a_scene, a_sprite.width, a_sprite.height)
        {
            m_sprite = a_sprite;
            m_sprite.SetOrigin(width / 2, height / 2);
            SetOrigin(width / 2, height / 2);
            AddChild(m_sprite);

            name = a_name;
        }
    }
}
