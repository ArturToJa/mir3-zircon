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
    public class Tachinid : MonsterObject
    {
        public override bool CanMove => false;

        public override void ProcessTarget()
        {
            if (Target == null) return;
            if (CanAttack && CanAttackTarget(Target))
            {
                Cell cell = null;
                for (int i = 0; i < 8; i++)
                {
                    cell = CurrentMap.GetCell(Functions.Move(Target.CurrentLocation, Functions.ShiftDirection(Target.Direction, i), 1));

                    if (cell == null || cell.Movements != null)
                    {
                        cell = null;
                        continue;
                    }
                    break;
                }

                if (cell != null)
                {
                    Direction = Functions.DirectionFromPoint(cell.Location, Target.CurrentLocation);
                    Teleport(CurrentMap, cell.Location);
                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       Target,
                                       GetDC(),
                                       AttackElement));
                }
            }
        }
    }
}
