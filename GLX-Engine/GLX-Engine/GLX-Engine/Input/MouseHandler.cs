using System;
using GLXEngine.Core;

namespace GLXEngine
{
    /**
	 * Simple MouseHandler interface between the Input class and your own code, that turns the Input.mouseX Input.mouseY
	 * into a more event based approach.
	 * 
	 * Do not forget to unsubscribe manually from each event before you set the handler to null.
	 */
    public class MouseHandler
    {
        //the target to generate mouse events for
        private GameObject m_target;
        //reference to the scene the target is a part of in order to register for update events
        private Scene m_scene;

        //declaration of the delegate type for all types of mouse events
        public delegate void OnMouseEvent(GameObject a_target, MouseEventType a_eventType, Vector2 a_mousePos);

        //generated the moment/frame you PRESS down the mouse, no matter where
        public event OnMouseEvent OnMouseDown;
        //generated the moment/frame you PRESS down the mouse, ON the target
        public event OnMouseEvent OnMouseDownOnTarget;

        //generated the moment/frame you RELEASE the mouse, no matter where
        public event OnMouseEvent OnMouseUp;
        //generated the moment/frame you RELEASE the mouse, ON the target
        public event OnMouseEvent OnMouseUpOnTarget;

        //generated the moment/frame you move the mouse, no matter where
        public event OnMouseEvent OnMouseMove;
        //generated the moment/frame you move the mouse, ON the target
        public event OnMouseEvent OnMouseMoveOnTarget;

        //generated the moment you move the mouse on the target
        public event OnMouseEvent OnMouseOverTarget;
        //generated the moment you move the mouse off the target
        public event OnMouseEvent OnMouseOffTarget;

        //generated if the mouse went down and up on this target
        public event OnMouseEvent OnMouseClick;

        //internal administration variables
        private bool _wasOnTarget = false;              //was the mouse on target last frame?
        private bool _wasMouseDown = false;             //was the mouse down last time we checked?
        private bool _wasMouseDownOnTarget = false;     //was the mouse down on the target last time we checked?
        private float _lastX;                           //where was the mouse last time we checked?
        private float _lastY;                           //where was the mouse last time we checked?

        //what is the difference between the origin of the target and our on target mouse down location?
        //this allows us to use the mouse down point as drag point instead of seeing the target jump due
        //to the fact that it sets its origin to the mouse down point.
        //eg instead of:
        //position.Set (Input.mouseX, Input.mouseY);
        //you can do
        //position.Set (Input.mouseX + _mouseHandler.offsetToTarget.x, Input.mouseY + _mouseHandler.offsetToTarget.y);
        private Vector2 _offset = new Vector2();

        /// <summary>
        /// Create a new MouseHandler for the given target.
        /// </summary>
        /// <param name="a_target">Target.</param>
        public MouseHandler(GameObject a_target)
        {
            m_target = a_target;
            m_scene = a_target.m_scene;
            m_scene.OnAfterStep += HandleOnStep;
            _lastX = Input.mouseX;
            _lastY = Input.mouseY;
        }

        /// <summary>
        /// Updates the internal adminstrates and triggers events where required.
        /// </summary>
        void HandleOnStep()
        {
            Vector2 mousePos = new Vector2(Input.mouseX, Input.mouseY);

            //mouse can enter/leave target without moving (the target may move!)
            bool isOnTarget = m_target.HitTestPoint(Input.mouseX, Input.mouseY);
            if (isOnTarget && !_wasOnTarget)
            {
                OnMouseOverTarget?.Invoke(m_target, MouseEventType.MouseOverTarget, mousePos);
            }
            else if (!isOnTarget && _wasOnTarget)
            {
                OnMouseOffTarget?.Invoke(m_target, MouseEventType.MouseOffTarget, mousePos);
            }

            //did we just press the mouse down?
            if (!_wasMouseDown && Input.GetMouseButton(0))
            {
                System.Console.WriteLine(m_target.HitTestPoint(Input.mouseX, Input.mouseY));
                OnMouseDown?.Invoke(m_target, MouseEventType.MouseDown, mousePos);

                if (isOnTarget && OnMouseDownOnTarget != null)
                    OnMouseDownOnTarget(m_target, MouseEventType.MouseDownOnTarget, mousePos);

                _wasMouseDown = true;
                _wasMouseDownOnTarget = isOnTarget;

                _offset = m_target.TransformPoint(0, 0);
                _offset.x = _offset.x - Input.mouseX;
                _offset.y = _offset.y - Input.mouseY;

            }
            else if (_wasMouseDown && !Input.GetMouseButton(0))
            {
                OnMouseUp?.Invoke(m_target, MouseEventType.MouseUp, mousePos);

                if (isOnTarget && OnMouseUpOnTarget != null)
                    OnMouseUpOnTarget(m_target, MouseEventType.MouseUpOnTarget, mousePos);

                if (isOnTarget && _wasMouseDownOnTarget && OnMouseClick != null)
                    OnMouseClick(m_target, MouseEventType.MouseClick, mousePos);

                _wasMouseDown = false;
                _wasMouseDownOnTarget = false;
                _offset.x = _offset.y = 0;
            }

            if (_lastX != Input.mouseX || _lastY != Input.mouseY)
            {
                _lastX = Input.mouseX;
                _lastY = Input.mouseY;

                OnMouseMove?.Invoke(m_target, MouseEventType.MouseMove, mousePos);

                if (isOnTarget && OnMouseMoveOnTarget != null)
                    OnMouseMoveOnTarget(m_target, MouseEventType.MouseMoveOnTarget, mousePos);
            }

            _wasOnTarget = isOnTarget;
        }

        //contains offset from mouse to target on click
        public Vector2 offsetToTarget
        {
            get { return _offset; }
        }

        ~MouseHandler()
        {
            m_scene.OnAfterStep -= HandleOnStep;

        }


    }

    public enum MouseEventType
    {
        MouseDown,
        MouseDownOnTarget,
        MouseUp,
        MouseUpOnTarget,
        MouseMove,
        MouseMoveOnTarget,
        MouseOverTarget,
        MouseOffTarget,
        MouseClick
    }

}

