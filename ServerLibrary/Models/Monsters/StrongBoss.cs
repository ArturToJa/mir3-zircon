﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class StrongBoss : MonsterObject
    {
        List<Cell> cells;
        public StrongBoss()
        {
            AvoidFireWall = false;
        }

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            cells = CurrentMap.GetCells(CurrentLocation, 0, ViewRange);
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange())
            {
                if (CanAttack)
                {
                    RangeAttack();
                }
            }
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
        }

        private void RangeAttack()
        {
            MapObject newTarget = Target;
            if (newTarget != null && CanAttackTarget(newTarget))
            {
                MirDirection dir = (MirDirection)SEnvir.Random.Next(8);
                Cell cell = null;
                for (int i = 0; i < 8; i++)
                {
                    cell = CurrentMap.GetCell(Functions.Move(newTarget.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                    if (cell == null || cell.Movements != null)
                    {
                        cell = null;
                        continue;
                    }
                    break;
                }

                if (cell != null)
                {
                    Direction = Functions.DirectionFromPoint(cell.Location, newTarget.CurrentLocation);
                    Teleport(CurrentMap, cell.Location);
                }
            }
            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.FireStorm, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (!CanAttackTarget(ob)) continue;

                    targetIDs.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob,
                        GetDC() + GetSC() * ob.Stats[Stat.Health] / 100,
                        Element.Null));
                }
            }
        }
    }
}