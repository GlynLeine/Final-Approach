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
            public GameObject m_gameObject;
            public CollisionDelegate m_onCollision;

            //------------------------------------------------------------------------------------------------------------------------
            //														ColliderInfo()
            //------------------------------------------------------------------------------------------------------------------------
            public ColliderInfo(GameObject a_gameObject, CollisionDelegate a_onCollision)
            {
                m_gameObject = a_gameObject;
                m_onCollision = a_onCollision;
            }
        }

        private QuadTree m_colliderTree;
        private List<GameObject> m_colliderList = new List<GameObject>();
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
            m_colliderList = new List<GameObject>(a_masterCollisionManager.m_colliderList);
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
                GameObject gameObject = m_colliderList[i];
                m_colliderTree.Insert(new QuadTree.Point(gameObject.screenPosition, gameObject));
            }

            for (int i = m_activeColliderList.Count - 1; i >= 0; i--)
            {
                if (i >= m_activeColliderList.Count) continue; //fix for removal in loop

                ColliderInfo info = m_activeColliderList[i];

                List<QuadTree.Point> foundColliders = new List<QuadTree.Point>();

                m_colliderTree.Query(BroadPhaseRectangle(info.m_gameObject), ref foundColliders);

                for (int j = foundColliders.Count - 1; j >= 0; j--)
                {

                    if (j >= foundColliders.Count) continue; //fix for removal in loop

                    GameObject other = foundColliders[j].data as GameObject;
                    if (info.m_gameObject != other)
                    {
                        if (info.m_gameObject.HitTest(ref other))
                        {
                            if (info.m_onCollision != null)
                                info.m_onCollision(other, info.m_gameObject.collider.m_minimumTranslationVec);

                            info.m_gameObject.collider.m_minimumTranslationVec = new Vector2();
                        }

                    }
                }
            }
        }

        private Circle BroadPhaseCircle(GameObject gameObject)
        {
            Vector2 position = gameObject.screenPosition;
            Vector2[] hullA = (gameObject.collider as BoxCollider).m_owner.GetHull();

            Vector2 velocityA = gameObject.m_velocity;

            float extendA = 0;

            foreach (Vector2 point in hullA)
            {
                if (Mathf.Abs(point.x) > extendA)
                    extendA = point.x;
                if (Mathf.Abs(point.y) > extendA)
                    extendA = point.y;
            }

            float deltaTime = Time.deltaTime;
            float radius = velocityA.magnitude * deltaTime * 0.5f + extendA;

            Vector2 center = position - velocityA * deltaTime * 0.5f;

            return new Circle(position.x, position.y, 500);
        }

        private AARectangle BroadPhaseRectangle(GameObject gameObject)
        {
            Vector2 position = gameObject.screenPosition;
            Vector2[] hullA = (gameObject.collider as BoxCollider).m_owner.GetHull();

            Vector2 velocityA = gameObject.m_velocity;

            float extendA = 0;

            foreach (Vector2 point in hullA)
            {
                if (Mathf.Abs(point.x) > extendA)
                    extendA = point.x;
                if (Mathf.Abs(point.y) > extendA)
                    extendA = point.y;
            }

            float deltaTime = Time.deltaTime;
            float radius = velocityA.magnitude * deltaTime * 0.5f + extendA;

            Vector2 center = position - velocityA * deltaTime * 0.5f;

            return new AARectangle(position.x, position.y, 100, 100);
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

                GameObject other = m_colliderList[j];
                if (gameObject != other)
                    if (gameObject.HitTest(ref other))
                        list.Add(other);

            }
            return list.ToArray();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(ref GameObject gameObject)
        {
            if (gameObject.collider != null && !m_colliderList.Contains(gameObject))
            {
                m_colliderList.Add(gameObject);
            }

            MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (info != null)
            {

                CollisionDelegate onCollision = (CollisionDelegate)Delegate.CreateDelegate(typeof(CollisionDelegate), gameObject, info, false);
                if (onCollision != null && !m_collisionReferences.ContainsKey(gameObject))
                {
                    ColliderInfo colliderInfo = new ColliderInfo(gameObject, onCollision);
                    m_collisionReferences[gameObject] = colliderInfo;
                    m_activeColliderList.Add(colliderInfo);
                }

            }
            else
            {
                validateCase(gameObject);
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
            m_colliderList.Remove(gameObject);
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

