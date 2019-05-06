namespace GLXEngine.ECS
{
    using EntityID = System.Int32;
    public abstract class Entity
    {
        private static EntityID lastObj = 0;

        private EntityID m_id = -1;

        public EntityID ID
        {
            get
            {
                if(m_id == -1)
                {
                    m_id = ++lastObj;
                    lastObj = m_id;
                }
                return m_id;
            }
            
            private set { }
        }
    }
}
