using System;
using System.Collections.Generic;
using GLXEngine.Core;

namespace GLXEngine
{
    /// <summary>
    /// GameObject is the base class for all display objects. 
    /// </summary>
    public abstract class GameObject : Transformable
    {
        public string name;
        private Collider _collider;

        protected List<GameObject> m_children = new List<GameObject>();
        private GameObject m_parent = null;
        public Scene m_scene = null;

        public Vector2 m_velocity = new Vector2();
        public float m_angularVelocity = 0;

        protected MouseHandler m_mouseHandler;
        public bool drag = false;

        public bool visible = true;

        public float continuousRotation = 0;

        private bool m_initialised = false;

        //------------------------------------------------------------------------------------------------------------------------
        //														GameObject()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="GLXEngine.GameObject"/> class.
        /// Since GameObjects contain a display hierarchy, a GameObject can be used as a container for other objects.
        /// Other objects can be added using child commands as AddChild.
        /// </summary>
        public GameObject()
        {
            if (Game.main != null)
            {
                m_scene = Game.main;
            }
        }

        public GameObject(Scene a_scene)
        {
            m_scene = a_scene;
            m_mouseHandler = new MouseHandler(this);
        }

        public bool p_initialised
        {
            get { return m_initialised; }
        }

        public void Initialise()
        {
            if (m_initialised)
                return;
            m_initialised = true;
            _collider = createCollider();
            if (m_scene != null)
                m_scene.Add(this);
        }

        public void ReInitialise()
        {
            if (!m_initialised)
                return;
            m_initialised = true;
            _collider = createCollider();
            if (m_scene != null)
            {
                m_scene.Remove(this);
                m_scene.Add(this);
            }
        }

        public virtual void Update(float a_dt){ }

        public virtual void OnMouseEvent (GameObject a_target, MouseEventType a_eventType, Vector2 a_mousePos) { }

        /// <summary>
        /// Return the collider to use for this game object, null is allowed 
        /// </summary>
        protected virtual Collider createCollider()
        {
            return null;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Index
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of this object in the parent's hierarchy list.
        /// Returns -1 if no parent is defined.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get
            {
                if (parent == null) return -1;
                return parent.m_children.IndexOf(this);
            }
        }


        public Vector2 GetScreenVelocity()
        {
            if (m_parent != null)
            {
                return m_velocity + m_parent.GetScreenVelocity();
            }
            return m_velocity;
        }

        public float GetScreenRotation()
        {
            if (m_parent != null)
            {
                return rotation + m_parent.GetScreenRotation();
            }
            return rotation;
        }
        

