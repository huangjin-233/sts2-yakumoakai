using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    [RegisterRelic(typeof(YakumoAkaiRelicPool))]
    public sealed class Lin : RelicModel
    {
        // 小图标（原版85x85）
        public override string PackedIconPath => $"res://images/relic/lin.png";
        // 轮廓图标（原版85x85）
        protected override string PackedIconOutlinePath => $"res://images/relic/outline/lin.png";
        // 大图标（原版256x256）
        protected override string BigIconPath => $"res://images/relic/large/lin.png";
        public override RelicRarity Rarity => RelicRarity.Event;

        // 稀有度
        protected override List<DynamicVar> CanonicalVars =>
        [
            new DamageVar(10m, ValueProp.Unpowered)
        ];
        // 动态变量

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
            DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (CombatManager.Instance.IsInProgress && target == base.Owner.Creature && result.UnblockedDamage > 0)
            {
                await CreatureCmd.Damage(choiceContext, base.Owner.Creature.CombatState.HittableEnemies,
                    base.DynamicVars.Damage, base.Owner.Creature);
            }

        }
    }
}
