using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace YakumoAkai.character.power
{    
    [RegisterPower]
    public class DivineGodIncantationPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override PowerAssetProfile AssetProfile => new(
            IconPath: "res://images/powers/divine_god_incantation_power.png",
            BigIconPath: "res://images/powers/divine_god_incantation_power.png"
        );

        // 允许层数为负数
        public override bool AllowNegative => false;
        public static Dictionary<Player, decimal> god = new Dictionary<Player, decimal>();
        public void SetValue(Player player, int value)
        {
            god[player] = value;
        }
        public static int GetValue(Player player)
        {
            return (int)(god.TryGetValue(player, out decimal value) ? value : 0);
        }
        public static void IncrementValue(Player player, int value)
        {
            god[player] = value;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {
            while (god[Owner.Player] >= 10)
            {
                god[Owner.Player] -= 10;
                await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);//防御
            }
        }
        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide combatSide, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
           HoverTipFactory.Static(StaticHoverTip.Block)];
    }
}

