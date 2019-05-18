using GLXEngine.Core;
using GLXEngine.Rendering.UI;
using System;
using GLXEngine.OpenGL;

namespace GLXEngine
{
    /// <summary>
    /// The Game class represents the Game window.
    /// Only a single instance of this class is allowed.
    /// </summary>
    public abstract class Game : Scene
    {
        public static Game main = null;

        public GLContext m_glContext;

        public override event RenderDelegate OnAfterRender;

        public readonly bool PixelArt;

        public UIOverlay UI;

        //------------------------------------------------------------------------------------------------------------------------
        //														Game()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="GLXEngine.Game"/> class.
        /// This class represents a game window, containing an openGL view.
        /// </summary>
        /// <param name='width'>
        /// Width of the window in pixels.
        /// </param>
        /// <param name='height'>
        /// Height of the window in pixels.
        /// </param>
        /// <param name='fullScreen'>
        /// If set to <c>true</c> the application will run in fullscreen mode.
        /// </param>
        public Game(int pWidth, int pHeight, bool pFullScreen, bool pVSync = true, int pRealWidth = -1, int pRealHeight = -1, bool pPixelArt = false) : base(new AARectangle(-pWidth * 0.5f, -pHeight * 0.5f, pWidth * 2f, pHeight * 2f))
        {
            if (pRealWidth <= 0)
            {
                pRealWidth = pWidth;
            }
            if (pRealHeight <= 0)
            {
                pRealHeight = pHeight;
            }
            PixelArt = pPixelArt;

            if (PixelArt)
            {
                // offset should be smaller than 1/(2 * "pixelsize"), but not zero:
                x = 0.01f;
                y = 0.01f;
            }

            if (main != null)
            {
                throw new Exception("Only a single instance of Game is allowed");
            }
            else
            {
                main = this;
                m_glContext = new GLContext(this);
                m_glContext.CreateWindow(pWidth, pHeight, pFullScreen, pVSync, (int)(pRealWidth*0.75), (int)(pRealHeight*0.75));

                m_renderRange = new AARectangle(0, 0, pWidth, pHeight);

                width = pWidth;
                height = pHeight;

                UI = new UIOverlay(width, height);
                UI.autoClear = true;

                //register ourselves for updates
                Add(this);

            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetViewPort()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the rendering output view port. All rendering will be done within the given rectangle.
        /// The default setting is {0, 0, game.width, game.height}.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='width'>
        /// The new width of the viewport.
        /// </param>
        /// <param name='height'>
        /// The new height of the viewport.
        /// </param>
        public void SetViewport(int x, int y, int width, int height)
        {
            // Translate from GLXEngine coordinates (origin top left) to OpenGL coordinates (origin bottom left):
            //Console.WriteLine ("Setting viewport to {0},{1},{2},{3}",x,y,width,height);
            m_glContext.SetScissor(x, game.height - height - y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowMouse()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Shows or hides the mouse cursor.
        /// </summary>
        /// <param name='enable'>
        /// Set this to 'true' to enable the cursor.
        /// Else, set this to 'false'.
        /// </param>
        public void ShowMouse(bool enable)
        {
            m_glContext.ShowCursor(enable);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Start()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Start the game loop. Call this once at the start of your game.
        /// </summary>
        public override void Start()
        {
            base.Start();
            m_glContext.Run();
        }

        bool recurse = true;

        public override void Render(GLContext glContext)
        {
            base.Render(glContext);
            //UI.Render(glContext);
            if (OnAfterRender != null && recurse)
            {
                recurse = false;
                OnAfterRender(glContext);
                recurse = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        override protected void RenderSelf(GLContext glContext)
        {
            //empty
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the width of the window.
        /// </summary>
        public override int width
        {
            get { return m_glContext.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the height of the window.
        /// </summary>
        public override int height
        {
            get { return m_glContext.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Destroy()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Destroys the game, and closes the game window.
        /// </summary>
        override protected void OnDestroy()
        {
            base.OnDestroy();
            m_glContext.Close();
        }

        public int currentFps
        {
            get
            {
                return m_glContext.currentFps;
            }
        }

        public int targetFps
        {
            get
            {
                return m_glContext.targetFps;
            }
            set
            {
                m_glContext.targetFps = value;
            }
        }

    }
}

