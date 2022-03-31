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
    public class StrongBoss : MonsterObject
    {
        public StrongBoss()
        {
            AvoidFireWall = false;
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
            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, ViewRange);
            if (targets.Count > 0)
            {
                MapObject newTarget = targets[SEnvir.Random.Next(targets.Count)];
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
                        UpdateAttackTime();
                    }
                }
                foreach(MapObject target in targets)
                {
                    if (target != null && CanAttackTarget(target))
                    {
                        Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                        ActionList.Add(new DelayedAction(
                                           SEnvir.Now.AddMilliseconds(400),
                                           ActionType.DelayAttack,
                                           Target,
                                           GetDC() + GetSC() * target.Stats[Stat.Health] / 100,
                                           AttackElement));
                    }
                }
            }
        }
    }
}
