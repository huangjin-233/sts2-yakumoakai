using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
namespace YakumoAkai.character.power
{
	public sealed class mp : PowerModel
	{
		// 效果类型
		public override PowerType Type => PowerType.Buff;
		// 效果堆叠类型
		public override PowerStackType StackType => PowerStackType.Counter;

		// 叠加的行为
		public override bool IsInstanced => false;

		// 允许层数为负数
		public override bool AllowNegative => false;
        public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (base.Owner.GetPowerAmount<mp>() >= 150)
            {
                Amount = 150;
            }
        }
    }
}

