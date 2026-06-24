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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public sealed class Nextmp : ModPowerTemplate
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;

        // 叠加的行为

        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/nextmp.png",
            BigIconPath: "res://images/powers/nextmp.png"
        );
        // 允许层数为负数
        public override bool AllowNegative => false;
        public override async Task AfterEnergyReset(Player player)
        {
            if (player == base.Owner.Player)
            {
                await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(),Owner, 20m,Owner, null);
                await PowerCmd.Remove(this);
            }
        }
    }
}

