using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.card.uncommon;

namespace YakumoAkai.character.power
{
    public class Zombiedex : TemporaryDexterityPower
    {
        public override AbstractModel OriginModel => ModelDb.Card<ZombieHat>();
    }
}

