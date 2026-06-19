using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Dream : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new CardsVar(3) // 伤害值
        ];
        // 动态变量
        public Dream()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int count = base.Owner.PlayerCombatState.Hand.Cards.Count;
            await CardCmd.Discard(choiceContext, PileType.Hand.GetPile(base.Owner).Cards);
            IReadOnlyList<CardModel> cards = PileType.Discard.GetPile(base.Owner).Cards;
            foreach (CardModel glass in await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, new CardSelectorPrefs(RelicModel.L10NLookup("DREAM.selectionScreenPrompt"), count)))
            {
                await CardPileCmd.Add(glass, PileType.Hand);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Dream.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
    }
}
