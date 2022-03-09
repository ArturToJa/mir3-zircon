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
    public class DemonicLunaticMirror : MonsterObject
    {
        public DemonicLunatic Owner;

        public DemonicLunaticMirror()
        {
            AvoidFireWall = false;
        }

        public override void Die()
        {
            Owner.minions.Remove(this);
            base.Die();
        }
    }
}
