using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace YakumoAkai.character.power
{
    public sealed class Yukafight : PowerModel
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;

        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public static Dictionary<Player, int> fight = new Dictionary<Player, int>();
        public void SetValue(Player player, int value)
        {
            Yukafight.fight[player] = value;
        }
        public static int GetValue(Player player)
        {
            return Yukafight.fight.TryGetValue(player, out int value) ? value : 0;
        }
        public static void IncrementValue(Player player, int value)
        {
            Yukafight.fight[player] = value;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == base.Owner.Player || cardPlay.Card.Type == CardType.Attack)
            {
                Yukafight.fight[base.Owner.Player]  += 1;
            }
            if (Yukafight.fight[base.Owner.Player] >= 2)
            {
                await PowerCmd.Apply<StrengthPower>(Owner, base.Amount, Owner, null);
                Yukafight.fight[base.Owner.Player] -= 2;
            }
        }
        public override async Task AfterEnergyReset(Player player)
        {
            if (player == base.Owner.Player)
            {
                await PowerCmd.Remove(this);
            }
        }
    }

}
