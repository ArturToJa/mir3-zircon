using System;
using System.Collections.Generic;
using System.Text;
using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public class EventObject
    {
        public SpawnInfo Info;
        public Map CurrentMap;

        public DateTime Start;
        public DateTime End;

        public EventObject(SpawnInfo info, InstanceInfo instance, byte index, DateTime end)
        {
            Info = info;
            CurrentMap = SEnvir.GetMap(info.CurrentMap.Info, instance, index);
            Start = SEnvir.Now;
            End = end;
            if (CurrentMap == null)
            {
                End = SEnvir.Now;
            }
        }

        public void DoEvent()
        {
            if (SEnvir.Now < End)
            {
                Info.DoSpawn(true);
            }
            else
            {
                for (int i = CurrentMap.Players.Count - 1; i >= 0; i--)
                {
                    PlayerObject player = CurrentMap.Players[i];
                    player.Teleport(player.Character.BindPoint.BindRegion, CurrentMap.Instance, CurrentMap.InstanceIndex);
                }
                SEnvir.Events.Remove(this);
                for(int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                {
                    if (CurrentMap.Objects[i] == null || !(CurrentMap.Objects[i] is MonsterObject)) continue;
                    MonsterObject monster = CurrentMap.Objects[i] as MonsterObject;
                    monster.EXPOwner = null;
                    monster.Die();
                }
            }
        }
    }
}
