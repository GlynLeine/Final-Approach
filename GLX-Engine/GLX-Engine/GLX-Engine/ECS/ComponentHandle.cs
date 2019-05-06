using System;
using System.Collections.Generic;
using System.Reflection;

namespace GLXEngine.ECS
{
    public class ECSComponentHandle<ComponentType> : ECSComponentHandle where ComponentType : ECSComponent
    {
        public ECSComponentHandle(object a_owner, FieldInfo a_component) : base(a_owner, a_component)
        { }

        public ECSComponentHandle(ECSComponentHandle a_source) : base(a_source)
        { }

        public new ComponentType value { get => (ComponentType)base.value; set => base.value = value; }
    }

    public class ECSComponentHandle
    {
        protected object m_owner;
        protected FieldInfo m_component;

        public Type m_componentType;

        public ECSComponentHandle(object a_owner, FieldInfo a_component)
        {
            m_owner = a_owner;
            m_component = a_component;
            m_componentType = a_component.FieldType;
        }

        public ECSComponentHandle(ECSComponentHandle a_source)
        {
            m_owner = a_source.m_owner;
            m_component = a_source.m_component;
            m_componentType = a_source.m_componentType;
        }

        public object value
        {
            get
            {
                return m_component.GetValue(m_owner);
            }
            set
            {
                m_component.SetValue(m_owner, value);
            }
        }
    }
}
