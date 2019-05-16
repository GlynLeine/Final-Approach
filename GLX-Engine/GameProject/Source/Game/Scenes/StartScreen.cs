using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace GameProject
{
    public class StartScreen : Scene
    {
        float insertBlinkTimeBuffer = 0;
        float insertBlinkTime = 0.5f;
        AnimationSprite insertSprite;

        Sprite loadingSprite;
        Sprite logoSprite;

        bool moveOn;
        int wait;

        EasyDraw UI;

        Sound backgroundMusic;
        public SoundChannel backgroundMusicChannel;

        public StartScreen() : base()
        {
            Start();
        }

        public override void Start()
        {
            base.Start();

            moveOn = false;
            wait = 0;

            UI = new EasyDraw(Game.main.width, Game.main.height);
            UI.clearColor = Color.FromArgb(255, 0, 0, 0);
            UI.autoClear = true;
            UI.SetOrigin(UI.width / 2, UI.height / 2);
            UI.SetXY(Game.main.width / 2, Game.main.height / 2);
            UI.TextFont(new Font("Eight-Bit Madness", 100));
            UI.TextAlign(CenterMode.Center, CenterMode.Center);
            UI.TextSize(35);
            UI.Stroke(150);
            UI.Fill(255);

            AddChild(UI);

            insertSprite = new AnimationSprite("Textures/insert.png", 2, 1);
            insertSprite.SetOrigin(insertSprite.width / 2, insertSprite.height / 2);
            insertSprite.x = game.width / 2;
            insertSprite.y = game.height - insertSprite.height / 2;
            insertSprite.visible = true;
            AddChild(insertSprite);

            loadingSprite = new Sprite("Textures/loading.png");
            loadingSprite.SetOrigin(loadingSprite.width / 2, loadingSprite.height / 2);
            loadingSprite.x = game.width / 2;
            loadingSprite.y = game.height / 2;
            loadingSprite.visible = false;
            AddChild(loadingSprite);

            logoSprite = new Sprite("Textures/startScreen.png");
            logoSprite.SetOrigin(logoSprite.width / 2, logoSprite.height / 2);
            logoSprite.x = game.width / 2;
            logoSprite.y = game.height / 2;
            logoSprite.visible = true;
            AddChild(logoSprite);


            backgroundMusic = new Sound("Audio/game_loop.wav", true);
            backgroundMusicChannel = backgroundMusic.Play();
        }

        public void Continue(Key a_key, bool a_pressed, int a_controllerID)
        {
            if (!a_pressed && m_active && m_timeActive > 1)
            {
                loadingSprite.visible = true;
                insertSprite.visible = false;
                logoSprite.visible = false;
                moveOn = true;
            }
        }


        public override void Update(float a_dt)
        {
            if (!m_active)
                return;

            if (moveOn && wait > 0)
            {
                Program program = game as Program;
                End();
                program.overworld.Restart();
            }

            if (moveOn)
                wait++;

            insertBlinkTimeBuffer += a_dt;
            if (insertBlinkTimeBuffer >= insertBlinkTime)
            {
                insertBlinkTimeBuffer -= insertBlinkTime;
                insertSprite.NextFrame();
            }
            if (moveOn)
                insertSprite.SetFrame(1);

        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);

            if (moveOn)
                UI.Clear(0);
        }

    }
}
