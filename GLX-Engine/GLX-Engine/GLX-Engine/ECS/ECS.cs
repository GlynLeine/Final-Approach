using System;
using System.Collections.Generic;
using System.Reflection;

namespace GLXEngine.ECS
{
    using EntityID = System.Int32;

    public abstract class ECSComponent
    {
        bool m_mutated = true;
    }

    public abstract class ECSSystem : GameObject
    {
        protected Dictionary<Entity, EntityID> m_objectsContained;
        private Dictionary<Type, Dictionary<EntityID, ECSComponentHandle>> m_components;
        private Dictionary<Type, bool> m_checkList;

        protected ECSManager m_owner;

        protected ECSSystem(Type[] a_types)
        {
            m_objectsContained = new Dictionary<Entity, EntityID>();
            m_components = new Dictionary<Type, Dictionary<EntityID, ECSComponentHandle>>();
            m_checkList = new Dictionary<Type, bool>();

            for (int i = 0; i < a_types.Length; i++)
                if (!a_types[i].IsAssignableFrom(typeof(ECSComponent)))
                    throw new FormatException("Not all types in component list inherit from ECSComponent.");
                else
                {
                    m_components.Add(a_types[i], new Dictionary<EntityID, ECSComponentHandle>());
                    m_checkList.Add(a_types[i], false);
                }
        }

        protected Dictionary<EntityID, ECSComponentHandle<T>> GetComponents<T>() where T : ECSComponent
        {
            Dictionary<EntityID, ECSComponentHandle<T>> ret = new Dictionary<EntityID, ECSComponentHandle<T>>();
            foreach (KeyValuePair<Entity, EntityID> entity in m_objectsContained)
                ret.Add(entity.Value, new ECSComponentHandle<T>(m_components[typeof(T)][entity.Value]));

            return ret;
        }

        protected List<EntityID> GetEntityList()
        {
            return new List<EntityID>(m_objectsContained.Values);
        }


        internal void Add(Entity a_object, params ECSComponentHandle[] a_components)
        {
            EntityID id = a_object.ID;

            m_objectsContained.Add(a_object, id);
            foreach (ECSComponentHandle component in a_components)
                m_components[component.m_componentType].Add(id, component);
        }

        public void Add(Entity a_object)
        {
            Dictionary<Type, ECSComponentHandle> componentsToAdd = new Dictionary<Type, ECSComponentHandle>();

            EntityID id = a_object.ID;

            FieldInfo[] fields = a_object.GetType().GetFields();
            if (fields != null)
                foreach (FieldInfo field in fields)
                {
                    if (m_components.ContainsKey(field.FieldType))
                    {
                        m_checkList[field.FieldType] = true;
                        componentsToAdd.Add(field.FieldType, new ECSComponentHandle(a_object, field));
                    }
                }

            bool valid = true;

            foreach (bool check in m_checkList.Values)
            {
                if (!check)
                    valid = false;
            }

            if (valid)
            {
                m_objectsContained.Add(a_object, id);
                foreach (KeyValuePair<Type, ECSComponentHandle> entry in componentsToAdd)
                    m_components[entry.Key].Add(id, entry.Value);
            }

        }

        public void Remove(Entity a_object)
        {
            if (m_objectsContained.ContainsKey(a_object))
            {
                foreach (KeyValuePair<Type, Dictionary<EntityID, ECSComponentHandle>> entry in m_components)
                    entry.Value.Remove(m_objectsContained[a_object]);
                m_objectsContained.Remove(a_object);
            }
        }

        public new abstract void Update(float a_dt);
    }
}
