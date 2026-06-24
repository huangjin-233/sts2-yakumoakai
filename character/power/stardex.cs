using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.uncommon;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public class Stardex : ModTemporaryAppliedPowerTemplate<StarlightAndGhostlySteps,DexterityPower>
    {
        public override AbstractModel OriginModel => ModelDb.Card<StarlightAndGhostlySteps>();
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/stardex.png",
            BigIconPath: "res://images/powers/stardex.png"
        );
    }
}

