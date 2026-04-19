using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace YakumoAkai.character.power
{
    public sealed class Lazy : PowerModel
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;

        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
        {
            if (card.Type == CardType.Attack) 
            {
                return false;
            }
            return true;
        }
        public override async Task AfterEnergyReset(Player player)
        {
            if (player == base.Owner.Player)
            {
                await PowerCmd.TickDownDuration(this);
            }
        }
    }
}
