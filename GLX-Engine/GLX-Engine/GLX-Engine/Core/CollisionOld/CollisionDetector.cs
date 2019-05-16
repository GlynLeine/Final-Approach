using GLXEngine.Core;
using System.Collections.Generic;
using System;

namespace GLXEngine
{
    public class CollisionDetector : GameObject
    {
        private readonly CollisionShape[] m_collisionShapes;

        public List<GameObject> m_collidingObjects = new List<GameObject>();

        Type[] m_ignoreTypes;

        public CollisionDetector(Scene a_scene, CollisionShape[] a_collisionShapes, Type[] a_ignoreTypes = null) : base(a_scene)
        {
            m_ignoreTypes = a_ignoreTypes;
            m_collisionShapes = a_collisionShapes;
            Initialise();

            //Game.main.OnAfterStep += Clear;
        }

        public GameObject[] GetCollidingObjects()
        {
            return m_collidingObjects.ToArray();
        }

        public void Clear()
        {
            m_collidingObjects.Clear();
        }

        public void OnCollision(CollisionInfo a_collisionInfo, Vector2 a_minimumTranslationVec, Vector2 a_pointOfImpact)
        {
            if (a_collisionInfo.m_collider.m_owner != parent)
            {
                m_collidingObjects.Add(a_collisionInfo.m_collider.m_owner);
            }
        }

        protected override Collider createCollider()
        {
            foreach(CollisionShape cs in m_collisionShapes)
                cs.m_parent = this;

            Collider cldr = new Collider(this, m_collisionShapes);
            cldr.m_hitTest = false;
            cldr.m_static = true;

            if(m_ignoreTypes != null)
                cldr.m_ignoreList.AddRange(m_ignoreTypes);

            return cldr;
        }
    }
}
