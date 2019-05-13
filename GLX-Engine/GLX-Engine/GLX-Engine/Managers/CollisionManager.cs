using System;
using System.Reflection;
using System.Collections.Generic;
using GLXEngine.Core;

namespace GLXEngine
{
    //------------------------------------------------------------------------------------------------------------------------
    //														CollisionManager
    //------------------------------------------------------------------------------------------------------------------------
    public class CollisionManager
    {
        private delegate void CollisionDelegate(GameObject a_gameObject, Vector2 a_minimumTranslationVec);

        //------------------------------------------------------------------------------------------------------------------------
        //														ColliderInfo
        //------------------------------------------------------------------------------------------------------------------------
        private struct ColliderInfo
        {
            public Collider m_collider;
            public CollisionDelegate m_onCollision;

            //------------------------------------------------------------------------------------------------------------------------
            //														ColliderInfo()
            //------------------------------------------------------------------------------------------------------------------------
            public ColliderInfo(Collider a_collider, CollisionDelegate a_onCollision)
            {
                m_collider = a_collider;
                m_onCollision = a_onCollision;
            }
        }

        private QuadTree m_colliderTree;
        private List<Collider> m_colliderList = new List<Collider>();
        private List<ColliderInfo> m_activeColliderList = new List<ColliderInfo>();
        private Dictionary<GameObject, ColliderInfo> m_collisionReferences = new Dictionary<GameObject, ColliderInfo>();

        //------------------------------------------------------------------------------------------------------------------------
        //														CollisionManager()
        //------------------------------------------------------------------------------------------------------------------------
        public CollisionManager(AARectangle a_bounds, int a_cellCapacity = 4)
        {
            m_colliderTree = new QuadTree(a_bounds, a_cellCapacity);
        }
        public CollisionManager(CollisionManager a_masterCollisionManager)
        {
            m_colliderList = new List<Collider>(a_masterCollisionManager.m_colliderList);
            m_activeColliderList = new List<ColliderInfo>(a_masterCollisionManager.m_activeColliderList);
            m_collisionReferences = new Dictionary<GameObject, ColliderInfo>(a_masterCollisionManager.m_collisionReferences);
            m_colliderTree = new QuadTree(a_masterCollisionManager.m_colliderTree);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public void Step()
        {
            m_colliderTree = new QuadTree(m_colliderTree.m_boundary, m_colliderTree.m_capacity);

            for (int i = 0; i < m_colliderList.Count; i++)
            {
                Collider collider = m_colliderList[i];
                foreach (CollisionShape shape in collider.m_shapes)
                {
                    Game.main.UI.Ellipse(shape.ScreenPos().x - 2.5f, shape.ScreenPos().y - 2.5f, 5, 5);
                    m_colliderTree.Insert(new QuadTree.Point(shape.ScreenPos(), collider));
                }
            }

            for (int i = m_activeColliderList.Count - 1; i >= 0; i--)
            {
                if (i >= m_activeColliderList.Count) continue; //fix for removal in loop

                ColliderInfo info = m_activeColliderList[i];

                List<QuadTree.Point> foundColliders = new List<QuadTree.Point>();

                m_colliderTree.Query(info.m_collider.BroadPhase(), ref foundColliders, 255);

                for (int j = foundColliders.Count - 1; j >= 0; j--)
                {

                    if (j >= foundColliders.Count) continue; //fix for removal in loop

                    Collider other = foundColliders[j].data as Collider;
                    if (info.m_collider != other)
                    {
                        if (info.m_collider.HitTest(ref other))
                        {
                            if (info.m_onCollision != null)
                                info.m_onCollision(other.m_owner, info.m_collider.m_minimumTranslationVec);

                            info.m_collider.m_minimumTranslationVec = new Vector2();
                        }

                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //												 GetCurrentCollisions()
        //------------------------------------------------------------------------------------------------------------------------
        public GameObject[] GetCurrentCollisions(GameObject gameObject)
        {
            List<GameObject> list = new List<GameObject>();
            for (int j = m_colliderList.Count - 1; j >= 0; j--)
            {

                if (j >= m_colliderList.Count) continue; //fix for removal in loop

                Collider other = m_colliderList[j];
                if (gameObject != other.m_owner)
                    if (gameObject.collider.HitTest(ref other))
                        list.Add(other.m_owner);

            }
            return list.ToArray();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(ref GameObject gameObject)
        {
            if (gameObject.collider != null && !m_colliderList.Contains(gameObject.collider))
            {
                m_colliderList.Add(gameObject.collider);

                MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (info != null)
                {

                    CollisionDelegate onCollision = (CollisionDelegate)Delegate.CreateDelegate(typeof(CollisionDelegate), gameObject, info, false);
                    if (onCollision != null && !m_collisionReferences.ContainsKey(gameObject))
                    {
                        ColliderInfo colliderInfo = new ColliderInfo(gameObject.collider, onCollision);
                        m_collisionReferences[gameObject] = colliderInfo;
                        m_activeColliderList.Add(colliderInfo);
                    }

                }
                else
                {
                    validateCase(gameObject);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														validateCase()
        //------------------------------------------------------------------------------------------------------------------------
        private void validateCase(GameObject gameObject)
        {
            MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (info != null)
            {
                throw new Exception("'OnCollision' function was not binded. Please check its case (capital O?)");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(GameObject gameObject)
        {
            m_colliderList.Remove(gameObject.collider);
            if (m_collisionReferences.ContainsKey(gameObject))
            {
                ColliderInfo colliderInfo = m_collisionReferences[gameObject];
                m_activeColliderList.Remove(colliderInfo);
                m_collisionReferences.Remove(gameObject);
            }
        }

        public string GetDiagnostics()
        {
            string output = "";
            output += "Number of colliders: " + m_colliderList.Count + '\n';
            output += "Number of active colliders: " + m_activeColliderList.Count + '\n';
            return output;
        }
    }
}

