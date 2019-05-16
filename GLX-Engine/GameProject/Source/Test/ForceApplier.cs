using GLXEngine;
using GLXEngine.Core;
using System.Collections.Generic;
using System;

namespace GameProject
{
    public class ForceApplier : GameObject
    {
        protected CollisionDetector m_detector;

        public delegate Vector2 ForceCalcDelegate(GameObject a_other);

        protected List<Type> m_applyList;

        private Type[] m_ignoreTypes;

        ForceCalcDelegate m_forceCalcFunc;

        public ForceApplier(Scene a_scene, CollisionShape[] a_areaOfEffect, ForceCalcDelegate a_forceCalcFunc, Type[] a_ignoreTypes = null) : base(a_scene)
        {
            m_detector = new CollisionDetector(a_scene, a_areaOfEffect, a_ignoreTypes);
            AddChild(m_detector);
            m_forceCalcFunc = a_forceCalcFunc;

            m_ignoreTypes = a_ignoreTypes;

            Initialise();
        }

        public ForceApplier(Scene a_scene, ForceCalcDelegate a_forceCalcFunc, Type[] a_ignoreTypes = null) : base(a_scene)
        {
            m_forceCalcFunc = a_forceCalcFunc;
            m_ignoreTypes = a_ignoreTypes;
        }

        public ForceApplier(Scene a_scene, Type[] a_ignoreTypes = null) : base(a_scene)
        {
            m_ignoreTypes = a_ignoreTypes;
        }

        public void SetForceCalcFunc(ForceCalcDelegate a_forceCalcFunc)
        {
            m_forceCalcFunc = a_forceCalcFunc;
        }

        public void SetAOE(CollisionShape[] a_areaOfEffect)
        {
            if (HasChild(m_detector))
                RemoveChild(m_detector);

            m_detector = new CollisionDetector(m_scene, a_areaOfEffect, m_ignoreTypes);
            AddChild(m_detector);

            if (p_initialised)
                ReInitialise();
            else
                Initialise();
        }

        public override void Update(float a_dt)
        {
            foreach (GameObject gameObject in m_detector.GetCollidingObjects())
            {
                if (gameObject != parent)
                {
                    if (m_applyList != null)
                    {
                        if (m_applyList.Contains(gameObject.GetType()))
                            gameObject.m_velocity += m_forceCalcFunc(gameObject);
                    }
                    else
                        gameObject.m_velocity += m_forceCalcFunc(gameObject);
                }
            }
            m_detector.Clear();
        }
    }
}