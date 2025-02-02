using System;
using System.Collections.Generic;
using System.Drawing;
using GLXEngine.OpenGL;

namespace GLXEngine.Core
{

    class WindowSize
    {
        public static WindowSize instance = new WindowSize();
        public int width, height;
    }

    public class GLContext
    {

        public const int MAXKEYS = 65535;
        public const int MAXBUTTONS = 255;

        public static bool[] keys = new bool[MAXKEYS + 1];
        public static Dictionary<Key, bool> keydown = new Dictionary<Key, bool>();
        public static Dictionary<Key, bool> keyup = new Dictionary<Key, bool>();
        private static bool[] buttons = new bool[MAXBUTTONS + 1];
        private static bool[] mousehits = new bool[MAXBUTTONS + 1];
        private static bool[] mouseup = new bool[MAXBUTTONS + 1]; //mouseup kindly donated by LeonB

        public static int mouseX = 0;
        public static int mouseY = 0;

        private Game _owner;

        private int _targetFrameRate = -1;
        private float _lastFrameTime = 0;
        private float _lastFPSTime = 0;
        private int _frameCount = 0;
        private int _lastFPS = 0;
        private bool _vsyncEnabled = false;

        static Color m_clearColor = Color.FromArgb(255, 0, 0, 0);

        private static double _realToLogicWidthRatio;
        private static double _realToLogicHeightRatio;

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public GLContext(Game owner)
        {
            _owner = owner;
            _lastFPS = _targetFrameRate;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Width
        //------------------------------------------------------------------------------------------------------------------------
        public int width
        {
            get { return WindowSize.instance.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Height
        //------------------------------------------------------------------------------------------------------------------------
        public int height
        {
            get { return WindowSize.instance.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ClearColor
        //------------------------------------------------------------------------------------------------------------------------
        public static Color clearColor
        { get { return m_clearColor; } set { m_clearColor = value; GL.ClearColor(m_clearColor.R / 255f, m_clearColor.G / 255f, m_clearColor.B / 255f, m_clearColor.A / 255f); } }

        //------------------------------------------------------------------------------------------------------------------------
        //														setupWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public void CreateWindow(int width, int height, bool fullScreen, bool vSync, int realWidth, int realHeight)
        {
            // This stores the "logical" width, used by all the game logic:
            WindowSize.instance.width = width;
            WindowSize.instance.height = height;
            _realToLogicWidthRatio = (double)realWidth / width;
            _realToLogicHeightRatio = (double)realHeight / height;
            _vsyncEnabled = vSync;

            GL.glfwInit();

            GL.glfwOpenWindowHint(GL.GLFW_FSAA_SAMPLES, 8);
            GL.glfwOpenWindow(realWidth, realHeight, 8, 8, 8, 8, 24, 0, (fullScreen ? GL.GLFW_FULLSCREEN : GL.GLFW_WINDOWED));
            GL.glfwSetWindowTitle("Game");
            GL.glfwSwapInterval(vSync);

            //GL.glfwSetErrorCallback(
            //    (GL.GlfwError error, string message) =>
            //    {
            //        Console.WriteLine("OpenGL Error: " + error + " - " + message);
            //    });

            GL.glfwSetKeyCallback(
                (int _key, int _mode) =>
                {
                    Key key = (Key)_key;
                    bool press = (_mode == 1);
                    if (press)
                        if (keydown.ContainsKey(key))
                            keydown[key] = true;
                        else
                            keydown.Add(key, true);
                    else
                        if (keyup.ContainsKey(key))
                        keyup[key] = true;
                    else
                        keyup.Add(key, true);

                    keys[_key] = press;
                });

            GL.glfwSetMouseButtonCallback(
                (int _button, int _mode) =>
                {
                    bool press = (_mode == 1);
                    if (press) mousehits[_button] = true;
                    else mouseup[_button] = true;
                    buttons[_button] = press;
                });

            GL.glfwSetWindowSizeCallback((int newWidth, int newHeight) =>
            {
                GL.Viewport(0, 0, newWidth, newHeight);
                GL.Enable(GL.MULTISAMPLE);
                GL.Enable(GL.TEXTURE_2D);
                GL.Enable(GL.BLEND);
                GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                GL.Hint(GL.PERSPECTIVE_CORRECTION, GL.FASTEST);
                //GL.Enable (GL.POLYGON_SMOOTH);
                //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

                // Load the basic projection settings:
                GL.MatrixMode(GL.PROJECTION);
                GL.LoadIdentity();
                // Here's where the conversion from logical width/height to real width/height happens: 
                GL.Ortho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);

                lock (WindowSize.instance)
                {
                    WindowSize.instance.width = (int)(newWidth / _realToLogicWidthRatio);
                    WindowSize.instance.height = (int)(newHeight / _realToLogicHeightRatio);
                }
            });
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowCursor()
        //------------------------------------------------------------------------------------------------------------------------
        public void ShowCursor(bool enable)
        {
            if (enable)
            {
                GL.glfwEnable(GL.GLFW_MOUSE_CURSOR);
            }
            else
            {
                GL.glfwDisable(GL.GLFW_MOUSE_CURSOR);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetScissor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetScissor(int x, int y, int width, int height)
        {
            if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height))
            {
                GL.Disable(GL.SCISSOR_TEST);
            }
            else
            {
                GL.Enable(GL.SCISSOR_TEST);
            }

            GL.Scissor(
                (int)(x * _realToLogicWidthRatio),
                (int)(y * _realToLogicHeightRatio),
                (int)(width * _realToLogicWidthRatio),
                (int)(height * _realToLogicHeightRatio)
            );
            //GL.Scissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Close()
        //------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            GL.glfwCloseWindow();
            GL.glfwTerminate();
            System.Environment.Exit(0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Run()
        //------------------------------------------------------------------------------------------------------------------------
        public void Run()
        {
            //Update();
            GL.glfwSetTime(0.0);
            do
            {
                if ((Time.time - _lastFrameTime > 1/_targetFrameRate) || _targetFrameRate < 0 || _vsyncEnabled)
                {
                    _lastFrameTime = Time.time;

                    //actual fps count tracker
                    _frameCount++;
                    if (Time.time - _lastFPSTime > 1)
                    {
                        _lastFPS = (int)(_frameCount / (Time.time - _lastFPSTime));
                        _lastFPSTime = Time.time;
                        _frameCount = 0;
                    }

                    UpdateMouseInput();
                    _owner.Step();

                    ResetHitCounters();
                    Display();

                    if (GL.GetError() != 0)
                        Console.WriteLine("OpenGL error: " + GL.GetError());

                    Time.newFrame();
                    GL.glfwPollEvents();
                }


            } while (GL.glfwGetWindowParam(GL.GLFW_ACTIVE) == 1);
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														display()
        //------------------------------------------------------------------------------------------------------------------------
        private void Display()
        {
            GL.Clear(GL.COLOR_BUFFER_BIT);

            GL.MatrixMode(GL.MODELVIEW);
            GL.LoadIdentity();

            _owner.Render(this);

            GL.glfwSwapBuffers();
            if (GetKey(Key.ESCAPE)) this.Close();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetColor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetColor(byte r, byte g, byte b, byte a)
        {
            GL.Color4ub(r, g, b, a);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PushMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PushMatrix(float[] matrix)
        {
            GL.PushMatrix();
            GL.MultMatrixf(matrix);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PopMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PopMatrix()
        {
            GL.PopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawQuad()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawQuad(float[] vertices, float[] uv)
        {
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uv);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawArrays(GL.QUADS, 0, vertices.Length / 2);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawLine()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawLine(float[] vertices, float width)
        {
            if (vertices.Length % 2 == 0)
            {
                GL.Enable(GL.LINE_SMOOTH);
                GL.LineWidth(width);
                GL.EnableClientState(GL.VERTEX_ARRAY);
                GL.VertexPointer(2, GL.FLOAT, 0, vertices);
                GL.DrawArrays(GL.LINES, 0, vertices.Length / 2);
                GL.DisableClientState(GL.VERTEX_ARRAY);
                GL.LineWidth(1);
                GL.Disable(GL.LINE_SMOOTH);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKey()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKey(Key key)
        {
            return keys[(int)key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyDown(Key key)
        {
            return (!keydown.ContainsKey(key) ? false : keydown[key]);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyUp(Key key)
        {
            return keyup[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButton()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButton(int button)
        {
            return buttons[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonDown(int button)
        {
            return mousehits[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonUp(int button)
        {
            return mouseup[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ResetHitCounters()
        //------------------------------------------------------------------------------------------------------------------------
        public static void ResetHitCounters()
        {
            keydown.Clear();
            keyup.Clear();
            Array.Clear(mousehits, 0, MAXBUTTONS);
            Array.Clear(mouseup, 0, MAXBUTTONS);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateMouseInput()
        //------------------------------------------------------------------------------------------------------------------------
        public static void UpdateMouseInput()
        {
            GL.glfwGetMousePos(out mouseX, out mouseY);
            mouseX = (int)(mouseX / _realToLogicWidthRatio);
            mouseY = (int)(mouseY / _realToLogicHeightRatio);
        }

        public int currentFps
        {
            get { return _lastFPS; }
        }

        public int targetFps
        {
            get { return _targetFrameRate; }
            set
            {
                _targetFrameRate = value;
            }
        }

    }

}