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
    public class DarkKing : MonsterObject
    {
        public int numberOfAttacks = 10;
        int attacksDone = 0;
        int tempAttackDelay;
        MirDirection tempDirection;
        Point tempLocation;

        public DarkKing()
        {
            AvoidFireWall = false;
        }

        public override void ProcessTarget()
        {
            if (Target == null)
            {
                AttackDelay = tempAttackDelay;
                Direction = tempDirection;
                Teleport(CurrentMap, tempLocation, false);
                Stats[Stat.MagicShield] -= 100;
                attacksDone = 0;
                return;
            }

            if(attacksDone <= 0)
            {
                if(CanAttack || CanMove)
                {
                    if (SEnvir.Random.Next(10) == 0)
                    {
                        attacksDone = numberOfAttacks;
                        tempAttackDelay = AttackDelay;
                        AttackDelay = 300;
                        tempDirection = Direction;
                        tempLocation = CurrentLocation;
                        Stats[Stat.MagicShield] += 100;
                    }
                }

                if (InAttackRange())
                {
                    if (CanAttack)
                    {
                        Attack();
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
            else if (CanAttack)
            {
                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, ViewRange);
                MapObject newTarget = targets[SEnvir.Random.Next(targets.Count)];

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
                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       Target,
                                       GetDC() + newTarget.Stats[Stat.Health] / 100,
                                       AttackElement));
                    attacksDone--;
                    if(attacksDone == 0)
                    {
                        AttackDelay = tempAttackDelay;
                        Direction = tempDirection;
                        Teleport(CurrentMap, tempLocation, false);
                        Stats[Stat.MagicShield] -= 100;
                    }
                }
            }
            
        }
    }
}
