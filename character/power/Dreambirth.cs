using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.card.rare;

namespace YakumoAkai.character.power
{
    public sealed class Dreambirth : PowerModel
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        private Node2D _vfxNode;  // 保存特效引用
        public void SetVfxNode(Node2D vfxNode)
        {
            _vfxNode = vfxNode;
        }
        // 允许层数为负数
        public override bool AllowNegative => false;
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {
            await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(),Owner, 9999, Owner, null);
        }
        public override async Task AfterEnergyReset(Player player)
        {
            if (player == base.Owner.Player)
            {
                await PowerCmd.TickDownDuration(this);
                await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(),Owner, -9999, Owner, null);
                await PowerCmd.Apply<mp>(new ThrowingPlayerChoiceContext(),Owner, DreamBirth.nowmp, Owner, null);
                if (_vfxNode != null && GodotObject.IsInstanceValid(_vfxNode))
                {
                    _vfxNode.QueueFree();
                    _vfxNode = null;
                }
            }
        }
    }
}

