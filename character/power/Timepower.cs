using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Characters;

namespace YakumoAkai.character.power
{
    public sealed class Timepower : PowerModel
    {
        private int _energy;

        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Single;

        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public int num;
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
             num = base.Owner.Player.PlayerCombatState.Energy;
        }
        public override async Task AfterEnergyReset(Player player)
        {
            await PlayerCmd.GainEnergy(num, base.Owner.Player);//能量
            await PowerCmd.Remove(this);
        }
    }
}
