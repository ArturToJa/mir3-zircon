using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Linq;
using Library.SystemModels;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public class ReviveDialog : DXWindow
    {
        #region Properties

        public override WindowType Type => WindowType.StorageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        #endregion

        public ReviveDialog()
        {
            HasTitle = false;
            Movable = false;
            CloseButton.Visible = false;
            SetClientSize(new Size(150, 60));

            DXButton respawnTownButton = new DXButton
            {
                Parent = this,
                Label = { Text = "Respawn in Town", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(100, SmallButtonHeight)
            };
            respawnTownButton.Location = new Point(ClientArea.X + (ClientArea.Width - respawnTownButton.Size.Width) / 2, 20);
            respawnTownButton.MouseClick += (o, e) =>
            {
                GameScene.Game.ReviveBox.Visible = false;
                CEnvir.Enqueue(new C.TownRevive());
            };

            DXButton respawnHereButton = new DXButton
            {
                Parent = this,
                Label = { Text = "Respawn Here - 50 levels", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(150, SmallButtonHeight)
            };
            respawnHereButton.Location = new Point(ClientArea.X + (ClientArea.Width - respawnHereButton.Size.Width) / 2, 50);
            respawnHereButton.MouseClick += (o, e) =>
            {
                GameScene.Game.ReviveBox.Visible = false;
                CEnvir.Enqueue(new C.InPlaceRevive());
            };
        }
    }
}
