using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public sealed class Rebirth : ModPowerTemplate
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;

        // 叠加的行为
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/rebirth.png",
            BigIconPath: "res://images/powers/rebirth.png"
        );

        // 允许层数为负数
        public override bool AllowNegative => false;
        public override bool ShouldDieLate(Creature creature)
        {
            if (creature != base.Owner)
            {
                return true;
            }
            return false;
        }
        public override async Task AfterPreventingDeath(Creature creature)
        {
            decimal amount = Math.Max(1m, (decimal)base.Owner.MaxHp * 0.8m);
            await CreatureCmd.Heal(base.Owner, amount);
            await PowerCmd.TickDownDuration(this);
        }
    }
}

