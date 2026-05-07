using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.power
{
    public class Fire : ModPowerTemplate, IHealthBarForecastSource
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/fire.png",
        BigIconPath: "res://images/powers/fire.png"
    );
        public override bool IsInstanced => false;

        // 允许层数为负数
        public static int mp1;
        public override bool AllowNegative => false;
        public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
        {
            return HealthBarForecasts
                .FromRight(context, new(1f, 0.478f, 0f), Colors.Orange)
                .Add(base.Amount, HealthBarForecastOrder.ForSideTurnStart(context.Creature, Owner.Side))
                .Build();
        }

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

