using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace YakumoAkai.character.power
{
    public class Kind : PowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1),new CardsVar(1),new PowerVar<mp>(0)];
        public static Dictionary<Player,int>mp=new Dictionary<Player,int>();
        public void SetValue(Player player,int value)
        {
            Kind.mp[player] = value;
        }
        public static int GetValue(Player player)
        {
            return Kind.mp.TryGetValue(player,out int value) ? value : 0;
        }
        public static void IncrementValue(Player player,int value)
        {
            Kind.mp[player] = value;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)

        {  
            while (Kind.mp[base.Owner.Player] >= 20)
            {
                Kind.mp[base.Owner.Player]-= 20;
                await PlayerCmd.GainEnergy(Amount, base.Owner.Player);
                await CardPileCmd.Draw(choiceContext, Amount, base.Owner.Player);
            }
            base.DynamicVars.Power<mp>().BaseValue = Kind.mp[base.Owner.Player];
        }
    }
}

