using GLXEngine.Core;
using GLXEngine;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace GameProject
{
    public class ScorePage : Scene
    {
        class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }

        List<KeyValuePair<string, int>> scoreList = new List<KeyValuePair<string, int>>();

        List<string> nameList;

        List<string> nameOptions = new List<string>();
        int selected = 0;
        string playerName;

        EasyDraw UI;
        bool entry;

        float nextTimeBuffer = 0;
        float nextCooldownTime = 0.15f;
        bool nextCooldown = false;

        float ScoreTimeBuffer = 0;
        float ScoreCooldownTime = 5f;

        public ScorePage() : base()
        {
            nameList = new List<string>(File.ReadAllLines("Misc/names.txt"));

            for (int j = 0; j < 10; j++)
                scoreList.Add(new KeyValuePair<string, int>(nameList[Utils.Random(0, nameList.Count - 1)] + " " + nameList[Utils.Random(0, nameList.Count - 1)], Utils.Random(10, 30)));

            scoreList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            Start();
            entry = false;
        }

        public override void Start()
        {
            base.Start();

            entry = true;


            playerName = "";
            nameOptions.Clear();

            for (int i = 0; i < 6; i++)
            {
                int index = Utils.Random(0, nameList.Count - 1);
                if (!nameOptions.Contains(nameList[index]))
                    nameOptions.Add(nameList[index]);
                else
                    i--;
            }

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

        }

        public void Next(float a_value, List<int> a_controllerID)
        {
            if (!nextCooldown)
            {
                nextCooldown = true;
                nextTimeBuffer = 0;

                selected -= Mathf.Floor(a_value);

                if (a_value > 0)
                {
                    if (selected < 0)
                        selected = 6;
                }
                else if (selected > 6)
                    selected = 0;
            }
        }

        public void Confirm(bool a_pressed, int a_controllerID)
        {
            if (entry && !a_pressed && m_active)
            {
                if (selected >= 6)
                {
                    scoreList.Add(new KeyValuePair<string, int>(playerName.Trim(), (game as Program).score));

                    scoreList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                    while (scoreList.Count > 10)
                        scoreList.Remove(scoreList.Last());

                    ScoreTimeBuffer = 0;

                    entry = false;
                }
                else
                {
                    if (playerName == null)
                        playerName = nameOptions[selected];
                    else
                        playerName += " " + nameOptions[selected];
                }
            }
        }

        public override void Update(float a_dt)
        {
            if (!m_active)
                return;

            if (entry)
            {
                nextTimeBuffer += a_dt;
                if (nextTimeBuffer >= nextCooldownTime)
                {
                    nextTimeBuffer = nextCooldownTime;
                    nextCooldown = false;
                }
            }
            else
            {
                ScoreTimeBuffer += a_dt;
                if (ScoreTimeBuffer >= ScoreCooldownTime)
                {
                    ScoreTimeBuffer = ScoreCooldownTime;
                    Program program = game as Program;
                    if (program.deathScreen.backgroundMusicChannel != null)
                        program.deathScreen.backgroundMusicChannel.Stop();
                    End();
                    program.startScreen.Restart();
                }
            }
        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);

            if (entry)
            {
                float lineHeight = UI.TextHeight(" ") * 1.5f;

                UI.Fill(0, 255, 255);
                UI.Text(playerName, game.width / 2, lineHeight);

                int i = 0;
                foreach (string nameOption in nameOptions)
                {
                    if (i == selected)
                        UI.Fill(255);
                    else
                        UI.Fill(50);
                    UI.Text(nameOption, game.width / 2, (i + 3) * lineHeight);
                    i++;
                }

                if (selected == 6)
                    UI.Fill(0, 255, 0);
                else
                    UI.Fill(0, 50, 0);
                UI.Text("Confirm", game.width / 2, 9 * lineHeight);
            }
            else
            {
                int i = 0;
                foreach (KeyValuePair<string, int> score in scoreList)
                {
                    UI.Fill(255);
                    UI.Text(score.Key + ": " + score.Value, game.width / 2, (i + 1) * UI.TextHeight(" ") * 1.5f);
                    i++;
                }
            }
        }

    }
}
