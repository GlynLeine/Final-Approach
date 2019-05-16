using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace GameProject
{
    public class DeathScreen : Scene
    {

        Sprite tumbStoneSprite;

        Sound backgroundMusic;
        public SoundChannel backgroundMusicChannel;

        public DeathScreen() : base()
        {

        }

        public override void Start()
        {
            base.Start();

            tumbStoneSprite = new Sprite("Textures/tumbStone.png");
            tumbStoneSprite.SetOrigin(tumbStoneSprite.width / 2, tumbStoneSprite.height / 2);
            tumbStoneSprite.x = game.width / 2;
            tumbStoneSprite.y = game.height / 2;
            AddChild(tumbStoneSprite);

            backgroundMusic = new Sound("Audio/death-menusong.wav", true);
            backgroundMusicChannel = backgroundMusic.Play();
        }

        public override void Update(float a_dt)
        {
            if (!m_active)
                return;

            if (m_timeActive > 5)
            {
                Program program = game as Program;
                End();
                program.scorePage.Restart();
            }
        }

        protected override void OnDestroy()
        {
            if (backgroundMusicChannel != null)
                backgroundMusicChannel.Stop();
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
