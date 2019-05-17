using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace GameProject
{
    public class TestScene : Scene
    {

        TestPlayer player;
        Wheel wheel;

        Sound m_startMusic;
        SoundChannel m_startMusicChannel;

        public TestScene() : base()
        {
            height = game.height;
            width = game.width;
        }

        public override void Start()
        {
            #region test shit
            player = new TestPlayer(this);
            player.SetXY(width - 100, height / 2);
            player.rotation += 45;
            AddChild(player);

            WallTile wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(width / 2 + 260, height / 2 - 164);
            AddChild(wall);
            wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(width / 2 + 260, height / 2 - 100);
            AddChild(wall);
            wall = new WallTile(this, new AnimationSprite("Textures/tileSheet.png", 13, 6));
            wall.SetXY(width / 2 + 260, height / 2 - 36);
            AddChild(wall);

            wheel = new Wheel(this);
            wheel.SetXY(width / 2, height / 2);
            AddChild(wheel);

            Border border = new Border(this, 0);
            AddChild(border);
            border = new Border(this, 1);
            AddChild(border);
            border = new Border(this, 2);
            AddChild(border);
            border = new Border(this, 3);
            AddChild(border);

            Fan fan = new Fan(this);
            fan.SetXY(70, height - 70);
            AddChild(fan);

            fan = new Fan(this);
            fan.SetXY(width - 70, height - 70);
            fan.rotation = 180;
            AddChild(fan);

            Magnet tmg = new Magnet(this);
            tmg.SetXY(width / 2, height * 0.6f);
            tmg.rotation += 90;
            AddChild(tmg);

            SnapLocation sl = new SnapLocation(this);
            sl.SetXY(200, 200);
            AddChild(sl);

            Goal goal = new Goal(this);
            goal.SetXY(width - 100, height - 200);
            AddChild(goal);
            #endregion

            base.Start();
        }

        public void FaceRight(float a_value, List<int> a_controllerID)
        {
            wheel.rotation += a_value * 4;
        }

        public override void Update(float a_dt)
        {
            if (!m_active)
                return;

            if (m_timeActive > 5)
            {
                //Program program = game as Program;
                //End();
                //program.scorePage.Restart();
            }
        }

        protected override void OnDestroy()
        {
            //if (backgroundMusicChannel != null)
            //    backgroundMusicChannel.Stop();
            base.OnDestroy();
        }

        public override void End()
        {
            base.End();
        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);

        }

    }
}
