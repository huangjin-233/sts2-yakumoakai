using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.power
{
    public class Maidknifepower : PowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public static Dictionary<Player, int> maid = new Dictionary<Player, int>();
        public void SetValue(Player player, int value)
        {
            Maidknifepower.maid[player] = value;
        }
        public static int GetValue(Player player)
        {
            return Maidknifepower.maid.TryGetValue(player, out int value) ? value : 0;
        }
        public static void IncrementValue(Player player, int value)
        {
            Maidknifepower.maid[player] = value;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {
            while (Maidknifepower.maid[base.Owner.Player] >= 10)
            {
                Maidknifepower.maid[base.Owner.Player] -= 10;
                await Cmd.CustomScaledWait(0.1f, 0.2f);
                Creature creature = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(base.Owner.CombatState.HittableEnemies);
                if (creature != null)
                {
                    await CreatureCmd.Damage(choiceContext, creature, base.Amount, ValueProp.Unpowered, base.Owner);
                }
            }
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
           HoverTipFactory.FromPower<mp>()
            ];
    }
}
