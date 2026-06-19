using System.Collections.Generic;
using System.Linq;
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
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    [RegisterRelic(typeof(YakumoAkaiRelicPool))]
    public sealed class GensokyoOnline : RelicModel
    {
        // 小图标（原版85x85）
        public override string PackedIconPath => $"res://images/relic/gensokyo_online.png";

        // 轮廓图标（原版85x85）
        protected override string PackedIconOutlinePath => $"res://images/relic/outline/gensokyo_online.png";

        // 大图标（原版256x256）
        protected override string BigIconPath => $"res://images/relic/large/gensokyo_online.png";
        public override RelicRarity Rarity => RelicRarity.Starter;
        // 稀有度

        // 动态变量

        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
            ICombatState combatState)
        {
            // 判断事件调用时是否为遗物持有者一方，且回合数是否为 1
            if (side == Owner.Creature.Side && combatState.RoundNumber == 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 80m,
                    base.Owner.Creature, null);
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

                await CardPileCmd.AddGeneratedCardsToCombat(distinctForCombat, PileType.Hand, Owner);
            }
            else if (side == Owner.Creature.Side && combatState.RoundNumber != 1)
            {
                Flash(); // 触发遗物图标闪烁
                await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 15m,
                    base.Owner.Creature, null);
            }

        }

    }
}

