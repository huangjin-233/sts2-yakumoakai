using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class PalmLeaf : CardModel
    {
        private CardModel _mockSelectedCard;
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<IntangiblePower>(1)
        ];
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public PalmLeaf()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardModel cardModel;
            if (_mockSelectedCard == null)
            {
                List<CardModel> cards1 = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<IroncladCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                List<CardModel> cards2 = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<SilentCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                List<CardModel> cards3 = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<RegentCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                List<CardModel> cards4 = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<NecrobinderCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                List<CardModel> cards5 = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<DefectCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                List<CardModel> card = new List<CardModel>();
                card.AddRange(cards1);
                card.AddRange(cards2);
                card.AddRange(cards3);
                card.AddRange(cards4);
                card.AddRange(cards5);
                List<CardModel> cards = new List<CardModel>();
                for (int i = 0; i < 3; i++)
                {
                    int cardid = base.Owner.Creature.CombatState.RunState.Rng.CombatCardGeneration.NextInt(card.Count);
                    cards.Add(card[cardid]);
                    card.RemoveAt(cardid);
                }
                cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);
            }
            else
            {
                cardModel = _mockSelectedCard;
            }
            if (cardModel != null)
            {
                cardModel.EnergyCost.SetThisTurnOrUntilPlayed(0);
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
            }
        }
        public void MockSelectedCard(CardModel card)
        {
            AssertMutable();
            _mockSelectedCard = card;
        }
        public override string PortraitPath => $"res://images/cards/skill/PalmLeaf.png";

        protected override void OnUpgrade()
        {
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(PalmLeaf));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

