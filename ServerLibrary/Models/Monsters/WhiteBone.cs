﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class WhiteBone : MonsterObject
    {
        public WhiteBone()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddSeconds(1), ActionType.Function));
            Direction = MirDirection.DownLeft;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.SummonSkeleton });

            ActionTime = SEnvir.Now.AddSeconds(1);
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Appear();
                    return;
            }

            base.ProcessAction(action);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CurrentLocation == Target.CurrentLocation)
                {
                    MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                    MoveTo(Target.CurrentLocation);

                return;
            }

            if (!CanAttack) return;

            Attack();

            int distance = ViewRange;
            MagicType magic = MagicType.MonsterScortchedEarth;
            Element element = Element.Null;

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = magic, Targets = targetIDs, Locations = locations });

            for (int i = 1; i <= distance; i++)
            {
                Point location = Functions.Move(CurrentLocation, Direction, i);
                Cell cell = CurrentMap.GetCell(location);

                if (cell == null) continue;

                locations.Add(cell.Location);

                if (cell.Objects != null)
                {
                    foreach (MapObject ob in cell.Objects)
                    {
                        if (!CanAttackTarget(ob)) continue;

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + i * 75),
                            ActionType.DelayAttack,
                            ob,
                            GetDC(),
                            element));
                    }
                }

                switch (Direction)
                {
                    case MirDirection.Up:
                    case MirDirection.Right:
                    case MirDirection.Down:
                    case MirDirection.Left:
                        cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -2)));

                        if (cell?.Objects != null)
                        {
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (!CanAttackTarget(ob)) continue;

                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500 + i * 75),
                                    ActionType.DelayAttack,
                                    ob,
                                    GetDC() / 2,
                                    element));
                            }
                        }
                        cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 2)));

                        if (cell?.Objects != null)
                        {
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (!CanAttackTarget(ob)) continue;

                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500 + i * 75),
                                    ActionType.DelayAttack,
                                    ob,
                                    GetDC() / 2,
                                    element));
                            }
                        }
                        break;
                    case MirDirection.UpRight:
                    case MirDirection.DownRight:
                    case MirDirection.DownLeft:
                    case MirDirection.UpLeft:
                        cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -1)));

                        if (cell?.Objects != null)
                        {
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (!CanAttackTarget(ob)) continue;

                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500 + i * 75),
                                    ActionType.DelayAttack,
                                    ob,
                                    GetDC() / 2,
                                    element));
                            }
                        }
                        cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 1)));

                        if (cell?.Objects != null)
                        {
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (!CanAttackTarget(ob)) continue;

                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500 + i * 75),
                                    ActionType.DelayAttack,
                                    ob,
                                    GetDC() / 2,
                                    element));
                            }
                        }
                        break;
                }
            }
        }

        public void Appear()
        {
            Visible = true;
            AddAllObjects();
        }
    }
}
