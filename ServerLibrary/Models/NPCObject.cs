using System;
using System.Drawing;
using System.Linq;
using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public class NPCObject : MapObject
    {
        public override ObjectType Race => ObjectType.NPC;

        public NPCInfo NPCInfo;

        public override string Name => NPCInfo.NPCName;

        public override bool Blocking => Visible;


        public void NPCCall(PlayerObject ob, NPCPage page)
        {
            while (true)
            {
                if (page == null) return;

                NPCPage failPage;
                if (!CheckPage(ob, page, out failPage))
                {
                    page = failPage;
                    continue;
                }

                DoActions(ob, page);

                if (page.SuccessPage != null)
                {
                    page = page.SuccessPage;
                    continue;
                }

                if (string.IsNullOrEmpty(page.Say))
                {
                    ob.NPC = null;
                    ob.NPCPage = null;
                    ob.Enqueue(new S.NPCClose());
                    return;
                }

                ob.NPC = this;
                ob.NPCPage = page;

                ob.Enqueue(new S.NPCResponse { ObjectID = ObjectID, Index = page.Index, ExpEventNumber = SEnvir.ExpEventLimit - SEnvir.ExpEventCount, BossEventNumber = SEnvir.BossEventLimit - SEnvir.BossEventCount });
                break;
            }
        }

        private void DoActions(PlayerObject ob, NPCPage page)
        {
            foreach (NPCAction action in page.Actions)
            {
                switch (action.ActionType)
                {
                    case NPCActionType.Teleport:
                        if (action.MapParameter1 == null && action.InstanceParameter1 == null) return;

                        if (action.InstanceParameter1 != null)
                        {
                            if (ob.CurrentMap.Instance != null)
                            {
                                return;
                            }

                            var index = SEnvir.LoadInstance(action.InstanceParameter1);

                            if (index == null) return;

                            ob.Teleport(action.InstanceParameter1.ConnectRegion, action.InstanceParameter1, index.Value);
                        }
                        else
                        {
                            Map map = SEnvir.GetMap(action.MapParameter1);

                            if (action.IntParameter1 == 0 && action.IntParameter2 == 0)
                                ob.Teleport(map, map.GetRandomLocation());
                            else
                                ob.Teleport(map, new Point(action.IntParameter1, action.IntParameter2));
                        }
                        break;
                    case NPCActionType.TakeGold:
                        ob.Gold -= action.IntParameter1;
                        ob.GoldChanged();
                        break;
                    case NPCActionType.ChangeElement:
                        UserItem weapon = ob.Equipment[(int) EquipmentSlot.Weapon];

                        S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                        result.NewStats[Stat.WeaponElement] = action.IntParameter1 - weapon.Stats[Stat.WeaponElement];

                        weapon.AddStat(Stat.WeaponElement, action.IntParameter1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        weapon.StatsChanged();
                        ob.RefreshStats();

                        ob.Enqueue(result);
                        break;
                    case NPCActionType.ChangeHorse:
                        ob.Character.Account.Horse = (HorseType) action.IntParameter1;

                        ob.RemoveMount();

                        ob.RefreshStats();

                        if (ob.Character.Account.Horse != HorseType.None) ob.Mount();
                        break;
                    case NPCActionType.GiveGold:

                        long gold = ob.Gold + action.IntParameter1;
                        
                        ob.Gold = (long) gold;
                        ob.GoldChanged();

                        break;
                    case NPCActionType.Marriage:
                        ob.MarriageRequest();
                        break;
                    case NPCActionType.Divorce:
                        ob.MarriageLeave();
                        break;
                    case NPCActionType.RemoveWeddingRing:
                        ob.MarriageRemoveRing();
                        break;
                    case NPCActionType.GiveItem:
                        if (action.ItemParameter1 == null) continue;

                        ItemCheck check = new ItemCheck(action.ItemParameter1, action.IntParameter1, UserItemFlags.None, TimeSpan.Zero);

                        if (!ob.CanGainItems(false, check)) continue;

                        while (check.Count > 0)
                            ob.GainItem(SEnvir.CreateFreshItem(check));

                        break;
                    case NPCActionType.TakeItem:
                        if (action.ItemParameter1 == null) continue;

                        ob.TakeItem(action.ItemParameter1, action.IntParameter1);
                        break;
                    case NPCActionType.ResetWeapon:
                        break;
                    case NPCActionType.GiveItemExperience:
                        break;
                    case NPCActionType.SpecialRefine:
                        break;
                    case NPCActionType.Rebirth:
                        if (Globals.RebirthDataList.Count > (ob.Character.Rebirth + 1) && ob.Level >= Globals.RebirthDataList[ob.Character.Rebirth + 1].RequiredLevel)
                            ob.NPCRebirth();
                        break;

                    case NPCActionType.GiveLevels:
                        ob.Level = Math.Min(ob.Level + action.IntParameter1, Config.MaxLevel);
                        ob.LevelUp();
                        break;
                    case NPCActionType.TakeLevels:
                        ob.Level = Math.Max(ob.Level - action.IntParameter1, 1);
                        ob.LevelUp();
                        break;
                    case NPCActionType.LearnSkill:
                        MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Index == action.IntParameter1);
                        UserMagic magic;
                        if (!ob.Magics.TryGetValue(info.Magic, out magic))
                        {
                            magic = SEnvir.UserMagicList.CreateNewObject();
                            magic.Character = ob.Character;
                            magic.Info = info;
                            magic.Level = 5;
                            ob.Magics[info.Magic] = magic;

                            ob.Enqueue(new S.NewMagic { Magic = magic.ToClientInfo() });

                            ob.Connection.ReceiveChat(string.Format(ob.Connection.Language.LearnBookSuccess, magic.Info.Name), MessageType.System);

                            foreach (SConnection con in ob.Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.LearnBookSuccess, magic.Info.Name), MessageType.System);

                            RefreshStats();
                        }
                        break;
                    case NPCActionType.SetSkilllevel:
                        info = SEnvir.MagicInfoList.Binding.First(x => x.Index == action.IntParameter1);
                        if (!ob.Magics.TryGetValue(info.Magic, out magic))
                        {
                            magic.Level = action.IntParameter2;
                            magic.Experience = 0;

                            ob.Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });

                            ob.Connection.ReceiveChat(string.Format(ob.Connection.Language.LearnBookSuccess, magic.Info.Name, magic.Level), MessageType.System);

                            foreach (SConnection con in ob.Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.LearnBookSuccess, magic.Info.Name, magic.Level), MessageType.System);

                            RefreshStats();
                        }
                        break;
                }
            }
        }
        private bool CheckPage(PlayerObject ob, NPCPage page, out NPCPage failPage)
        {
            failPage = null;
            foreach (NPCCheck check in page.Checks)
            {
                failPage = check.FailPage;
                UserItem weap;
                switch (check.CheckType)
                {
                    case NPCCheckType.Level:
                        if (!Compare(check.Operator, ob.Level, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Class:
                        if (!Compare(check.Operator, (int)ob.Class, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Gold:
                        if (!Compare(check.Operator, ob.Gold, check.IntParameter1)) return false;
                        break;

                    case NPCCheckType.HasWeapon:
                        if (ob.Equipment[(int)EquipmentSlot.Weapon] != null != (check.Operator == Operator.Equal)) return false;
                        break;

                    case NPCCheckType.WeaponLevel:
                        if (!Compare(check.Operator, ob.Equipment[(int)EquipmentSlot.Weapon].Level, check.IntParameter1)) return false;
                        break;

                    case NPCCheckType.WeaponCanRefine:
                        if ((ob.Equipment[(int)EquipmentSlot.Weapon].Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable != (check.Operator == Operator.Equal)) return false;
                        break;
                    case NPCCheckType.WeaponAddedStats:
                        if (!Compare(check.Operator, ob.Equipment[(int)EquipmentSlot.Weapon].Stats[check.StatParameter1], check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.WeaponElement:
                        weap = ob.Equipment[(int)EquipmentSlot.Weapon];

                        Stat element;
                        int value = 0;

                        switch ((Element)check.IntParameter1)
                        {
                            case Element.Null:
                            case Element.None:
                                value += weap.Stats.GetWeaponElementValue();
                                value += weap.Info.Stats.GetWeaponElementValue();
                                break;
                            case Element.Fire:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.FireAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Ice:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.IceAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Lightning:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.LightningAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Wind:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.WindAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Holy:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.HolyAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Dark:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.DarkAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Phantom:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.PhantomAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }
                                break;
                        }


                        if (!Compare(check.Operator, value, check.IntParameter2)) return false;
                        break;
                    case NPCCheckType.PKPoints:
                        if (!Compare(check.Operator, ob.Stats[Stat.PKPoint], check.IntParameter1 == 0 ? Config.RedPoint : check.IntParameter1) && ob.Stats[Stat.Redemption] == 0)
                            return false;
                        break;
                    case NPCCheckType.Horse:
                        if (!Compare(check.Operator, (int)ob.Character.Account.Horse, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Marriage:
                        if (check.Operator == Operator.Equal)
                        {
                            if (ob.Character.Partner == null) return false;
                        }
                        else
                        {
                            if (ob.Character.Partner != null) return false;
                        }
                        break;
                    case NPCCheckType.WeddingRing:
                        if (check.Operator == Operator.Equal)
                        {
                            if (ob.Equipment[(int) EquipmentSlot.RingL] == null || (ob.Equipment[(int) EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage) return false;
                        }
                        else
                        {
                            if (ob.Equipment[(int)EquipmentSlot.RingL] != null && (ob.Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                        }
                        break;
                    case NPCCheckType.HasItem:
                        if (check.ItemParameter1 == null) continue;
                        if (!Compare(check.Operator, ob.GetItemCount(check.ItemParameter1), check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.CanGainItem:
                        if (check.ItemParameter1 == null) continue;

                        ItemCheck itemCheck = new ItemCheck(check.ItemParameter1, check.IntParameter1, UserItemFlags.None, TimeSpan.Zero);

                        if (!ob.CanGainItems(false, itemCheck)) return false;
                        break;
                    case NPCCheckType.CanResetWeapon:
                        if (check.Operator == Operator.Equal)
                        {
                            if (SEnvir.Now < ob.Equipment[(int)EquipmentSlot.Weapon].ResetCoolDown) return false;
                        }
                        else
                        {
                            if (SEnvir.Now >= ob.Equipment[(int)EquipmentSlot.Weapon].ResetCoolDown) return false;
                        }
                        break;
                    case NPCCheckType.Random:
                        if (!Compare(check.Operator, SEnvir.Random.Next(check.IntParameter1), check.IntParameter2)) return false;
                        break;
                    case NPCCheckType.ExpEventStarted:
                        if (!SEnvir.Events.Exists(x => x.CurrentMap.Info.FileName.StartsWith("12_"))) return false;
                        break;
                }
            }
            return true;
        }


        private bool Compare(Operator op, long pValue, long cValue)
        {
            switch (op)
            {
                case Operator.Equal:
                    return pValue == cValue;
                case Operator.NotEqual:
                    return pValue != cValue;
                case Operator.LessThan:
                    return pValue < cValue;
                case Operator.LessThanOrEqual:
                    return pValue <= cValue;
                case Operator.GreaterThan:
                    return pValue > cValue;
                case Operator.GreaterThanOrEqual:
                    return pValue >= cValue;
                default: return false;
            }
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectNPC
            {
                ObjectID = ObjectID,

                NPCIndex = NPCInfo.Index,

                CurrentLocation =  CurrentLocation,

                Direction = Direction,
            };
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override bool CanDataBeSeenBy(PlayerObject ob)
        {
            return false;
        }

        public override Packet GetDataPacket(PlayerObject ob)
        {
            return null;
        }
    }
}
