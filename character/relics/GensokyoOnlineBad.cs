using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    [Pool(typeof(YakumoAkaiRelicPool))]
    public class GensokyoOnlineBad : CustomRelicModel
    {
        // 稀有度
        public override RelicRarity Rarity => RelicRarity.Starter;


        // 小图标（原版85x85）
        public override string PackedIconPath => $"res://images/relic/gensokyo_online_bad.png";
        // 轮廓图标（原版85x85）
        protected override string PackedIconOutlinePath => $"res://images/relic/outline/gensokyo_online_bad.png";
        // 大图标（原版256x256）
        protected override string BigIconPath => $"res://images/relic/large/gensokyo_online_bad.png";

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            // 判断事件调用时是否为遗物持有者一方，且回合数是否为 1
            if (side == Owner.Creature.Side && combatState.RoundNumber == 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(base.Owner.Creature, 60m, base.Owner.Creature, null);
                await KindSoul.CreateInHand(base.Owner, 1, combatState);
            }
            else if (side == Owner.Creature.Side && combatState.RoundNumber != 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(base.Owner.Creature, 10m, base.Owner.Creature, null);
            }

        }

        // 初始遗物的升级可以写这里
        public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<GensokyoOnline>();
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<KindSoul>(),
            HoverTipFactory.FromPower<mp>()];
    }
    [HarmonyPatch(typeof(TouchOfOrobas))]
    public static class TransBaseRelic
    {
        [HarmonyPostfix]
        [HarmonyPatch("get_RefinementUpgrades")]
        public static void Postfix(ref Dictionary<ModelId, RelicModel> __result)
        {
            var GensokyoOnline = ModelDb.Relic<GensokyoOnline>().Id;
            var GensokyoOnlineBad = ModelDb.Relic<GensokyoOnlineBad>();


            if (!__result.ContainsKey(GensokyoOnline)) __result[GensokyoOnline] = GensokyoOnlineBad;
        }
    }
}