using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.uncommon;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public class Zombiedex :ModTemporaryAppliedPowerTemplate<ZombieHat,DexterityPower>
    {
        public override AbstractModel OriginModel => ModelDb.Card<ZombieHat>();
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/zombiedex.png",
            BigIconPath: "res://images/powers/zombiedex.png"
        );
    }
}

