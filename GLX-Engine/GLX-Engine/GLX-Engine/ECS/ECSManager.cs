using System;
using System.Collections.Generic;
using System.Reflection;

namespace GLXEngine.ECS
{

    using EntityType = System.Type;
    using ComponentType = System.Type;
    using SystemType = System.Type;

    public class ECSManager
    {
        Dictionary<EntityType, Dictionary<ComponentType, string>> m_componentNames = new Dictionary<EntityType, Dictionary<ComponentType, string>>();
        Dictionary<EntityType, List<Entity>> m_entities = new Dictionary<EntityType, List<Entity>>();
        Dictionary<ComponentType, Dictionary<Entity, ECSComponentHandle>> m_components = new Dictionary<ComponentType, Dictionary<Entity, ECSComponentHandle>>();
        Dictionary<SystemType, ECSSystem> m_systems = new Dictionary<SystemType, ECSSystem>();

        public void AddEntity(Entity a_entity)
        {
            EntityType entityType = a_entity.GetType();

            FieldInfo[] fields = entityType.GetFields();

            if (!m_entities[entityType].Contains(a_entity))
            {
                if (!m_componentNames.ContainsKey(entityType))
                {
                    if (fields != null)
                    {
                        Dictionary<ComponentType, string> componentNames = new Dictionary<ComponentType, string>();

                        foreach (FieldInfo field in fields)
                        {
                            if (field.FieldType.IsAssignableFrom(typeof(ECSComponent)))
                            {
                                componentNames.Add(field.FieldType, field.Name);

                                if(!m_components.ContainsKey(field.FieldType))
                                    m_components.Add(field.FieldType, new Dictionary<Entity, ECSComponentHandle>());

                                m_components[field.FieldType].Add(a_entity, new ECSComponentHandle(a_entity, field));
                            }
                        }

                        if (componentNames.Count > 0)
                        {
                            m_componentNames.Add(entityType, componentNames);
                            m_entities[entityType].Add(a_entity);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<ComponentType, string> componentName in m_componentNames[entityType])
                    {
                        m_components[componentName.Key].Add(a_entity, new ECSComponentHandle(a_entity, entityType.GetField(componentName.Value)));
                    }
                }
            }
        }

        public void RemoveEntity(Entity a_entity)
        {
            m_entities[a_entity.GetType()].Remove(a_entity);
            foreach(KeyValuePair<Type, string> components in m_componentNames[a_entity.GetType()])
            {
                m_components[components.Key].Remove(a_entity);
            }
        }

        public Dictionary<Entity, ECSComponentHandle> GetComponents<T>() where T : ECSComponent
        {
            return m_components[typeof(T)];
        }

        public ECSComponentHandle GetComponent<T>(Entity a_entity) where T : ECSComponent
        {
            return m_components[typeof(T)][a_entity];
        }


    }
}
