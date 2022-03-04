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
    public sealed class AnnouncementLabel : DXWindow
    {
        #region Properties
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public AnnouncementLabel()
        {
        }
    }

    public sealed class AnnouncementDialog
    {
        public List<string> announcements = new List<string>();

        public DXLabel label;
        public AnnouncementLabel window;

        public DateTime expireTime;
        private int duration = 3;

        public AnnouncementDialog()
        {
            window = new AnnouncementLabel
            {
                Parent = GameScene.Game,
                PassThrough = true,
                Size = new Size(450, 50),
                CanResizeHeight = false,
                CanResizeWidth = false,
                Opacity = 0.5f,
                HasTitle = false,
                
            };
            window.CloseButton.Visible = false;
            window.Location = new Point((GameScene.Game.Size.Width - window.Size.Width) / 2, 0);

            label = new DXLabel
            {
                Parent = window,
                Text = "",
                Size = new Size(10, 10)
            };
            label.Location = new Point((window.Size.Width - label.Size.Width) /2, window.Size.Height + label.Size.Height);
        }

        public void AddNewLabel(string text)
        {
            announcements.Add(text);
            if(announcements.Count > 0)
            {
                window.Visible = true;
            }
        }

        public void Update()
        {
            if(announcements.Count > 0)
            {
                if(label.Location.Y == window.Size.Height + label.Size.Height)
                {
                    label.Text = announcements[0];
                    label.Location = new Point((window.Size.Width - label.Size.Width) / 2, -50);
                    expireTime = CEnvir.Now.AddSeconds(duration);
                }

                if(label.Location.Y != window.Size.Height / 2)
                {
                    if(expireTime > CEnvir.Now)
                    {
                        label.Location = new Point(label.Location.X, label.Location.Y + 1);
                    }
                }
                if (expireTime <= CEnvir.Now)
                {
                    label.Location = new Point(label.Location.X, label.Location.Y + 1);
                }
                if(label.Location.Y >= window.Size.Height + label.Size.Height)
                {
                    announcements.RemoveAt(0);
                    if(announcements.Count <= 0)
                    {
                        window.Visible = false;
                    }
                }
            }
        }

        public void Dispose()
        {
            if(window != null)
            {
                window.Dispose();
                window = null;
            }
            if (label != null)
            {
                label.Dispose();
                label = null;
            }
        }
    }
}
