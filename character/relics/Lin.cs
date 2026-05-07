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
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    public sealed class Lin : RelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        // 稀有度
        protected override List<DynamicVar> CanonicalVars => [
                   new DamageVar(10m,ValueProp.Unpowered)
                ];
        // 动态变量

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (CombatManager.Instance.IsInProgress && target == base.Owner.Creature && result.UnblockedDamage > 0)
            {
                await CreatureCmd.Damage(choiceContext, base.Owner.Creature.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner.Creature);
            }
            
        }
        [ModInitializer(nameof(Initialize))]
        public static class MyCustomModInitializer
        {
            public static void Initialize()
            {
                ModHelper.AddModelToPool(typeof(YakumoAkaiRelicPool), typeof(Lin));
                var harmony = new Harmony("huangjin.yakumoakai");
                harmony.PatchAll();
                // 初始化 harmony 库
            }
        }
    }
}