        //------------------------------------------------------------------------------------------------------------------------
        //														collider
        //------------------------------------------------------------------------------------------------------------------------
        public Collider collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														game
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the game that this object belongs to. 
        /// This is a unique instance throughout the runtime of the game.
        /// Use this to access the top of the displaylist hierarchy, and to retreive the width and height of the screen.
        /// </summary>
        public Game game
        {
            get
            {
                return Game.main;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														OnDestroy()
        //------------------------------------------------------------------------------------------------------------------------
        //subclasses can use this call to clean up resources once on destruction
        protected virtual void OnDestroy()
        {
            //empty
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Destroy()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Destroy this instance, and removes it from the game. To complete garbage collection, you must nullify all 
        /// your own references to this object.
        /// </summary>
        public void Destroy()
        {
            if (!m_scene.Contains(this)) return;
            OnDestroy();

            //detach all children
            while (m_children.Count > 0)
            {
                GameObject child = m_children[0];
                if (child != null) child.Destroy();
            }
            //detatch from parent
            if (parent != null) parent.RemoveChild(this);
            //remove from scene
            if (m_scene != null) m_scene.Remove(this);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Render
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get all a list of all objects that currently overlap this one.
        /// Calling this method will test collisions between this object and all other colliders in the scene.
        /// It can be called mid-step and is included for convenience, not performance.
        /// </summary>
        public GameObject[] GetCollisions()
        {
            return game.GetGameObjectCollisions(this);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Render
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// This function is called by the renderer. You can override it to change this object's rendering behaviour.
        /// When not inside the GLXEngine package, specify the parameter as GLXEngine.Core.GLContext.
        /// This function was made public to accomodate split screen rendering. Use SetViewPort for that.
        /// </summary>
        /// <param name='glContext'>
        /// Gl context, will be supplied by internal caller.
        /// </param>
        public virtual void Render(GLContext glContext)
        {
            if (!m_initialised)
                throw new ApplicationException("This object has not had it's Initialise() function called.");

            if (visible)
            {
                glContext.PushMatrix(matrix);

                RenderSelf(glContext);
                foreach (GameObject child in GetChildren())
                {
                    child.Render(glContext);
                }

                glContext.PopMatrix();
            }
            rotation += continuousRotation;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void RenderSelf(GLContext glContext)
        {
            //if (visible == false) return;
            //glContext.PushMatrix(matrix);
            //glContext.PopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														parent
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the parent GameObject.
        /// When the parent moves, this object moves along.
        /// </summary>
        public GameObject parent
        {
            get { return m_parent; }
            set
            {
                if (m_parent != null)
                {
                    m_parent.removeChild(this);
                    m_parent = null;
                }
                m_parent = value;
                if (value != null)
                {
                    m_parent.addChild(this);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														AddChild()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the specified GameObject as a child to this one.
        /// </summary>
        /// <param name='child'>
        /// Child object to add.
        /// </param>
        public void AddChild(GameObject child)
        {
            child.parent = this;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RemoveChild()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the specified child GameObject from this object.
        /// </summary>
        /// <param name='child'>
        /// Child object to remove.
        /// </param>
        public void RemoveChild(GameObject child)
        {
            if (child.parent == this)
            {
                child.parent = null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														removeChild()
        //------------------------------------------------------------------------------------------------------------------------
        private void removeChild(GameObject child)
        {
            m_children.Remove(child);

        }

        //------------------------------------------------------------------------------------------------------------------------
        //														addChild()
        //------------------------------------------------------------------------------------------------------------------------
        private void addChild(GameObject child)
        {
            if (child.HasChild(this)) return; //no recursive adding
            m_children.Add(child);
            return;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														AddChildAt()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the specified GameObject as a child to this object at an specified index. 
        /// This will alter the position of other objects as well.
        /// You can use this to determine the layer order (z-order) of child objects.
        /// </summary>
        /// <param name='child'>
        /// Child object to add.
        /// </param>
        /// <param name='index'>
        /// Index in the child list where the object should be added.
        /// </param>
        public void AddChildAt(GameObject child, int index)
        {
            if (child.parent != this)
            {
                AddChild(child);
            }
            if (index < 0) index = 0;
            if (index >= m_children.Count) index = m_children.Count - 1;
            m_children.Remove(child);
            m_children.Insert(index, child);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HasChild()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns 'true' if the specified object is a child of this object.
        /// </summary>
        /// <param name='gameObject'>
        /// The GameObject that should be tested.
        /// </param>
        public bool HasChild(GameObject gameObject)
        {
            GameObject par = gameObject;
            while (par != null)
            {
                if (par == this) return true;
                par = par.parent;
            }
            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetChildren()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all children that belong to this object.
        /// The function returns System.Collections.Generic.List<GameObject>.
        /// </summary>
        public List<GameObject> GetChildren()
        {
            return m_children;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetChildIndex()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the specified object in this object's child list at given location.
        /// This will alter the position of other objects as well.
        /// You can use this to determine the layer order (z-order) of child objects.
        /// </summary>
        /// <param name='child'>
        /// Child.
        /// </param>
        /// <param name='index'>
        /// Index.
        /// </param>
        public void SetChildIndex(GameObject child, int index)
        {
            if (child.parent != this) AddChild(child);
            if (index < 0) index = 0;
            if (index >= m_children.Count) index = m_children.Count - 1;
            m_children.Remove(child);
            m_children.Insert(index, child);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Tests if this object overlaps the one specified. 
        /// </summary>
        /// <returns>
        /// <c>true</c>, if test was hit, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Other.
        /// </param>
        virtual public bool HitTest(ref GameObject other)
        {
            return _collider != null && other._collider != null && _collider.HitTest(ref other._collider);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTestPoint()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns 'true' if a 2D point overlaps this object, false otherwise
        /// You could use this for instance to check if the mouse (Input.mouseX, Input.mouseY) is over the object.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate to test.
        /// </param>
        /// <param name='y'>
        /// The y coordinate to test.
        /// </param>
        virtual public bool HitTestPoint(float x, float y)
        {
            return _collider != null && _collider.HitTestPoint(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														TransformPoint()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Transforms the point from local to global space.
        /// If you insert a point relative to the object, it will return that same point relative to the game.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate to transform.
        /// </param>
        /// <param name='y'>
        /// The y coordinate to transform.
        /// </param>
        public override Vector2 TransformPoint(float x, float y)
        {
            Vector2 ret = base.TransformPoint(x, y);
            if (parent == null)
            {
                return ret;
            }
            else
            {
                return parent.TransformPoint(ret.x, ret.y);
            }
        }

        public Vector2 TransformPoint(Vector2 a_position)
        {
            Vector2 ret = base.TransformPoint(a_position.x, a_position.y);
            if (parent == null)
            {
                return ret;
            }
            else
            {
                return parent.TransformPoint(ret.x, ret.y);
            }
        }

        public Vector2 screenPosition
        {
            get
            {
                return TransformPoint(0f, 0f);
            }

            set
            {
                position = InverseTransformPoint(value.x, value.y);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //												InverseTransformPoint()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Transforms the point from global into local space.
        /// If you insert a point relative to the stage, it will return that same point relative to this GameObject.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate to transform.
        /// </param>
        /// <param name='y'>
        /// The y coordinate to transform.
        /// </param>
        public override Vector2 InverseTransformPoint(float x, float y)
        {
            Vector2 ret = base.InverseTransformPoint(x, y);
            if (parent == null)
            {
                return ret;
            }
            else
            {
                return parent.InverseTransformPoint(ret.x, ret.y);
            }
        }

        public Vector2 InverseTransformPoint(Vector2 a_position)
        {
            Vector2 ret = base.InverseTransformPoint(a_position.x, a_position.y);
            if (parent == null)
            {
                return ret;
            }
            else
            {
                return parent.InverseTransformPoint(ret.x, ret.y);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ToString()
        //------------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return "[" + this.GetType().Name + "::" + name + "]";
        }

    }
}

