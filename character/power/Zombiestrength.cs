using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.card.uncommon;

namespace YakumoAkai.character.power
{
    public class Zombiestrength : TemporaryStrengthPower
    {
        public override AbstractModel OriginModel => ModelDb.Card<ZombieHat>();
    }
}

