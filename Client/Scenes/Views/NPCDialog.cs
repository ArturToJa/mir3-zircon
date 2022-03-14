
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using S = Library.Network.ServerPackets;
using C = Library.Network.ClientPackets;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class NPCDialog : DXWindow
    {
        #region Properties
        public static  Regex R = new Regex(@"\[(?<Text>.*?):(?<ID>.+?)\]", RegexOptions.Compiled);

        public NPCPage Page;
        public DXLabel PageText;
        
        public List<DXLabel> Buttons = new List<DXLabel>();
        public bool Opened;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);


            if (PageText == null || IsResizing) return;

            PageText.Location = new Point(ClientArea.X + 10, ClientArea.Y + 10);
            PageText.Size = new Size(ClientArea.Width - 20, ClientArea.Height - 20);

            ProcessText();
        }

        public override void OnIsResizingChanged(bool oValue, bool nValue)
        {
            PageText.Location = new Point(ClientArea.X + 10, ClientArea.Y + 10);
            PageText.Size = new Size(ClientArea.Width - 20, ClientArea.Height - 20);

            ProcessText();


            base.OnIsResizingChanged(oValue, nValue);
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.NPCGoodsBox != null && !IsVisible)
                GameScene.Game.NPCGoodsBox.Visible = false;

            if (GameScene.Game.NPCSellBox != null && !IsVisible)
                GameScene.Game.NPCSellBox.Visible = false;

            if (GameScene.Game.NPCUpgradeBox != null && !IsVisible)
                GameScene.Game.NPCUpgradeBox.Visible = false;

            if (GameScene.Game.NPCQuestBox != null && !IsVisible)
                GameScene.Game.NPCQuestBox.Visible = false;

            if (GameScene.Game.NPCAdoptCompanionBox != null && !IsVisible)
                GameScene.Game.NPCAdoptCompanionBox.Visible = false;

            if (GameScene.Game.NPCCompanionStorageBox != null && !IsVisible)
                GameScene.Game.NPCCompanionStorageBox.Visible = false;

            if (GameScene.Game.NPCWeddingRingBox != null && !IsVisible)
                GameScene.Game.NPCWeddingRingBox.Visible = false;

            if (GameScene.Game.NPCItemFragmentBox != null && !IsVisible)
                GameScene.Game.NPCItemFragmentBox.Visible = false;

            if (GameScene.Game.NPCWeaponCraftBox != null && !IsVisible)
                GameScene.Game.NPCWeaponCraftBox.Visible = false;

            if (GameScene.Game.NPCUpgradeGemBox != null && !IsVisible)
                GameScene.Game.NPCUpgradeGemBox.Visible = false;

            if (GameScene.Game.NPCLevelUpBox != null && !IsVisible)
                GameScene.Game.NPCLevelUpBox.Visible = false;

            if (Opened) 
            {
                GameScene.Game.NPCID = 0;
                Opened = false;
                CEnvir.Enqueue(new C.NPCClose());
            }


            if (IsVisible)
            {
                if (GameScene.Game.CharacterBox.Location.X < Size.Width)
                    GameScene.Game.CharacterBox.Location = new Point(Size.Width, 0);

                GameScene.Game.StorageBox.Location = new Point(GameScene.Game.Size.Width - GameScene.Game.StorageBox.Size.Width, GameScene.Game.InventoryBox.Size.Height);
            }
            else if (GameScene.Game.CharacterBox.Location.X == Size.Width)
            {
                GameScene.Game.CharacterBox.ApplySettings();
                GameScene.Game.StorageBox.ApplySettings();//.Location = new Point(GameScene.Game.Size.Width - GameScene.Game.StorageBox.Size.Width - GameScene.Game.InventoryBox.Size.Width, 0);
            }
        }
        
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCDialog()
        {
            HasTitle = false;
            TitleLabel.Text = string.Empty;
            HasFooter = false;
            Movable = false;
            SetClientSize(new Size(491, 220));

            PageText = new DXLabel
            {
                AutoSize = false,
                Outline = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                Parent = this,
                Location = new Point(ClientArea.X + 10, ClientArea.Y + 10),
                Size = new Size(ClientArea.Width - 20, ClientArea.Height - 20),
            };
        }

        #region Methods
        public void Response(S.NPCResponse info)
        {
            GameScene.Game.NPCID = info.ObjectID;
            GameScene.Game.NPCBox.Visible = true;

            Page = info.Page;
            //  RawPageText = info.Page.Say.Replace("\n", "");
            PageText.Text = R.Replace(Page.Say, @"${Text}");

            int height = DXLabel.GetHeight(PageText, ClientArea.Width).Height;
            if (height > ClientArea.Height)
                SetClientSize(new Size(ClientArea.Width, height));


            ProcessText();

            Opened = true;

            GameScene.Game.NPCGoodsBox.Visible = false;
            GameScene.Game.NPCSellBox.Visible = false;
            GameScene.Game.NPCUpgradeBox.Visible = false;
            GameScene.Game.NPCAdoptCompanionBox.Visible = false;
            GameScene.Game.NPCCompanionStorageBox.Visible = false;
            GameScene.Game.NPCWeddingRingBox.Visible = false;
            GameScene.Game.NPCItemFragmentBox.Visible = false;
            GameScene.Game.NPCLevelUpBox.Visible = false;
            GameScene.Game.NPCWeaponCraftBox.Visible = false;
            GameScene.Game.NPCUpgradeGemBox.Visible = false;

            switch (info.Page.DialogType)
            {
                case NPCDialogType.None:
                    break;
                case NPCDialogType.BuySell:
                    GameScene.Game.NPCGoodsBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCGoodsBox.Visible = Page.Goods.Count > 0;
                    GameScene.Game.NPCGoodsBox.NewGoods(Page.Goods);
                    GameScene.Game.NPCSellBox.Visible = Page.Types.Count > 0;
                    GameScene.Game.NPCSellBox.Location = GameScene.Game.NPCGoodsBox.Visible ? new Point(Size.Width - GameScene.Game.NPCSellBox.Size.Width, Size.Height) : new Point(0, Size.Height);
                    break;

                case NPCDialogType.Refine:
                    GameScene.Game.NPCUpgradeBox.Visible = true;
                    GameScene.Game.NPCUpgradeBox.Location = new Point(Size.Width - GameScene.Game.NPCUpgradeBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.CompanionManage:
                    GameScene.Game.NPCCompanionStorageBox.Visible = true;
                    GameScene.Game.NPCCompanionStorageBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCAdoptCompanionBox.Visible = true;
                    GameScene.Game.NPCAdoptCompanionBox.Location = new Point(Size.Width - GameScene.Game.NPCAdoptCompanionBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.WeddingRing:
                    GameScene.Game.NPCWeddingRingBox.Visible = true;
                    GameScene.Game.NPCWeddingRingBox.Location = new Point(Size.Width - GameScene.Game.NPCWeddingRingBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.ItemFragment:
                    GameScene.Game.NPCItemFragmentBox.Visible = true;
                    GameScene.Game.NPCItemFragmentBox.Location = new Point(Size.Width - GameScene.Game.NPCItemFragmentBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.WeaponCraft:
                    GameScene.Game.NPCWeaponCraftBox.Visible = true;
                    GameScene.Game.NPCWeaponCraftBox.Location = new Point(Size.Width - GameScene.Game.NPCWeaponCraftBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.UpgradeGem:
                    GameScene.Game.NPCUpgradeGemBox.Visible = true;
                    GameScene.Game.NPCUpgradeGemBox.Location = new Point(Size.Width - GameScene.Game.NPCUpgradeGemBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.LevelUpScroll:
                    GameScene.Game.NPCLevelUpBox.Visible = true;
                    GameScene.Game.NPCLevelUpBox.Location = new Point(Size.Width - GameScene.Game.NPCLevelUpBox.Size.Width, Size.Height);
                    break;
            }
        }
        
        private void ProcessText()
        {
            foreach (DXLabel label in Buttons)
                label.Dispose();

            Buttons.Clear();
            //string rawText = RawPageText.Replace("\n", "");

            MatchCollection matches = R.Matches(Page.Say);
            List<CharacterRange> ranges = new List<CharacterRange>();

            int offset = 1;
            foreach (Match match in matches)
            {
                ranges.Add(new CharacterRange(match.Groups["Text"].Index - offset, match.Groups["Text"].Length));
                offset += 3 + match.Groups["ID"].Length;
            }

            for (int i = 0; i < ranges.Count; i++)
            {
                List<ButtonInfo> buttons = GetWordRegionsNew(DXManager.Graphics, PageText.Text, PageText.Font, PageText.DrawFormat, PageText.Size.Width, ranges[i].First, ranges[i].Length);

                List<DXLabel> labels = new List<DXLabel>();

                foreach (ButtonInfo info in buttons)
                {
                    labels.Add(new DXLabel
                    {
                        AutoSize = false,
                        Parent = PageText,
                        ForeColour = Color.Yellow,
                        Location = info.Region.Location,
                        DrawFormat = PageText.DrawFormat,
                        Text = PageText.Text.Substring(info.Index, info.Length),
                        Font = new Font(PageText.Font.FontFamily, PageText.Font.Size, FontStyle.Underline),
                        Size = info.Region.Size,
                        Outline = false,
                        Sound = SoundIndex.ButtonC,
                    });
                }

                int index = i;
                DateTime NextButtonTime = DateTime.MinValue;
                foreach (DXLabel label in labels)
                {
                    label.MouseEnter += (o, e) =>
                    {
                        if (GameScene.Game.Observer) return;
                        foreach (DXLabel l in labels)
                            l.ForeColour = Color.Red;
                    };

                    label.MouseLeave += (o, e) =>
                    {
                        if (GameScene.Game.Observer) return;
                        foreach (DXLabel l in labels)
                            l.ForeColour = Color.Yellow;
                    };
                    label.MouseClick += (o, e) =>
                    {
                        if (GameScene.Game.Observer) return;

                        if (matches[index].Groups["ID"].Value == "0")
                        {
                            Visible = false;
                            return;
                        }

                        if (CEnvir.Now < NextButtonTime) return;

                        NextButtonTime = CEnvir.Now.AddSeconds(1);

                        CEnvir.Enqueue(new C.NPCButton { ButtonID = int.Parse(matches[index].Groups["ID"].Value) });
                    };

                    Buttons.Add(label);
                }
            }

        }

        public static List<ButtonInfo> GetWordRegionsNew(Graphics graphics, string text, Font font, TextFormatFlags flags, int width, int index, int length)
        {

            List<ButtonInfo> regions = new List<ButtonInfo>();

            Size tSize = TextRenderer.MeasureText(graphics, "A", font, new Size(width, 2000), flags);
            int h = tSize.Height;
            int leading = tSize.Width - (TextRenderer.MeasureText(graphics, "AA", font, new Size(width, 2000), flags).Width - tSize.Width);

            int lineStart = 0;
            int lastHeight = h;

            //IfWord Wrap ?
            //{
            Regex regex = new Regex(@"(?<Words>\S+)", RegexOptions.Compiled);

            MatchCollection matches = regex.Matches(text);

            List<CharacterRange> ranges = new List<CharacterRange>();

            foreach (Match match in matches)
                ranges.Add(new CharacterRange(match.Index, match.Length));


            ButtonInfo currentInfo = null;



            //If Word Wrap enabled.
            foreach (CharacterRange range in ranges)
            {
                int height = TextRenderer.MeasureText(graphics, text.Substring(0, range.First + range.Length), font, new Size(width, 9999), flags).Height;

                if (range.First >= index + length) break;

                if (height > lastHeight)
                {
                    lineStart = range.First; // New Line was formed record from start.
                    lastHeight = height;

                    //This Word is on a new line and therefore must start at 0.
                    //We do NOT know its length on this new line but since its on a new line it will be easy to measure.

                    if (range.First >= index)
                    {
                        //We need to capture this word
                        //It needs to be a new Rectangle.
                        Rectangle region = new Rectangle
                        {
                            X = 0,
                            Y = height - h,
                            Width = TextRenderer.MeasureText(graphics, text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
                            Height = h,
                        };
                        currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                        regions.Add(currentInfo);
                    }

                }
                else
                {
                    //it is on the same Line IT Must be able to contain ALL of the letters. (Word Wrap)
                    //just need to know the length of the word and the Length of the start of the line to the start of the word

                    if (range.First >= index)
                    {
                        if (currentInfo == null)
                        {
                            Rectangle region = new Rectangle
                            {
                                X = TextRenderer.MeasureText(graphics, text.Substring(lineStart, range.First - lineStart), font, new Size(width, 9999), flags).Width,
                                Y = height - h,
                                Width = TextRenderer.MeasureText(graphics, text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
                                Height = h,
                            };

                            if (region.X > 0)
                                region.X -= leading;
                            currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                            regions.Add(currentInfo);
                        }
                        else
                        {
                            //Measure Current.Index to range.First + Length
                            currentInfo.Length = (range.First + range.Length) - currentInfo.Index;
                            currentInfo.Region.Width = TextRenderer.MeasureText(graphics, text.Substring(currentInfo.Index, currentInfo.Length), font, new Size(width, 9999), flags).Width;
                        }
                        //We need to capture this word.
                        //ADD to any previous rects otherwise create new ?
                    }
                }
            }
            //}

            return regions;
            /*
            for (int i = 0; i < text.Length; i++)
            {
                Size size = TextRenderer.MeasureText(graphics, text.Substring(lineStart, i  - lineStart + 1), font, new Size(width, 9999), flags); // +1 Because its pointless measuring a 0 length string.
                int height = TextRenderer.MeasureText(graphics, text.Substring(0, i + 1), font, new Size(width, 9999), flags).Height;

                if (i == text.Length - 1 || i == index + length)
                {
                    current.Width = lastSize.Width - current.X;
                    regions.Add(new ButtonInfo { Region = current, Text = text.Substring(textStart, i - textStart).Replace("\r", "") });
                    break;
                }

                if (height > lastHeight)
                {
                    x = 0;
                    y += lastSize.Height;
                    
                    lineStart = i;
                    size = TextRenderer.MeasureText(graphics, text.Substring(lineStart, i - lineStart + 1), font, new Size(width, 9999), flags);
                    if (size.Height > h)
                        size = new Size(size.Width, h);

                    if (i > index)
                    {
                        current.Width = lastSize.Width - current.X;
                        regions.Add(new ButtonInfo { Region =  current, Text = text.Substring(textStart, i - textStart).Replace("\r", "") });

                        current.X = x;
                        current.Y = y;
                        current.Height = h;
                        textStart = i;
                    }
                }
                if (i == index)
                {
                    current.X = x;
                    current.Y = y;
                    current.Height = h;
                    textStart = i;
                }

                x += size.Width;

                lastSize = size;
                lastHeight = height;
            }




            return regions;*/
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Page = null;

                if (PageText != null)
                {
                    if (!PageText.IsDisposed)
                        PageText.Dispose();

                    PageText = null;
                }

                if (Buttons != null)
                {
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        if (Buttons[i] != null)
                        {
                            if (!Buttons[i].IsDisposed)
                                Buttons[i].Dispose();

                            Buttons[i] = null;
                        }
                    }

                    Buttons.Clear();
                    Buttons = null;
                }

                Opened = false;
            }

        }

        #endregion

        public class ButtonInfo
        {
            public Rectangle Region;
            public int Index;
            public int Length;
        }
    }

    public sealed class NPCGoodsDialog : DXWindow
    {
        #region Properties

        #region SelectedCell

        public NPCGoodsCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                NPCGoodsCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private NPCGoodsCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(NPCGoodsCell oValue, NPCGoodsCell nValue)
        {
            if (oValue != null) oValue.Selected = false;
            if (nValue != null) nValue.Selected = true;

            BuyButton.Enabled = nValue != null;

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        private DXVScrollBar ScrollBar;
        public DXCheckBox GuildCheckBox;

        public List<NPCGoodsCell> Cells = new List<NPCGoodsCell>();
        private DXButton BuyButton;
        public DXControl ClientPanel;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            ClientPanel.Size = ClientArea.Size;
            ClientPanel.Location = ClientArea.Location;

        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCGoodsDialog()
        {
            TitleLabel.Text = "Goods";

            HasFooter = true;
            Movable = false;


            SetClientSize(new Size(227, 7*43 + 1));

            ClientPanel = new DXControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 1),
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width - 2, ClientArea.Y + 1);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;

            BuyButton = new DXButton
            {
                Location = new Point(40, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Buy" },
                Enabled = false,
            };
            BuyButton.MouseClick += (o, e) => Buy();

            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Use Guild Funds:" },
                Enabled = false,
            };
            GuildCheckBox.Location = new Point( 200, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) /2);

        }

        #region Methods

        public void NewGoods(IList<NPCGood> goods)
        {
            foreach (NPCGoodsCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (NPCGood good in goods)
            {
                NPCGoodsCell cell;
                Cells.Add(cell = new NPCGoodsCell
                {
                    Parent = ClientPanel,
                    Good = good
                });
                cell.MouseClick += (o, e) => SelectedCell = cell;
                cell.MouseWheel += ScrollBar.DoMouseWheel;
                cell.MouseDoubleClick += (o, e) => Buy();
            }


            ScrollBar.MaxValue = goods.Count*43 - 2;
            SetClientSize(new Size(ClientArea.Width, Math.Min(ScrollBar.MaxValue, 7*43 - 3) + 1));
            ScrollBar.VisibleSize = ClientArea.Height;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientArea.Height - 2);

            BuyButton.Location = new Point(30, Size.Height - 43);
            GuildCheckBox.Location = new Point(120, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) / 2);
            ScrollBar.Value = 0;
            UpdateLocations();
        }
        private void UpdateLocations()
        {
            int y = -ScrollBar.Value + 1;

            foreach (NPCGoodsCell cell in Cells)
            {
                cell.Location = new Point(1, y);

                y += cell.Size.Height + 3;
            }
        }

        public void Buy()
        {
            if (GameScene.Game.Observer) return;

            if (SelectedCell == null) return;

            long gold = MapObject.User.Gold;

            if (GuildCheckBox.Checked && GameScene.Game.GuildBox.GuildInfo != null)
                gold = GameScene.Game.GuildBox.GuildInfo.GuildFunds;


            if (SelectedCell.Good.Item.StackSize > 1)
            {
                long maxCount = SelectedCell.Good.Item.StackSize;

                maxCount = Math.Min(maxCount, gold / SelectedCell.Good.Cost);



                if (SelectedCell.Good.Item.Weight > 0)
                {
                    switch (SelectedCell.Good.Item.ItemType)
                    {
                        case ItemType.Amulet:
                        case ItemType.Poison:
                            if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                            {
                                GameScene.Game.ReceiveChat($"You do not have enough weight to buy any '{SelectedCell.Good.Item.ItemName}'.", MessageType.System);
                                return;
                            }
                            break;
                        default:
                            maxCount = Math.Min(maxCount, (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight) / SelectedCell.Good.Item.Weight);
                            break;
                    }
                }

                if (maxCount < 0)
                {
                    GameScene.Game.ReceiveChat($"You do not have enough weight to buy any '{SelectedCell.Good.Item.ItemName}'.", MessageType.System);
                    return;
                }

                ClientUserItem item = new ClientUserItem(SelectedCell.Good.Item, (int) Math.Min(int.MaxValue, maxCount));

                DXItemAmountWindow window = new DXItemAmountWindow("Buy Item", item);
                window.ConfirmButton.MouseClick += (o, e) =>
                {
                    CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = window.Amount, GuildFunds = GuildCheckBox.Checked });
                    GuildCheckBox.Checked = false;
                };
            }
            else
            {
                if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                {
                    GameScene.Game.ReceiveChat($"You do not have enough weight to buy a '{SelectedCell.Good.Item.ItemName}'.", MessageType.System);
                    return;
                }

                if (SelectedCell.Good.Cost > gold)
                {
                    GameScene.Game.ReceiveChat($"You do not have enough gold to buy a '{SelectedCell.Good.Item.ItemName}'.", MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = 1, GuildFunds = GuildCheckBox.Checked });
                GuildCheckBox.Checked = false;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedCell = null;
                SelectedCellChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }
                    }

                    Cells.Clear();
                    Cells = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCGoodsCell : DXControl
    {
        #region Properties

        #region Good

        public NPCGood Good
        {
            get => _Good;
            set
            {
                if (_Good == value) return;

                NPCGood oldValue = _Good;
                _Good = value;

                OnGoodChanged(oldValue, value);
            }
        }
        private NPCGood _Good;
        public event EventHandler<EventArgs> GoodChanged;
        public void OnGoodChanged(NPCGood oValue, NPCGood nValue)
        {
            ItemCell.Item = new ClientUserItem(Good.Item, 1) { Flags = UserItemFlags.Locked  };
            
            switch (Good.Item.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Book:
                    ItemCell.Item.Flags |= UserItemFlags.NonRefinable;
                    break;
            }
            ItemNameLabel.Text = Good.Item.ItemName;

            CostLabel.Text = Good.Cost.ToString("##,##0");
            CostLabel.Location = new Point(GoldIcon.Location.X - CostLabel.Size.Width, GoldIcon.Location.Y + GoldIcon.Size.Height - CostLabel.Size.Height);

            CostLabel.ForeColour = Good.Cost > MapObject.User.Gold ? Color.Red : Color.Yellow;

            switch (Good.Item.ItemType)
            {
                case ItemType.Nothing:
                    RequirementLabel.Text = string.Empty;
                    break;
                case ItemType.Meat:
                    RequirementLabel.Text = $"Quality: {Good.Item.SetValue / 1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Ore:
                    RequirementLabel.Text = $"Purity: {Good.Item.SetValue / 1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:

                    if (GameScene.Game.CanUseItem(ItemCell.Item))
                    {
                        RequirementLabel.Text = "Can use Item";
                        RequirementLabel.ForeColour = Color.Aquamarine;
                    }
                    else
                    {
                        RequirementLabel.Text = "Cannot use Item";
                        RequirementLabel.ForeColour = Color.Red;
                    }
                    break;
            }


            GoodChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;

        public DXImageControl GoldIcon;
        public DXLabel ItemNameLabel, RequirementLabel, CostLabel;

        #endregion

        public NPCGoodsCell()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);
            //  Border = true;
            //   ForeColour = Color.White;
            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(219, 40);

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight)/2, (Size.Height - DXItemCell.CellHeight)/2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };
            ItemNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X*2 + ItemCell.Size.Width, ItemCell.Location.Y),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            RequirementLabel = new DXLabel
            {
                Parent = this,
                Text = "Requirement",
                IsControl = false,
            };
            RequirementLabel.Location = new Point(ItemCell.Location.X*2 + ItemCell.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RequirementLabel.Size.Height);


            GoldIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.Inventory,
                Index = 121,
                Parent = this,
                IsControl = false,
            };
            GoldIcon.Location = new Point(Size.Width - GoldIcon.Size.Width - ItemCell.Location.X - 10, Size.Height - GoldIcon.Size.Height - ItemCell.Location.X);

            CostLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods

        public void UpdateColours()
        {

            CostLabel.ForeColour = Good.Cost > MapObject.User.Gold ? Color.Red : Color.Yellow;

            switch (Good.Item.ItemType)
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:
                    RequirementLabel.ForeColour = GameScene.Game.CanUseItem(ItemCell.Item) ? Color.Aquamarine : Color.Red;
                    break;
            }




        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = ItemCell.Item;
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Good = null;
                GoodChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (GoldIcon != null)
                {
                    if (!GoldIcon.IsDisposed)
                        GoldIcon.Dispose();

                    GoldIcon = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (RequirementLabel != null)
                {
                    if (!RequirementLabel.IsDisposed)
                        RequirementLabel.Dispose();

                    RequirementLabel = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCSellDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton SellButton;
        public DXLabel GoldLabel;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCSellDialog()
        {
            TitleLabel.Text = "Sell Items";

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 7),
                Parent = this,
                GridType = GridType.Sell,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 50));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.LinkChanged += Cell_LinkChanged;
            }


            GoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "Sale Total",
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "Select All" },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            SellButton = new DXButton
            {
                Label = { Text = "Sell" },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            SellButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCSell { Links = links });
            };
        }

        #region Methods
        private void Cell_LinkChanged(object sender, EventArgs e)
        {
            long sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                count++;
                sum += cell.Link.Item.Price(cell.LinkedCount);
            }


            GoldLabel.Text = sum.ToString("#,##0");

            SellButton.Enabled = count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (SellButton != null)
                {
                    if (!SellButton.IsDisposed)
                        SellButton.Dispose();

                    SellButton = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCUpgradeItemDialog : DXWindow
    {
        #region Properties

        public DXItemGrid ItemToUpgradeGrid, SacrificeItemGrid, SpecialGrid;
        public DXButton SubmitButton;

        public DXLabel RequiredItem;
        public DXLabel RequiredSpecial;
        public DXLabel GoldCost;

        private int sacrificeItems = 0;
        private bool specialItems = false;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                ItemToUpgradeGrid.ClearLinks();
                SacrificeItemGrid.ClearLinks();
                SpecialGrid.ClearLinks();
                sacrificeItems = 0;
                specialItems = false;
            }
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCUpgradeItemDialog()
        {
            TitleLabel.Text = "Refine";

            SetClientSize(new Size(300, 130));
            DXLabel label = new DXLabel
            {
                Text = "Item to upgrade",
                Location = ClientArea.Location,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            ItemToUpgradeGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.EquipmentUpgradeTarget,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            RequiredItem = new DXLabel
            {
                Text = "Items to sacrifice",
                Location = new Point(label.Location.X, ItemToUpgradeGrid.Location.Y + ItemToUpgradeGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            SacrificeItemGrid = new DXItemGrid
            {
                GridSize = new Size(3, 1),
                Parent = this,
                GridType = GridType.EquipmentUpgradeItems,
                Linked = true,
                Location = new Point(RequiredItem.Location.X + 5, RequiredItem.Location.Y + RequiredItem.Size.Height + 5)
            };

            RequiredSpecial = new DXLabel
            {
                Text = "Special",
                Location = new Point(SacrificeItemGrid.Location.X + SacrificeItemGrid.Size.Width + DXItemCell.CellWidth - 7, RequiredItem.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            SpecialGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.EquipmentUpgradeSpecial,
                Linked = true,
                Location = new Point(RequiredSpecial.Location.X + 5, RequiredSpecial.Location.Y + RequiredSpecial.Size.Height + 5)
            };
            foreach (DXItemCell cell in ItemToUpgradeGrid.Grid)
            {
                cell.LinkChanged += ItemToUpgradeCell_LinkChanged;
            }
            foreach (DXItemCell cell in SacrificeItemGrid.Grid)
            {
                cell.LinkChanged += SacrificeItemCell_LinkChanged;
            }
            foreach (DXItemCell cell in SpecialGrid.Grid)
            {
                cell.LinkChanged += SpecialCell_LinkChanged;
            }


            GoldCost = new DXLabel
            {
                Parent = this,
                Text = "0",
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            GoldCost.Location = new Point(ClientArea.Right - GoldCost.Size.Width - 60, ItemToUpgradeGrid.Location.Y);

            label = new DXLabel
            {
                Parent = this,
                Text = "Gold Cost:",
            };
            label.Location = new Point(GoldCost.Location.X - label.Size.Width - 5, GoldCost.Location.Y + (GoldCost.Size.Height - label.Size.Height) / 2);

            SubmitButton = new DXButton
            {
                Label = { Text = "Submit" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Right - SubmitButton.Size.Width, ClientArea.Bottom - SubmitButton.Size.Height);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (ItemToUpgradeGrid.Grid[0].Link == null) return;
                CellLinkInfo itemInfo = new CellLinkInfo { Count = ItemToUpgradeGrid.Grid[0].LinkedCount, GridType = ItemToUpgradeGrid.Grid[0].Link.GridType, Slot = ItemToUpgradeGrid.Grid[0].Link.Slot };
                ItemToUpgradeGrid.Grid[0].Link.Locked = true;
                ItemToUpgradeGrid.Grid[0].Link = null;

                List<CellLinkInfo> sacrificeItems = new List<CellLinkInfo>();
                foreach (DXItemCell cell in SacrificeItemGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    sacrificeItems.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CellLinkInfo special = null;
                if (SpecialGrid.Grid[0].Link != null)
                {
                    special = new CellLinkInfo { Count = SpecialGrid.Grid[0].LinkedCount, GridType = SpecialGrid.Grid[0].Link.GridType, Slot = SpecialGrid.Grid[0].Link.Slot };
                    SpecialGrid.Grid[0].Link.Locked = true;
                    SpecialGrid.Grid[0].Link = null;
                }

                CEnvir.Enqueue(new C.NPCItemUpgrade { Item = itemInfo, SacrificeItems = sacrificeItems, SpecialItem = special});
            };
        }

        #region Methods
        private void ItemToUpgradeCell_LinkChanged(object sender, EventArgs e)
        {
            ClientUserItem item = null;
            string sacrifice = "Items to sacrifice";
            string special = "Special";
            int goldCost = 0;
            foreach (DXItemCell cell in ItemToUpgradeGrid.Grid)
            {
                if (cell.Link?.Item == null) continue;
                item = cell.Link.Item;
            }
            if(item != null)
            {
                goldCost = (int)((float)item.Info.Price * Globals.EquipmentUpgradeList[item.Level].GoldMultiplier);
                int specialItemIndex = Globals.EquipmentUpgradeList[item.Level].SpecialItem;
                if (specialItemIndex != -1)
                {
                    special = Globals.UpgradeSpecialItems[specialItemIndex];
                }
                else
                {
                    special = "None";
                }

                if (Globals.EquipmentUpgradeList[item.Level].NumberOfItems > 0)
                {
                    sacrifice = Globals.UpgradeSacrificeItems[item.Info.SetValue - 2] + String.Format(": required {0}", Globals.EquipmentUpgradeList[item.Level].NumberOfItems);
                }
                else
                {
                    sacrifice = "None";
                }
            }

            RequiredSpecial.Text = special;
            RequiredItem.Text = sacrifice;
            GoldCost.Text = goldCost.ToString("#,##0");
            CheckUnlockButton();
        }

        private void SacrificeItemCell_LinkChanged(object sender, EventArgs e)
        {
            sacrificeItems = 0;
            foreach (DXItemCell cell in SacrificeItemGrid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sacrificeItems++;
            }
            CheckUnlockButton();
        }

        private void SpecialCell_LinkChanged(object sender, EventArgs e)
        {
            specialItems = false;
            foreach (DXItemCell cell in SpecialGrid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                specialItems = true;
            }
            CheckUnlockButton();
        }

        private void CheckUnlockButton()
        {
            ClientUserItem item = null;
            foreach (DXItemCell cell in ItemToUpgradeGrid.Grid)
            {
                if (cell.Link?.Item == null) continue;
                item = cell.Link.Item;
            }
            if(item != null)
            {
                Globals.EquipmentUpgradeCost cost = Globals.EquipmentUpgradeList[item.Level];
                SubmitButton.Enabled = (cost.NumberOfItems == sacrificeItems) && (cost.SpecialItem != -1 ? specialItems : true);
            }
            else
            {
                SubmitButton.Enabled = false;
            }
        }

        #endregion
    }
    public sealed class NPCQuestDialog : DXWindow
    {
        #region Properties

        #region NPCInfo

        public NPCInfo NPCInfo
        {
            get => _NPCInfo;
            set
            {
                if (_NPCInfo == value) return;

                NPCInfo oldValue = _NPCInfo;
                _NPCInfo = value;

                OnNPCInfoChanged(oldValue, value);
            }
        }
        private NPCInfo _NPCInfo;
        public event EventHandler<EventArgs> NPCInfoChanged;
        public void OnNPCInfoChanged(NPCInfo oValue, NPCInfo nValue)
        {
            NPCInfoChanged?.Invoke(this, EventArgs.Empty);


            UpdateQuestDisplay();
        }

        #endregion

        #region SelectedQuest

        public NPCQuestRow SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                if (_SelectedQuest == value) return;

                NPCQuestRow oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private NPCQuestRow _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(NPCQuestRow oValue, NPCQuestRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            foreach (DXItemCell cell in RewardGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            if (SelectedQuest?.QuestInfo == null)
            {
                TasksLabel.Text = string.Empty;
                DescriptionLabel.Text = string.Empty;

                AcceptButton.Visible = false;
                CompleteButton.Visible = false;
                EndLabel.Text = string.Empty;
                return;
            }

            SelectedQuest.Selected = true;
            
            int standard = 0, choice = 0;
            HasChoice = false;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ClientUserItem item = new ClientUserItem(reward.Item, reward.Amount)
                {
                    Flags = flags,
                    ExpireTime = duration
                };

                if (reward.Choice)
                {
                    if (choice >= ChoiceGrid.Grid.Length) continue;
                    
                    HasChoice = true;

                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;
                    choice++;
                }
                else
                {
                    if (standard >= RewardGrid.Grid.Length) continue;

                    RewardGrid.Grid[standard].Item = item;
                    RewardGrid.Grid[standard].Tag = reward;
                    standard++;
                }
            }

            if (HasChoice)
                SelectedCell = null;


            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, false);
            TasksLabel.Text = GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            EndLabel.Text = SelectedQuest.QuestInfo.FinishNPC.RegionName;

            AcceptButton.Visible = SelectedQuest.UserQuest == null;
            CompleteButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete;

            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedCell

        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                DXItemCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private DXItemCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue != null)
            {
                oValue.FixedBorder = false;
                oValue.Border = false;
                oValue.FixedBorderColour = false;
                oValue.BorderColour = Color.Lime;
            }

            if (nValue != null)
            {
                nValue.Border = true;
                nValue.FixedBorder = true;
                nValue.FixedBorderColour = true;
                nValue.BorderColour = Color.Lime;
            }
            
            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public NPCQuestRow[] Rows;

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXVScrollBar ScrollBar;

        public DXLabel TasksLabel, DescriptionLabel, EndLabel;

        public DXItemGrid RewardGrid, ChoiceGrid;

        public DXButton AcceptButton, CompleteButton;

        public ClientUserItem[] RewardArray, ChoiceArray;

        public bool HasChoice;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCQuestDialog()
        {
            TitleLabel.Text = "Quests";

            HasFooter = false;
            Movable = false;
            SetClientSize(new Size(300, 487));
            Location = new Point(GameScene.Game.NPCBox.Size.Width, 0);

            DXLabel label = new DXLabel
            {
                Text = "Log",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
            };

            Rows = new NPCQuestRow[6];

            DXControl panel = new DXControl
            {
                Size = new Size(ClientArea.Width, 2+ Rows.Length * 22),
                Location = new Point(ClientArea.X, ClientArea.Top + label.Size.Height),
                Parent = this,
                DrawTexture = true,
            };


            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCQuestRow
                {
                    Parent = panel,
                    Location = new Point(2, 2 + i*22)
                };
                int index = i;
                Rows[index].MouseClick += (o, e) =>
                {
                    if (Rows[index].QuestInfo == null) return;

                    SelectedQuest = Rows[index];
                };
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = panel,
                Location = new Point(panel.Size.Width - 15, 3),
                Size = new Size(14, Rows.Length * 22 - 4),
                VisibleSize = Rows.Length,
                Change = 1,
            };
            ScrollBar.ValueChanged += (o,e) => UpdateScrollBar();

            label = new DXLabel
            {
                Text = "Details",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, panel.Location.Y + panel.Size.Height + 5),
            };

            
            DescriptionLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(ClientArea.Width - 4, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = "Tasks",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 5),
            };


            TasksLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(ClientArea.Width - 4, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = "Rewards",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, TasksLabel.Location.Y + TasksLabel.Size.Height + 5),
            };

            RewardArray = new ClientUserItem[5];
            RewardGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + 2, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(RewardArray.Length, 1),
                ItemGrid = RewardArray,
                ReadOnly = true,
            };

            label = new DXLabel
            {
                Text = "Choice",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(RewardGrid.Location.X + 13 + RewardGrid.Size.Width, TasksLabel.Location.Y + TasksLabel.Size.Height + 5),
            };

            ChoiceArray = new ClientUserItem[3];
            ChoiceGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(RewardGrid.Location.X + 16 + RewardGrid.Size.Width, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(ChoiceArray.Length, 1),
                ItemGrid = ChoiceArray,
                ReadOnly = true,
            };

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {

                cell.MouseClick += (o, e) =>
                {
                    if (((DXItemCell)o).Item == null) return;

                    SelectedCell = (DXItemCell) o;
                };
            }

            label = new DXLabel
            {
                Text = "End:",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10),
            };

            EndLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12)/2),
            };
            EndLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.FinishNPC.Region.Map;

            };


            AcceptButton = new DXButton
            {
                Label = { Text = "Accept" },
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Size.Width - 100), label.Location.Y + label.Size.Height + 5),
                Size = new Size(100, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            AcceptButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                CEnvir.Enqueue(new C.QuestAccept { Index = SelectedQuest.QuestInfo.Index });
            };

            CompleteButton = new DXButton
            {
                Label = { Text = "Complete" },
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Size.Width - 100), ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10),
                Size = new Size(100, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            CompleteButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                if (HasChoice && SelectedCell == null)
                {
                    GameScene.Game.ReceiveChat("Please select a reward.", MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.QuestComplete { Index = SelectedQuest.QuestInfo.Index, ChoiceIndex = ((QuestReward) SelectedCell?.Tag)?.Index ?? 0 });
            };
        }

        #region Methods

        public void UpdateQuestDisplay()
        {
            if (NPCInfo == null)
            {
                Visible = false;
                return;
            }

            Quests.Clear();

            List<QuestInfo> availableQuests = new List<QuestInfo>(), currentQuests = new List<QuestInfo>(), completeQuests = new List<QuestInfo>();

            foreach (QuestInfo quest in NPCInfo.StartQuests)
            {
                if (!GameScene.Game.CanAccept(quest)) continue;

                availableQuests.Add(quest);
            }
            
            foreach (QuestInfo quest in NPCInfo.FinishQuests)
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);

                if (userQuest == null || userQuest.Completed) continue;

                if (!userQuest.IsComplete)
                    currentQuests.Add(quest);
                else
                    completeQuests.Add(quest);
            }


            completeQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            availableQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            currentQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));

            Quests.AddRange(completeQuests);
            Quests.AddRange(availableQuests);
            Quests.AddRange(currentQuests);

            Visible = Quests.Count > 0;

            if (Quests.Count == 0) return;

            QuestInfo previousQuest = SelectedQuest?.QuestInfo;

            _SelectedQuest = null;

            UpdateScrollBar();

            if (previousQuest != null)
            {
                foreach (NPCQuestRow row in Rows)
                {
                    if (row.QuestInfo != previousQuest) continue;

                    _SelectedQuest = row;
                    break;
                }
            }

            if (SelectedQuest == null)
                SelectedQuest = Rows[0];

            if (SelectedQuest?.QuestInfo != null)
            {
                DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, false);
                TasksLabel.Text = GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

                AcceptButton.Visible = SelectedQuest.UserQuest == null;
                CompleteButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete;
            }
        }

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Quests.Count;
            
            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].QuestInfo = i + ScrollBar.Value >= Quests.Count ? null : Quests[i + ScrollBar.Value];
            }


        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _NPCInfo = null;
                NPCInfoChanged = null;

                Quests.Clear();
                Quests = null;

                HasChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                _SelectedCell = null;
                SelectedCellChanged = null;
                
                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (TasksLabel != null)
                {
                    if (!TasksLabel.IsDisposed)
                        TasksLabel.Dispose();

                    TasksLabel = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (EndLabel != null)
                {
                    if (!EndLabel.IsDisposed)
                        EndLabel.Dispose();

                    EndLabel = null;
                }

                if (RewardGrid != null)
                {
                    if (!RewardGrid.IsDisposed)
                        RewardGrid.Dispose();

                    RewardGrid = null;
                }

                if (ChoiceGrid != null)
                {
                    if (!ChoiceGrid.IsDisposed)
                        ChoiceGrid.Dispose();

                    ChoiceGrid = null;
                }

                if (AcceptButton != null)
                {
                    if (!AcceptButton.IsDisposed)
                        AcceptButton.Dispose();

                    AcceptButton = null;
                }

                if (CompleteButton != null)
                {
                    if (!CompleteButton.IsDisposed)
                        CompleteButton.Dispose();

                    CompleteButton = null;
                }

                RewardArray = null;
                ChoiceArray = null;
            }

        }

        #endregion
    }

    public sealed class NPCQuestRow : DXControl
    {
        #region Properties

        #region QuestInfo

        public QuestInfo QuestInfo
        {
            get => _QuestInfo;
            set
            {
                QuestInfo oldValue = _QuestInfo;
                _QuestInfo = value;

                OnQuestInfoChanged(oldValue, value);
            }
        }
        private QuestInfo _QuestInfo;
        public event EventHandler<EventArgs> QuestInfoChanged;
        public void OnQuestInfoChanged(QuestInfo oValue, QuestInfo nValue)
        {
            if (QuestInfo == null)
            {
                Selected = false;
                UserQuest = null;
                QuestNameLabel.Text = string.Empty;
                QuestIcon.Visible = false;
            }
            else
            {
                UserQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == QuestInfo);
                QuestNameLabel.Text = QuestInfo.QuestName;
                QuestIcon.Visible = true;
            }

            if (UserQuest == null)
                QuestIcon.BaseIndex = 83; //Available
            else if (!UserQuest.IsComplete)
                QuestIcon.BaseIndex = 85; //Completed
            else
                QuestIcon.BaseIndex = 93; //Current
            
            QuestInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UserQuest

        public ClientUserQuest UserQuest
        {
            get => _UserQuest;
            set
            {
                ClientUserQuest oldValue = _UserQuest;
                _UserQuest = value;

                OnUserQuestChanged(oldValue, value);
            }
        }
        private ClientUserQuest _UserQuest;
        public event EventHandler<EventArgs> UserQuestChanged;
        public void OnUserQuestChanged(ClientUserQuest oValue, ClientUserQuest nValue)
        {
            UserQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXAnimatedControl QuestIcon;
        public DXLabel QuestNameLabel;

        #endregion
        
        public NPCQuestRow()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(280, 20);

            QuestIcon = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(2,2),
                Loop = true,
                LibraryFile = LibraryFile.Interface,
                BaseIndex = 83,
                FrameCount = 2,
                AnimationDelay = TimeSpan.FromSeconds(1),
                Visible = false,
                IsControl = false,
            };

            QuestNameLabel = new DXLabel
            {
                Location = new Point(20, 2),
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _QuestInfo = null;
                QuestInfoChanged = null;

                _UserQuest = null;
                UserQuestChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (QuestIcon != null)
                {
                    if (!QuestIcon.IsDisposed)
                        QuestIcon.Dispose();

                    QuestIcon = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCAdoptCompanionDialog : DXWindow
    {
        #region Properties

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel NameLabel, IndexLabel, PriceLabel;
        public DXButton LeftButton, RightButton, AdoptButton, UnlockButton;

        public DXTextBox CompanionNameTextBox;

        public List<CompanionInfo> AvailableCompanions = new List<CompanionInfo>();

        #region SelectedCompanionInfo

        public CompanionInfo SelectedCompanionInfo
        {
            get => _SelectedCompanionInfo;
            set
            {
                if (_SelectedCompanionInfo == value) return;

                CompanionInfo oldValue = _SelectedCompanionInfo;
                _SelectedCompanionInfo = value;

                OnSelectedCompanionInfoChanged(oldValue, value);
            }
        }
        private CompanionInfo _SelectedCompanionInfo;
        public event EventHandler<EventArgs> SelectedCompanionInfoChanged;
        public void OnSelectedCompanionInfoChanged(CompanionInfo oValue, CompanionInfo nValue)
        {
            CompanionDisplay = null;

            if (SelectedCompanionInfo?.MonsterInfo == null) return;

            CompanionDisplay = new MonsterObject(SelectedCompanionInfo);

            RefreshUnlockButton();

            PriceLabel.Text = SelectedCompanionInfo.Price.ToString("#,##0");
            NameLabel.Text = SelectedCompanionInfo.MonsterInfo.MonsterName;
            SelectedCompanionInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        
        

        #endregion

        #region SelectedIndex

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                int oldValue = _SelectedIndex;
                _SelectedIndex = value;

                OnSelectedIndexChanged(oldValue, value);
            }
        }
        private int _SelectedIndex;
        public event EventHandler<EventArgs> SelectedIndexChanged;
        public void OnSelectedIndexChanged(int oValue, int nValue)
        {
            if (SelectedIndex >= Globals.CompanionInfoList.Count) return;

            SelectedCompanionInfo = Globals.CompanionInfoList[SelectedIndex];

            IndexLabel.Text = $"{SelectedIndex + 1} of {Globals.CompanionInfoList.Count}";

            LeftButton.Enabled = SelectedIndex > 0;

            RightButton.Enabled = SelectedIndex < Globals.CompanionInfoList.Count - 1;

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AdoptAttempted

        public bool AdoptAttempted
        {
            get => _AdoptAttempted;
            set
            {
                if (_AdoptAttempted == value) return;

                bool oldValue = _AdoptAttempted;
                _AdoptAttempted = value;

                OnAdoptAttemptedChanged(oldValue, value);
            }
        }
        private bool _AdoptAttempted;
        public event EventHandler<EventArgs> AdoptAttemptedChanged;
        public void OnAdoptAttemptedChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            AdoptAttemptedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region CompanionNameValid

        public bool CompanionNameValid
        {
            get => _CompanionNameValid;
            set
            {
                if (_CompanionNameValid == value) return;

                bool oldValue = _CompanionNameValid;
                _CompanionNameValid = value;

                OnCompanionNameValidChanged(oldValue, value);
            }
        }
        private bool _CompanionNameValid;
        public event EventHandler<EventArgs> CompanionNameValidChanged;
        public  void OnCompanionNameValidChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            CompanionNameValidChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public bool CanAdopt => GameScene.Game.User != null && SelectedCompanionInfo != null && SelectedCompanionInfo.Price <= GameScene.Game.User.Gold && !AdoptAttempted && !UnlockButton.Visible && CompanionNameValid;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCAdoptCompanionDialog()
        {
            TitleLabel.Text = "Adopt Companion";
            
            Movable = false;

            SetClientSize(new Size(275, 130));
            CompanionDisplayPoint = new Point(40, 95);

            NameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            NameLabel.SizeChanged += (o, e) =>
            {
                NameLabel.Location = new Point(CompanionDisplayPoint.X  + 25 - NameLabel.Size.Width / 2, CompanionDisplayPoint.Y + 30);
            };

            IndexLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(CompanionDisplayPoint.X , 200),
            };
            IndexLabel.SizeChanged += (o, e) =>
            {
                IndexLabel.Location = new Point(CompanionDisplayPoint.X  + 25 - IndexLabel.Size.Width / 2, CompanionDisplayPoint.Y + 55);
            };
            LeftButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 32,
                Location = new Point(CompanionDisplayPoint.X - 20, CompanionDisplayPoint.Y + 55)
            };
            LeftButton.MouseClick += (o, e) => SelectedIndex--;
            RightButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 37,
                Location = new Point(CompanionDisplayPoint.X + 60, CompanionDisplayPoint.Y + 55)
            };
            RightButton.MouseClick += (o, e) => SelectedIndex++;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Price:"
            };
            label.Location = new Point(160 - label.Size.Width, CompanionDisplayPoint.Y);

            PriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(160 , CompanionDisplayPoint.Y),
                ForeColour = Color.White,
            };

            CompanionNameTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(160, CompanionDisplayPoint.Y + 25),
                Size = new Size(120, 20)
            };
            CompanionNameTextBox.TextBox.TextChanged += TextBox_TextChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "Name:"
            };
            label.Location = new Point(CompanionNameTextBox.Location.X - label.Size.Width, CompanionNameTextBox.Location.Y + (CompanionNameTextBox.Size.Height - label.Size.Height)/2);

            AdoptButton = new DXButton
            {
                Parent = this,
                Location = new Point(CompanionNameTextBox.Location.X, CompanionNameTextBox.Location.Y + 27),
                Size = new Size(120, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Adopt" }
            };
            AdoptButton.MouseClick += AdoptButton_MouseClick;

                UnlockButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 80, ClientArea.Y),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Unlock" }
            };

            UnlockButton.MouseClick += UnlockButton_MouseClick;

            SelectedIndex = 0;
        }

        #region Methods
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            CompanionNameValid = Globals.CharacterReg.IsMatch(CompanionNameTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(CompanionNameTextBox.TextBox.Text))
                CompanionNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                CompanionNameTextBox.BorderColour = CompanionNameValid ? Color.Green : Color.Red;
        }

        private void AdoptButton_MouseClick(object sender, MouseEventArgs e)
        {
            AdoptAttempted = true;

            CEnvir.Enqueue(new C.CompanionAdopt { Index = SelectedCompanionInfo.Index, Name = CompanionNameTextBox.TextBox.Text });
        }
        private void UnlockButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Inventory.All(x => x == null || x.Info.Effect != ItemEffect.CompanionTicket))
            {
                GameScene.Game.ReceiveChat("You need a Companion Ticket to unlock a new appearance", MessageType.System);
                return;
            }

            DXMessageBox box = new DXMessageBox($"Are you sure you want to use a Companion Ticket?\n\n" + $"" + $"This will unlock the {SelectedCompanionInfo.MonsterInfo.MonsterName} appearance for new companions", "Unlock Appearance", DXMessageBoxButtons.YesNo);


            box.YesButton.MouseClick += (o1, e1) =>
            {
                CEnvir.Enqueue(new C.CompanionUnlock { Index = SelectedCompanionInfo.Index });

                UnlockButton.Enabled = false;
            };
        }

        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }

        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }


            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }

        public void RefreshUnlockButton()
        {

            UnlockButton.Visible = !SelectedCompanionInfo.Available && !AvailableCompanions.Contains(SelectedCompanionInfo);

            if (GameScene.Game.User == null || SelectedCompanionInfo == null || SelectedCompanionInfo.Price <= GameScene.Game.User.Gold)
                PriceLabel.ForeColour = Color.FromArgb(198, 166, 99);
            else
                PriceLabel.ForeColour = Color.Red;


            AdoptButton.Enabled = CanAdopt;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                _SelectedCompanionInfo = null;
                SelectedCompanionInfoChanged = null;

                _SelectedIndex = 0;
                SelectedIndexChanged = null;

                _AdoptAttempted = false;
                AdoptAttemptedChanged = null;

                _CompanionNameValid = false;
                CompanionNameValidChanged = null;
                    
                
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (IndexLabel != null)
                {
                    if (!IndexLabel.IsDisposed)
                        IndexLabel.Dispose();

                    IndexLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (LeftButton != null)
                {
                    if (!LeftButton.IsDisposed)
                        LeftButton.Dispose();

                    LeftButton = null;
                }

                if (RightButton != null)
                {
                    if (!RightButton.IsDisposed)
                        RightButton.Dispose();

                    RightButton = null;
                }
                
                if (AdoptButton != null)
                {
                    if (!AdoptButton.IsDisposed)
                        AdoptButton.Dispose();

                    AdoptButton = null;
                }

                if (UnlockButton != null)
                {
                    if (!UnlockButton.IsDisposed)
                        UnlockButton.Dispose();

                    UnlockButton = null;
                }

                if (CompanionNameTextBox != null)
                {
                    if (!CompanionNameTextBox.IsDisposed)
                        CompanionNameTextBox.Dispose();

                    CompanionNameTextBox = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCCompanionStorageDialog : DXWindow
    {
        #region Properties

        private DXVScrollBar ScrollBar;

        public NPCCompanionStorageRow[] Rows;

        public List<ClientUserCompanion> Companions = new List<ClientUserCompanion>();


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCCompanionStorageDialog()
        {
            TitleLabel.Text = "Storage";

            Movable = false;

            SetClientSize(new Size(198, 349));

            Rows = new NPCCompanionStorageRow[4];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCCompanionStorageRow
                {
                    Parent = this,
                    Location = new Point(ClientArea.X, ClientArea.Y + i*88),
                };
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 15, ClientArea.Y + 1),
                Size = new Size(14, Rows.Length * 87 -1),
                VisibleSize = Rows.Length,
                Change = 1,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();
        }

        #region Methods

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Companions.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].UserCompanion = i + ScrollBar.Value >= Companions.Count ? null : Companions[i + ScrollBar.Value];
            }


        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Companions.Clear();
                Companions = null;
                
                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCCompanionStorageRow : DXControl
    {
        #region Properties
        #region UserCompanion

        public ClientUserCompanion UserCompanion
        {
            get => _UserCompanion;
            set
            {
                ClientUserCompanion oldValue = _UserCompanion;
                _UserCompanion = value;

                OnUserCompanionChanged(oldValue, value);
            }
        }
        private ClientUserCompanion _UserCompanion;
        public event EventHandler<EventArgs> UserCompanionChanged;
        public void OnUserCompanionChanged(ClientUserCompanion oValue, ClientUserCompanion nValue)
        {
            UserCompanionChanged?.Invoke(this, EventArgs.Empty);

            if (UserCompanion == null)
            {
                Visible = false;
                return;
            }

            Visible = true;

            CompanionDisplay = new MonsterObject(UserCompanion.CompanionInfo);

            NameLabel.Text = UserCompanion.Name;
            LevelLabel.Text = $"Level {UserCompanion.Level}";

            if (UserCompanion == GameScene.Game.Companion)
                Selected = true;
            else
            {
                Selected = false;

                if (!string.IsNullOrEmpty(UserCompanion.CharacterName))
                {
                    RetrieveButton.Enabled = false;
                    RetrieveButton.Hint = $"The Companion is currently with {UserCompanion.CharacterName}.";
                }
                else
                {
                    RetrieveButton.Enabled = true;
                    RetrieveButton.Hint = null;
                }

            }
        }

        #endregion
        
        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            RetrieveButton.Visible = !Selected;
            StoreButton.Visible = Selected;


            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;
        public DXLabel NameLabel, LevelLabel;
        public DXButton StoreButton, RetrieveButton;

        #endregion

        public NPCCompanionStorageRow()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(180, 85);
            CompanionDisplayPoint = new Point(10, 45);

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(85,5)

            };

            LevelLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(85, 30)
            };

            StoreButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Store" },
                Visible = false
            };
            StoreButton.MouseClick += StoreButton_MouseClick;


            RetrieveButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Retrieve" }
            };
            RetrieveButton.MouseClick += RetrieveButton_MouseClick;


        }

        #region Methods

        private void StoreButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionStore { Index = UserCompanion.Index });
        }

        private void RetrieveButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionRetrieve { Index = UserCompanion.Index });
        }

        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }

        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }


            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }

        
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _UserCompanion = null;
                UserCompanionChanged = null;
                
                _Selected = false;
                SelectedChanged = null;

                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (StoreButton != null)
                {
                    if (!StoreButton.IsDisposed)
                        StoreButton.Dispose();

                    StoreButton = null;
                }

                if (RetrieveButton != null)
                {
                    if (!RetrieveButton.IsDisposed)
                        RetrieveButton.Dispose();

                    RetrieveButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCWeddingRingDialog : DXWindow
    {
        #region Properties

        public DXItemGrid RingGrid;
        public DXButton BindButton;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCWeddingRingDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(60, 85));
            CloseButton.Visible = false;

            DXLabel label = new DXLabel
            {
                Text = "Ring",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
            RingGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Width - 36)/2, label.Size.Height + label.Location.Y + 5),
                GridSize = new Size(1, 1),
                Linked = true,
                GridType = GridType.WeddingRing,
            };

            RingGrid.Grid[0].LinkChanged += (o, e) => BindButton.Enabled = RingGrid.Grid[0].Item != null;
            RingGrid.Grid[0].BeforeDraw += (o, e) => Draw(RingGrid.Grid[0], 31);

            BindButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point((ClientArea.Width - 50)/2 + ClientArea.X, ClientArea.Bottom - SmallButtonHeight),
                Label = { Text = "Bind" },
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled =  false,
            };
            BindButton.MouseClick += (o, e) =>
            {
                if (RingGrid.Grid[0].Item == null || RingGrid.Grid[0].Item.Info.ItemType != ItemType.Ring) return;


                CEnvir.Enqueue(new C.MarriageMakeRing {  Slot = RingGrid.Grid[0].Link.Slot });

                RingGrid.Grid[0].Link = null;
            };
        }
        
        #region Methods

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }
        
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (RingGrid != null)
                {
                    if (!RingGrid.IsDisposed)
                        RingGrid.Dispose();

                    RingGrid = null;
                }

                if (BindButton != null)
                {
                    if (!BindButton.IsDisposed)
                        BindButton.Dispose();

                    BindButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCItemFragmentDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton FragmentButton;
        public DXLabel CostLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }
        
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCItemFragmentDialog()
        {
            TitleLabel.Text = "Fragment Items";

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.ItemFragment,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 50));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();


            CostLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "Fragment Cost:",
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "Select All" },
                Location = new Point(ClientArea.X, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            FragmentButton = new DXButton
            {
                Label = { Text = "Fragment" },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            FragmentButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCFragment { Links = links });
            };
        }

        #region Methods
        private void CalculateCost()
        {
            int sum = 0;

            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.FragmentCost();
                count++;
            }

            CostLabel.ForeColour = sum > MapObject.User.Gold ? Color.Red : Color.White;

            CostLabel.Text = sum.ToString("#,##0");

            FragmentButton.Enabled = sum <= MapObject.User.Gold && count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (FragmentButton != null)
                {
                    if (!FragmentButton.IsDisposed)
                        FragmentButton.Dispose();

                    FragmentButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCUpgradeGemDialog : DXWindow
    {
        #region Properties
        public DXItemGrid TargetCell;
        public DXItemGrid Grid;
        public DXButton UpgradeButton;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                TargetCell.ClearLinks();
                Grid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCUpgradeGemDialog()
        {
            TitleLabel.Text = "Upgrade Gem";

            DXLabel label = new DXLabel
            {
                Text = "Item to upgrade",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            Movable = false;
            SetClientSize(new Size(label.Size.Width + 50, label.Size.Height + 150));
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y);

            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.EquipmentUpgradeGemTarget,
                Linked = true,
            };
            TargetCell.Location = new Point(label.Location.X + (label.Size.Width - TargetCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Gem",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(TargetCell.Location.X + (TargetCell.Size.Width - label.Size.Width) / 2, TargetCell.Location.Y + 60);

            Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.EquipmentUpgradeGemItems,
                Linked = true
            };
            Grid.Location = new Point(label.Location.X + (label.Size.Width - Grid.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            UpgradeButton = new DXButton
            {
                Label = { Text = "Upgrade" },
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            UpgradeButton.Location = new Point(Grid.Location.X + (Grid.Size.Width - UpgradeButton.Size.Width) / 2, Grid.Location.Y + Grid.Size.Height + 5);
            UpgradeButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                DXItemCell target = TargetCell.Grid[0];
                DXItemCell gem = Grid.Grid[0];

                if (target.Link == null) return;
                if (gem.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = target.LinkedCount, GridType = target.Link.GridType, Slot = target.Link.Slot };
                target.Link.Locked = true;
                target.Link = null;

                CellLinkInfo gemLink = new CellLinkInfo { Count = gem.LinkedCount, GridType = gem.Link.GridType, Slot = gem.Link.Slot };
                gem.Link.Locked = true;
                gem.Link = null;

                CEnvir.Enqueue(new C.NPCUpgradeGem { Target = targetLink, Gem = gemLink});
            };
            Grid.Grid[0].LinkChanged += (o, e) => ShouldEnableButton();
            TargetCell.Grid[0].LinkChanged += (o, e) => ShouldEnableButton();
        }

        #region Methods
        private void ShouldEnableButton()
        {
            UpgradeButton.Enabled = Grid.Grid[0].Item != null && TargetCell.Grid[0].Item != null;
        }

        #endregion
    }

    public sealed class NPCLevelUpDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton LevelUpButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                Grid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public NPCLevelUpDialog()
        {
            TitleLabel.Text = "Level Up Scrolls";

            Movable = false;
            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.LevelUpScrolls,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 110));
            Grid.Location = new Point(ClientArea.X, ClientArea.Y + 60);

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => ShouldEnableButton();

            LevelUpButton = new DXButton
            {
                Label = { Text = "Level Up" },
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            LevelUpButton.Location = new Point(ClientArea.X + (ClientArea.Width - LevelUpButton.Size.Width) / 2, ClientArea.Bottom - 45);
            LevelUpButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCLevelUpScroll { Links = links });
            };
        }

        #region Methods
        private void ShouldEnableButton()
        {
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                count++;
            }

            LevelUpButton.Enabled = count > 0;
        }

        #endregion
    }

    public class NPCWeaponCraftWindow : DXWindow
    {
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        private DXComboBox ClassComboBox;

        private DXImageControl PreviewImageBox;


        public DXItemGrid TemplateCell;

        public DXItemGrid YellowCell;
        public DXItemGrid BlueCell;
        public DXItemGrid RedCell;
        public DXItemGrid PurpleCell;
        public DXItemGrid GreenCell;
        public DXItemGrid GreyCell;

        private DXLabel ClassLabel;

        private DXButton AttemptButton;
        
        
        #region RequiredClass

        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                RequiredClass oldValue = _RequiredClass;
                _RequiredClass = value;

                OnRequiredClassChanged(oldValue, value);
            }
        }
        private RequiredClass _RequiredClass;
        public event EventHandler<EventArgs> RequiredClassChanged;
        public virtual void OnRequiredClassChanged(RequiredClass oValue, RequiredClass nValue)
        {

            if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.Effect == ItemEffect.WeaponTemplate)
            {
                switch (RequiredClass)
                {
                    case RequiredClass.None:
                        PreviewImageBox.Index = 1110;
                        break;
                    case RequiredClass.Warrior:
                        PreviewImageBox.Index = 1111;
                        break;
                    case RequiredClass.Wizard:
                        PreviewImageBox.Index = 1112;
                        break;
                    case RequiredClass.Taoist:
                        PreviewImageBox.Index = 1113;
                        break;
                    case RequiredClass.Assassin:
                        PreviewImageBox.Index = 1114;
                        break;

                }
            }
            else
            {
                PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
            }

            AttemptButton.Enabled = CanCraft;

            RequiredClassChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public long Cost
        {
            get
            {

                long cost = Globals.CraftWeaponPercentCost;

                if (TemplateCell.Grid[0].Item != null && TemplateCell.Grid[0].Item.Info.Effect != ItemEffect.WeaponTemplate)
                {
                    switch (TemplateCell.Grid[0].Item.Info.Rarity)
                    {
                        case Rarity.Common:
                            cost = Globals.CommonCraftWeaponPercentCost;
                            break;
                        case Rarity.Superior:
                            cost = Globals.SuperiorCraftWeaponPercentCost;
                            break;
                        case Rarity.Elite:
                            cost = Globals.EliteCraftWeaponPercentCost;
                            break;
                    }
                }

                return cost;
            }
        }

        public bool CanCraft => Cost <= GameScene.Game.User.Gold && TemplateCell.Grid[0].Link != null && RequiredClass != RequiredClass.None;

        public NPCWeaponCraftWindow()
        {
            TitleLabel.Text = "Weapon Craft";

            HasFooter = false;

            SetClientSize(new Size(250, 280));

            DXLabel label = new DXLabel
            {
                Text = "Template / Weapon",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y);

            TemplateCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftTemplate,
                Linked = true,
            };
            TemplateCell.Location = new Point(label.Location.X + (label.Size.Width - TemplateCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);
            TemplateCell.Grid[0].LinkChanged += (o, e) =>
            {
                if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.Effect == ItemEffect.WeaponTemplate)
                {
                    ClassLabel.Text = "Class:";
                    switch (RequiredClass)
                    {
                        case RequiredClass.None:
                            PreviewImageBox.Index = 1110;
                            break;
                        case RequiredClass.Warrior:
                            PreviewImageBox.Index = 1111;
                            break;
                        case RequiredClass.Wizard:
                            PreviewImageBox.Index = 1112;
                            break;
                        case RequiredClass.Taoist:
                            PreviewImageBox.Index = 1113;
                            break;
                        case RequiredClass.Assassin:
                            PreviewImageBox.Index = 1114;
                            break;

                    }
                }
                else
                {
                    ClassLabel.Text = "Stats:";
                    PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
                }

                ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);

                AttemptButton.Enabled = CanCraft;
            };


            label = new DXLabel
            {
                Text = "Yellow",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 60);
            YellowCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftYellow,
                Linked = true,
            };
            YellowCell.Location = new Point(label.Location.X + (label.Size.Width - YellowCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Blue",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 60);
            BlueCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftBlue,
                Linked = true,
            };
            BlueCell.Location = new Point(label.Location.X + (label.Size.Width - BlueCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Red",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 60);
            RedCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftRed,
                Linked = true,
            };
            RedCell.Location = new Point(label.Location.X + (label.Size.Width - RedCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Purple",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 120);

            PurpleCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftPurple,
                Linked = true,
            };
            PurpleCell.Location = new Point(label.Location.X + (label.Size.Width - PurpleCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Green",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 120);

            GreenCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGreen,
                Linked = true,
            };
            GreenCell.Location = new Point(label.Location.X + (label.Size.Width - GreenCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Grey",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 120);

            GreyCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGrey,
                Linked = true,
            };
            GreyCell.Location = new Point(label.Location.X + (label.Size.Width - GreyCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);


            ClassLabel = new DXLabel
            {
                Text = "Class:",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);
            #region Class
            ClassComboBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(GreenCell.Size.Width + 48, DXComboBox.DefaultNormalHeight),
            };
            ClassComboBox.Location = new Point(GreenCell.Location.X + 1, ClientArea.Y + 185);
            ClassComboBox.SelectedItemChanged += (o, e) =>
            {
                RequiredClass = (RequiredClass?)ClassComboBox.SelectedItem ?? RequiredClass.None;
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.None}" },
                Item = RequiredClass.None
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Warrior}" },
                Item = RequiredClass.Warrior
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Wizard}" },
                Item = RequiredClass.Wizard
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Taoist}" },
                Item = RequiredClass.Taoist
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Assassin}" },
                Item = RequiredClass.Assassin
            };

            ClassComboBox.ListBox.SelectItem(RequiredClass.None);
            #endregion

            #region Preview

            PreviewImageBox = new DXImageControl
            {
                Parent = this,
                Location = new Point(ClientArea.X + 20, ClientArea.Y + ClientArea.Height / 2 - 76),
                LibraryFile = LibraryFile.Equip,
                Index = 1110,
                Border = true,
            };

            #endregion

           

            AttemptButton = new DXButton
            {
                Parent = this,
                Location = new Point(YellowCell.Location.X, ClientArea.Y + 260),
                Size = new Size(YellowCell.Size.Width + 99, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Craft" }
            };
            AttemptButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;


                if (TemplateCell.Grid[0].Link == null) return;

                C.NPCWeaponCraft packet = new C.NPCWeaponCraft
                {
                    Class = RequiredClass,

                    Template = new CellLinkInfo { Count = TemplateCell.Grid[0].LinkedCount, GridType = TemplateCell.Grid[0].Link.GridType, Slot = TemplateCell.Grid[0].Link.Slot }
                };

                TemplateCell.Grid[0].Link.Locked = true;
                TemplateCell.Grid[0].Link = null; 

                if (YellowCell.Grid[0].Link != null)
                {
                    packet.Yellow = new CellLinkInfo { Count = YellowCell.Grid[0].LinkedCount, GridType = YellowCell.Grid[0].Link.GridType, Slot = YellowCell.Grid[0].Link.Slot };
                    YellowCell.Grid[0].Link.Locked = true;
                    YellowCell.Grid[0].Link = null;
                }

                if (BlueCell.Grid[0].Link != null)
                {
                    packet.Blue = new CellLinkInfo { Count = BlueCell.Grid[0].LinkedCount, GridType = BlueCell.Grid[0].Link.GridType, Slot = BlueCell.Grid[0].Link.Slot };
                    BlueCell.Grid[0].Link.Locked = true;
                    BlueCell.Grid[0].Link = null;
                }

                if (RedCell.Grid[0].Link != null)
                {
                    packet.Red = new CellLinkInfo { Count = RedCell.Grid[0].LinkedCount, GridType = RedCell.Grid[0].Link.GridType, Slot = RedCell.Grid[0].Link.Slot };
                    RedCell.Grid[0].Link.Locked = true;
                    RedCell.Grid[0].Link = null;
                }

                if (PurpleCell.Grid[0].Link != null)
                {
                    packet.Purple = new CellLinkInfo { Count = PurpleCell.Grid[0].LinkedCount, GridType = PurpleCell.Grid[0].Link.GridType, Slot = PurpleCell.Grid[0].Link.Slot };
                    PurpleCell.Grid[0].Link.Locked = true;
                    PurpleCell.Grid[0].Link = null;
                }

                if (GreenCell.Grid[0].Link != null)
                {
                    packet.Green = new CellLinkInfo { Count = GreenCell.Grid[0].LinkedCount, GridType = GreenCell.Grid[0].Link.GridType, Slot = GreenCell.Grid[0].Link.Slot };
                    GreenCell.Grid[0].Link.Locked = true;
                    GreenCell.Grid[0].Link = null;
                }

                if (GreyCell.Grid[0].Link != null)
                {
                    packet.Grey = new CellLinkInfo { Count = GreyCell.Grid[0].LinkedCount, GridType = GreyCell.Grid[0].Link.GridType, Slot = GreyCell.Grid[0].Link.Slot };
                    GreyCell.Grid[0].Link.Locked = true;
                    GreyCell.Grid[0].Link = null;
                }

                CEnvir.Enqueue(packet);
                AttemptButton.Enabled = CanCraft;
            };
        }

    }
}