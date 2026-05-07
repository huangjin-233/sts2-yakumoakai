using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace YakumoAkai.character.power
{
    public sealed class Clearmirrorpower : PowerModel
    {
        // 效果类型
        public override PowerType Type => PowerType.Buff;
        // 效果堆叠类型
        public override PowerStackType StackType => PowerStackType.Single;

        // 叠加的行为
        public override bool IsInstanced => false;

        // 允许层数为负数
        public override bool AllowNegative => false;
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player == base.Owner.Player)
            {
                    CardModel cardModel = PileType.Exhaust.GetPile(base.Owner.Player).Cards.Where((CardModel c) => !c.Keywords.Contains(CardKeyword.Unplayable)).ToList().StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                 .FirstOrDefault();
                    if (cardModel == null)
                    {
                        cardModel = PileType.Exhaust.GetPile(base.Owner.Player).Cards.ToList().StableShuffle(base.Owner.Player.RunState.Rng.Shuffle).FirstOrDefault();
                    }
                    if (cardModel != null)
                    {
                        await CardCmd.AutoPlay(choiceContext, cardModel, null);
                    }
            }
        }
    }
}

