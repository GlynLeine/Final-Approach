using System;
using System.Drawing;
using System.Collections.Generic;
using GLXEngine;								// GLXEngine contains the engine
using GLXEngine.Core;
using static GLXEngine.Utils;

namespace GameProject
{
    public class TestProgram : Game
    {
        public TestScene testScene;

        public int controller;

        public TestProgram() : base(1920, 1080, true)        // Create a window that's 800x600 and NOT fullscreen
        {
            GLContext.clearColor = Color.FromArgb(109, 106, 106);

            #region Set Input
            m_keyInputHandler.CreateEvent("MoveForward");
            m_keyInputHandler.CreateEvent("MoveRight");
            m_keyInputHandler.CreateEvent("FaceRight");
            m_keyInputHandler.CreateEvent("FaceForward");
            m_keyInputHandler.CreateEvent("Shoot");

            m_keyInputHandler.CreateEvent("Test");

            m_keyInputHandler.CreateEvent("Start");
            m_keyInputHandler.MapEventToKeyAction("Start", Key.DIGITAL1);

            m_keyInputHandler.CreateEvent("StartGame");
            m_keyInputHandler.MapEventToKeyAction("StartGame", Key.DIGITAL0);

            m_keyInputHandler.CreateEvent("PrintDiagnostics");
            m_keyInputHandler.MapEventToKeyAction("PrintDiagnostics", Key.TILDE);

            m_keyInputHandler.MapEventToKeyAction("Shoot", Key.SPACE);

            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.W, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveForward", Key.S, -1f);

            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.D, 1f);
            m_keyInputHandler.MapEventToKeyAxis("MoveRight", Key.A, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.RIGHT, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceRight", Key.LEFT, -1f);

            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.UP, 1f);
            m_keyInputHandler.MapEventToKeyAxis("FaceForward", Key.DOWN, -1f);

            m_keyInputHandler.ScanObject(this);
            #endregion

            Sprite logo = new Sprite("Textures/Andorid Assembly Logo.png");
            logo.SetOrigin(logo.width / 2, logo.height / 2);
            logo.SetXY(width / 2, height / 2);
            AddChild(logo);

            testScene = new TestScene();
            testScene.m_active = true;
            AddChild(testScene);

            UI.NoFill();

            ShowMouse(true);

            Console.WriteLine(GetDiagnostics());
        }

        public void Start(bool a_pressed, int a_controllerID)
        {
            if (!a_pressed)
            {
                controller = a_controllerID;
                GameController gc = GetController(a_controllerID);
                if (gc != null)
                {
                    gc.SendData(1, "0");
                    gc.SendData(2, "93");
                }
            }
        }

        public void StartGame(bool a_pressed, int a_controllerID)
        {
            if (!a_pressed)
            {
                testScene.Start();
            }
        }

        //public void Test(Key a_key, bool a_pressed, int a_controllerID)
        //{
        //    Console.WriteLine(a_key);
        //}

        public void PrintDiagnostics(bool a_pressed)
        {
            if (!a_pressed)
                Console.WriteLine(GetDiagnostics());

            Console.WriteLine(1f / Time.deltaTime);
        }

        //public void Shoot(bool a_pressed, int a_controllerID)
        //{
        //    if (!a_pressed)
        //    {
        //        WallTile wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
        //        wall.SetXY(player.x, player.y);
        //        AddChild(wall);
        //    }
        //}

        public override void Restart()
        {
            GetController(controller).SendData(1, 180);
            GetController(controller).SendData(2, 93);

            testScene = new TestScene();
            testScene.m_active = true;
            AddChild(testScene);
        }

        public override void Step()
        {
            base.Step();
        }

        public override void Update(float a_dt)
        {
        }

        static void Main()                          // Main() is the first method that's called when the program is run
        {
            new TestProgram().Start();                  // Create a "MyGame" and start it
        }
    }
}