using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.power
{
    public class Fire : PowerModel
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public static int mp1;
        public override bool AllowNegative => false;
        private int TriggerCount
        {
            get
            {
                IEnumerable<Creature> source = from c in base.Owner.CombatState.GetOpponentsOf(base.Owner)
                                               where c.IsAlive
                                               select c;
                return Math.Min(base.Amount, 1 + source.Sum((Creature a) => a.GetPowerAmount<AccelerantPower>()));
            }
        }
        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)

        {
            if (side != base.Owner.Side)
            {
                return;
            }
            int iterations = TriggerCount;
            for (int i = 0; i < iterations; i++)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            }
        }
    }
}

