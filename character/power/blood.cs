using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.power
{
    public class blood : PowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature dealer, DamageResult result, ValueProp props, Creature target, CardModel cardSource)
        {
            if (dealer == base.Owner && props.IsPoweredAttack_() && result.UnblockedDamage > 0)
            {
                await CreatureCmd.Heal(base.Owner, base.Amount);
            }
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

