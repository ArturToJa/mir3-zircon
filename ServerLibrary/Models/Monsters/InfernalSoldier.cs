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
    public class InfernalSoldier : MonsterObject
    {
        List<Cell> cells;

        public InfernalSoldier()
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

        public void RangeAttack()
        {
            MagicType type = MagicType.FireWall;

            switch (SEnvir.Random.Next(5))
            {
                case 0:
                    LineAoE(12, -2, 2, MagicType.MonsterScortchedEarth, Element.None);
                    break;
                
                default:
                    switch (SEnvir.Random.Next(2))
                    {
                        case 0:
                            //FireWall();
                            type = MagicType.FireStorm;
                            break;
                            /*                        case 1:
                                                        //AttackMagic(MagicType.FireStorm, Element.Fire, false);
                                                        type = MagicType.FireWall;
                                                        break;*/
                    }
                    List<uint> targetIDs = new List<uint>();
                    List<Point> locations = new List<Point>();

                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = type, Targets = targetIDs, Locations = locations });

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
                                GetDC(),
                                Element.Fire));
                        }
                    }
                    break;
            }
        }
    }
}
