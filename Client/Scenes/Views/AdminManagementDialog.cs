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

        public Dictionary<int, DXNumberTextBox> GameRatesMap = new Dictionary<int, DXNumberTextBox>();
        public DXTextBox MonsterNameBox;
        public DXNumberTextBox MonsterNumberBox;
        public DXCheckBox MonsterIsPetBox;
        public DXNumberTextBox SelfLevelNumberBox;
        public DXNumberTextBox SelfGoldNumberBox;
        public DXTextBox ItemNameBox;
        public DXNumberTextBox ItemAmountNumberBox;

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

            PrepareAdminTab();
            PrepareGameTab();
            PreparePlayerTab();
            PrepareEventTab();
        }

        void PrepareAdminTab()
        {
            AdminTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Admin" } },
            };

            DXLabel label = new DXLabel
            {
                Parent = AdminTab,
                Text = "Monster"
            };
            label.Location = new Point(10, 10 + (10 + label.Size.Height) * 0);

            MonsterNameBox = new DXTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(60, label.Location.Y),
                Size = new Size(100, 18),

            };

            DXCheckBox MonsterIsPetBox = new DXCheckBox
            {
                Parent = AdminTab,
                Label = { Text = "Is Pet" },
                Checked = false,
            };
            MonsterIsPetBox.Location = new Point(MonsterNameBox.Location.X + MonsterNameBox.Size.Width + 10, MonsterNameBox.Location.Y);

            label = new DXLabel
            {
                Parent = AdminTab,
                Text = "Amount"
            };
            label.Location = new Point(10, 10 + (10 + label.Size.Height) * 1);

            MonsterNumberBox = new DXNumberTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(60, label.Location.Y),
                Size = new Size(50, 18),
                MaxValue = 5000,
                MinValue = 1
            };

            MonsterNumberBox.Value = 1;

            DXButton spawnMonsterButton = new DXButton
            {
                Parent = AdminTab,
                Label = { Text = "Spawn", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight)
            };
            spawnMonsterButton.Location = new Point(MonsterNumberBox.Location.X + MonsterNumberBox.Size.Width + 30, MonsterNumberBox.Location.Y);
            spawnMonsterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.ReceiveChat("Spawning Monster/s", MessageType.System);
                CEnvir.Enqueue(new C.MonsterSpawn
                {
                    Name = MonsterNameBox.TextBox.Text,
                    Amount = (int)MonsterNumberBox.Value,
                    IsPet = MonsterIsPetBox.Checked
                });

            };

            DXButton levelButton = new DXButton
            {
                Parent = AdminTab,
                Label = { Text = "Level", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight)
            };
            levelButton.Location = new Point(10, 10 + (10 + label.Size.Height) * 3);
            levelButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.LevelUp
                {
                    Name = "",
                    Level = (int)SelfLevelNumberBox.Value,
                    Self = true
                });

            };

            SelfLevelNumberBox = new DXNumberTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(70, levelButton.Location.Y),
                Size = new Size(50, 18),
                MaxValue = 100000000,
                MinValue = 1
            };
            SelfLevelNumberBox.Value = 1;

            DXButton goldButton = new DXButton
            {
                Parent = AdminTab,
                Label = { Text = "Gold", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight)
            };
            goldButton.Location = new Point(10, 10 + (10 + label.Size.Height) * 4);
            goldButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GiveGold
                {
                    Name = "",
                    Gold = (int)SelfGoldNumberBox.Value,
                    Self = true
                });

            };

            SelfGoldNumberBox = new DXNumberTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(70, goldButton.Location.Y),
                Size = new Size(50, 18),
                MaxValue = 100000000,
                MinValue = -100000000
            };
            SelfGoldNumberBox.Value = 1;

            DXButton itemButton = new DXButton
            {
                Parent = AdminTab,
                Label = { Text = "Item", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight)
            };
            itemButton.Location = new Point(10, 10 + (10 + label.Size.Height) * 5);
            itemButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.MakeItem
                {
                    Name = ItemNameBox.TextBox.Text,
                    Amount = (int)ItemAmountNumberBox.Value
                });

            };

            ItemNameBox = new DXTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(70, itemButton.Location.Y),
                Size = new Size(100, 18),

            };

            ItemAmountNumberBox = new DXNumberTextBox
            {
                Parent = AdminTab,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point(180, itemButton.Location.Y),
                Size = new Size(50, 18),
                MaxValue = 100000000,
                MinValue = 1
            };
            ItemAmountNumberBox.Value = 1;
        }

        void PrepareGameTab()
        {
            GameTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Game" } },
            };

            //GameTab.BeforeChildrenDraw += GameTab_BeforeChildrenDraw;

            for (int i = 0; i < 5; i++)
            {
                DXLabel label = new DXLabel
                {
                    Parent = GameTab,
                    Text = ((ServerRates)i).ToString() + " rate: "
                };
                label.Location = new Point(20, 30 + (10 + label.Size.Height) * i);
                GameRatesMap[i] = new DXNumberTextBox
                {
                    Parent = GameTab,
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Location = new Point(140, label.Location.Y),
                    Size = new Size(100, 18),
                    MaxValue = int.MaxValue,
                    MinValue = 0

                };
                GameRatesMap[i].Value = 0;
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
                CEnvir.Enqueue(new C.ServerRates
                {
                    ExperienceRate = (int)GameRatesMap[0].Value,
                    DropRate = (int)GameRatesMap[1].Value,
                    GoldRate = (int)GameRatesMap[2].Value,
                    SkillRate = (int)GameRatesMap[3].Value,
                    CompanionRate = (int)GameRatesMap[4].Value,
                });

            };
        }



        void PreparePlayerTab()
        {
            PlayerTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Player" } },
            };
        }

        void PrepareEventTab()
        {
            EventTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Event" } },
            };
        }
    }
}
