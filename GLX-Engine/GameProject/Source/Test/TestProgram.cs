using System;
using System.Drawing;
using System.Collections.Generic;
using GLXEngine;								// GLXEngine contains the engine
using GLXEngine.Core;

namespace GameProject
{
    public class TestProgram : Game
    {

        TestPlayer player;
        WallTile floor;

        public TestProgram() : base(1280, 720, false)        // Create a window that's 800x600 and NOT fullscreen
        {
            GLContext.clearColor = Color.FromArgb(109, 106, 106);

            #region Set Input
            m_keyInputHandler.CreateEvent("MoveForward");
            m_keyInputHandler.CreateEvent("MoveRight");
            m_keyInputHandler.CreateEvent("FaceRight");
            m_keyInputHandler.CreateEvent("FaceForward");
            m_keyInputHandler.CreateEvent("Shoot");

            m_keyInputHandler.CreateEvent("Test");
            m_keyInputHandler.MapEventToKeyAction("Test", Key.DIGITAL0);

            m_keyInputHandler.CreateEvent("PrintDiagnostics");

            m_keyInputHandler.MapEventToKeyAction("PrintDiagnostics", Key.TILDE);

            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.SPACE);
            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.DIGITAL0);

            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.JOYSTICK_LEFT_Y, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.W, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.S, -1f);

            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.JOYSTICK_LEFT_X, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.D, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.A, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.JOYSTICK_RIGHT_X, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.RIGHT, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.LEFT, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.JOYSTICK_RIGHT_Y, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.UP, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.DOWN, -1f);

            m_keyInputHandler.ScanObject(this);
            #endregion

            player = new TestPlayer(this);
            player.SetXY(640, 360);
            AddChild(player);

            WallTile wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(650, 296);
            AddChild(wall);
            wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(650, 360);
            AddChild(wall);
            wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(650, 424);
            AddChild(wall);

            floor = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6), width, 50);
            floor.SetXY(640, 550);
            floor.SetOrigin(width / 2f, 25);
            //floor.rotation += 20;
            AddChild(floor);

            UI.NoFill();

            Console.WriteLine(GetDiagnostics());
        }

        public void FaceRight(float a_value, List<int> a_controllerID)
        {
            floor.rotation += a_value*0.2f;
        }

        public void PrintDiagnostics(bool a_pressed)
        {
            if (!a_pressed)
                Console.WriteLine(GetDiagnostics());

            Console.WriteLine(1f / Time.deltaTime);
        }

        public void Shoot(bool a_pressed, int a_controllerID)
        {
            if (!a_pressed)
            {
                WallTile wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
                wall.SetXY(player.x, player.y);
                AddChild(wall);
            }
        }

        public override void Step()
        {
            base.Step();
        }

        void Update(float a_dt)
        {
        }

        static void Main()                          // Main() is the first method that's called when the program is run
        {
            new TestProgram().Start();                  // Create a "MyGame" and start it
        }
    }
}