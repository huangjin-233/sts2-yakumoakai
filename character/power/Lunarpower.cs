using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.power
{
    [RegisterPower]
    public sealed class Lunarpower : ModPowerTemplate
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;

        // 叠加的行为
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/lunarpower.png",
            BigIconPath: "res://images/powers/lunarpower.png"
        );

        // 允许层数为负数
        public override bool AllowNegative => false;
    }
}
