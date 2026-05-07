using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

        private Node2D _vfxNode;  // 保存特效引用
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Single;

        // 叠加的行为


        // 允许层数为负数
        public override bool AllowNegative => false;
        public int num;
        public void SetVfxNode(Node2D vfxNode)
        {
            _vfxNode = vfxNode;
        }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
             num = Owner.Player.PlayerCombatState.Energy;
        }
        public override async Task AfterEnergyReset(Player player)
        {
            await PlayerCmd.GainEnergy(num, Owner.Player);//能量
            await PowerCmd.Remove(this);
            if (_vfxNode != null && GodotObject.IsInstanceValid(_vfxNode))
            {
                _vfxNode.QueueFree();
                _vfxNode = null;
            }
        }
    }
}
