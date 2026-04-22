using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    public sealed class GensokyoOnline : RelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        // 稀有度

        // 动态变量

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            // 判断事件调用时是否为遗物持有者一方，且回合数是否为 1
            if (side == Owner.Creature.Side && combatState.RoundNumber == 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(base.Owner.Creature, 80m, base.Owner.Creature, null);
                await KindSoul.CreateInHand(base.Owner, 1, combatState);
                IEnumerable<CardModel> distinctForCombat = CardFactory.GetDistinctForCombat(
                base.Owner,
                from c in base.Owner.Creature.Player.Character.CardPool.GetUnlockedCards(
                    base.Owner.Creature.Player.UnlockState,
                    base.Owner.Creature.CombatState.RunState.CardMultiplayerConstraint)
                select c,
                1,
                base.Owner.Creature.CombatState.RunState.Rng.CombatCardGeneration).ToList();
                foreach (CardModel card in distinctForCombat)
                {
                    card.EnergyCost.SetThisTurn(0);
                    CardCmd.Upgrade(card);
                }
                await CardPileCmd.AddGeneratedCardsToCombat(distinctForCombat, PileType.Hand, addedByPlayer: true);
            }
            else if (side == Owner.Creature.Side && combatState.RoundNumber != 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(base.Owner.Creature, 15m, base.Owner.Creature, null);
            }

        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<KindSoul>(),
            HoverTipFactory.FromPower<mp>()];
        [ModInitializer(nameof(Initialize))]
        public static class MyCustomModInitializer
        {
            public static void Initialize()
            {

                ModHelper.AddModelToPool(typeof(YakumoAkaiRelicPool), typeof(GensokyoOnline));

                var harmony = new Harmony("huangjin.yakumoakai");
                harmony.PatchAll();
                // 初始化 harmony 库
            }
        }
    }
}

