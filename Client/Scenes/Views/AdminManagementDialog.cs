using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public enum ServerRates
    {
        Experience = 0,
        Drop = 1,
        Gold = 2,
        Skill = 3,
        Companion = 4
    }
    public sealed class AdminManagementDialog : DXWindow
    {
        #region Properties
        private DXTabControl TabControl;
        private DXTab AdminTab, GameTab, PlayerTab, EventTab;

        public Dictionary<int, DXTextBox> GameRatesMap = new Dictionary<int, DXTextBox>();

        public override WindowType Type => WindowType.CharacterBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        #endregion

        public AdminManagementDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(266, 371));


            TabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };
            AdminTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Admin" } },
            };
            GameTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Game" } },
            };
            //GameTab.BeforeChildrenDraw += GameTab_BeforeChildrenDraw;
            PlayerTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Player" } },
            };
            EventTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Event" } },
            };

            for (int i = 0; i < 5; i++)
            {
                DXLabel label = new DXLabel
                {
                    Parent = GameTab,
                    Text = ((ServerRates)i).ToString() + " rate: "
                };
                label.Location = new Point(20, 30 + (10 + label.Size.Height) * i);
                GameRatesMap[i] = new DXTextBox
                {
                    Parent = GameTab,
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Location = new Point(140, label.Location.Y),
                    Size = new Size(100, 18)
                };
            }

            DXButton filterButton = new DXButton
            {
                Parent = GameTab,
                Label = { Text = "Apply", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight)
            };
            filterButton.Location = new Point(100, GameTab.Size.Height - 50);
            filterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.ReceiveChat("Applied Server settings", MessageType.System);
                CEnvir.Enqueue(new C.ServerRates { ExperienceRate = int.Parse(GameRatesMap[0].TextBox.Text),
                    DropRate = int.Parse(GameRatesMap[1].TextBox.Text),
                    GoldRate = int.Parse(GameRatesMap[2].TextBox.Text),
                    SkillRate = int.Parse(GameRatesMap[3].TextBox.Text),
                    CompanionRate = int.Parse(GameRatesMap[4].TextBox.Text),
                });
                
            };
        }
    }
}
