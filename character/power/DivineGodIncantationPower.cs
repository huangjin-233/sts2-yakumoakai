using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.power
{
    public class DivineGodIncantationPower : PowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public static Dictionary<Player, int> god = new Dictionary<Player, int>();
        public void SetValue(Player player, int value)
        {
            DivineGodIncantationPower.god[player] = value;
        }
        public static int GetValue(Player player)
        {
            return DivineGodIncantationPower.god.TryGetValue(player, out int value) ? value : 0;
        }
        public static void IncrementValue(Player player, int value)
        {
            DivineGodIncantationPower.god[player] = value;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {
            while (DivineGodIncantationPower.god[base.Owner.Player] >= 10)
            {
                DivineGodIncantationPower.god[base.Owner.Player] -= 10;
                await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);//防御
            }
        }
        public override async Task AfterEnergyReset(Player player)
        {
            await PowerCmd.Remove(this);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
           HoverTipFactory.Static(StaticHoverTip.Block)];
    }
}

