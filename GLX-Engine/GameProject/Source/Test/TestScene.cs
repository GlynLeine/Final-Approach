using GLXEngine.Core;
using GLXEngine;
using static GLXEngine.Utils;

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

            Sprite bkgnd = new Sprite("Textures/bicktestbackground.png");
            bkgnd.height = Game.main.width / bkgnd.width * bkgnd.height;
            bkgnd.width = Game.main.width;
            BackgroundTile bt = new BackgroundTile(this, bkgnd);
            bt.x = Game.main.width / 2;
            bt.y = Game.main.height / 2;
            AddChild(bt);

            player = new TestPlayer(this);
            player.SetXY(width / 2, 100);
            player.rotation += 90;
            AddChild(player);

            Sprite wallSprite = new Sprite("Textures/Rectangle.png");
            WallTile wt = new WallTile(this, wallSprite, 350, 64);
            wt.SetXY(width / 2, 200);
            AddChild(wt);

            SnapLocation sl = new SnapLocation(this);
            sl.SetXY(512, 150);
            AddChild(sl);

            Fan fan = new Fan(this);
            fan.SetXY(width - MAX_COL_WIDTH, MAX_COL_WIDTH);
            AddChild(fan);

            Magnet tmg = new Magnet(this);
            tmg.SetXY(width - MAX_COL_WIDTH * 2, MAX_COL_WIDTH);
            AddChild(tmg);

            ConveyerBelt cb = new ConveyerBelt(this, 320);
            cb.right = true;
            cb.SetXY(200, 400);
            AddChild(cb);

            cb = new ConveyerBelt(this, 320);
            cb.right = false;
            cb.SetXY(width - 520, 400);
            AddChild(cb);

            wallSprite = new Sprite("Textures/Rectangle.png");
            wt = new WallTile(this, wallSprite, 600, 64);
            wt.SetXY(width / 2, height - 500);
            wt.continuousRotation = 10;
            AddChild(wt);

            Goal goal = new Goal(this);
            goal.SetXY(width / 2, height);
            AddChild(goal);

            Border border = new Border(this, 0);
            AddChild(border);
            border = new Border(this, 1);
            AddChild(border);
            border = new Border(this, 2);
            AddChild(border);
            border = new Border(this, 3);
            AddChild(border);

            #endregion

            base.Start();
        }

        //public void FaceRight(float a_value, List<int> a_controllerID)
        //{
        //    wheel.rotation += a_value * 4;
        //}

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
            base.OnDestroy();
            Game.main.Restart();
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
