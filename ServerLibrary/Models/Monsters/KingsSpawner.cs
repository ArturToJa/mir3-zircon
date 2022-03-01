using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class KingsSpawner : MonsterObject
    {
        public MonsterInfo MonsterSpawnInfo;
        public int NumberToSpawn;

        public override void Die()
        {
            base.Die();

            if (MonsterSpawnInfo == null) return;
            if(EXPOwner.GroupMembers != null)
            {
                foreach (PlayerObject ob in EXPOwner.GroupMembers)
                {
                    if (ob.CurrentMap != CurrentMap || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Config.MaxViewRange)) continue;

                    for (int i = 0; i < NumberToSpawn; i++)
                    {
                        MonsterObject mob = GetMonster(MonsterSpawnInfo);
                        mob.Spawn(CurrentMap, ob.CurrentLocation);
                    }
                }
            }
            else 
            {
                for (int i = 0; i < NumberToSpawn; i++)
                {
                    MonsterObject mob = GetMonster(MonsterSpawnInfo);
                    mob.Spawn(CurrentMap, EXPOwner.CurrentLocation);
                }
            }
        }
    }
}
