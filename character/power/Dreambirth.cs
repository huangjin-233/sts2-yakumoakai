using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace YakumoAkai.character.power
{
    public sealed class Dreambirth : PowerModel
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Single;
        public int nowmp;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {
            int nowmp = (int)base.Owner.GetPowerAmount<mp>();
            await PowerCmd.Apply<mp>(Owner, 150m, Owner, null);
        }
        public override async Task AfterEnergyReset(Player player)
        {
            if (player == base.Owner.Player)
            {
                await PowerCmd.TickDownDuration(this);
                await PowerCmd.Apply<mp>(Owner, -300m, Owner, null);
                await PowerCmd.Apply<mp>(Owner, nowmp, Owner, null);
            }
        }
    }
}

