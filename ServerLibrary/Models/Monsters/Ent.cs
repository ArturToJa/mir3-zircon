using System;
using System.Collections.Generic;
using System.Drawing;
using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class Ent : MonsterObject
    {

        protected override bool InAttackRange()
        {
            if(MonsterInfo.IsBoss)
            {
                if (Target.CurrentMap != CurrentMap) return false;

                return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange);
            }
            else
            {
                return base.InAttackRange();
            }
        }

        public override void ProcessTarget()
        {
            if(Target == null) return;
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

            if (!CanAttack || !InAttackRange()) return;

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.PoisonDust, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            foreach (Cell cell in CurrentMap.GetCells(CurrentLocation, 0, ViewRange))
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
                        GetDC(),
                        AttackElement));
                }
            }
        }
    }
}
