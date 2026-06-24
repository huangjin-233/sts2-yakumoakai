using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.uncommon;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public class Zombiestrength : ModTemporaryAppliedPowerTemplate<ZombieHat,StrengthPower>
    {
        public override AbstractModel OriginModel => ModelDb.Card<ZombieHat>();
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/zombiestrength.png",
            BigIconPath: "res://images/powers/zombiestrength.png"
        );
    }
}

