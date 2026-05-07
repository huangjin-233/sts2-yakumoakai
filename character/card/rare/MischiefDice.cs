using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace YakumoAkai.character.card.rare
{
    public sealed class MischiefDice : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<IntangiblePower>(1)
        ];
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public MischiefDice()
            : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IReadOnlyList<CardModel> cards1 = PileType.Draw.GetPile(base.Owner).Cards;
            IReadOnlyList<CardModel> cards2 = PileType.Discard.GetPile(base.Owner).Cards;
            List<CardModel> cards = new List<CardModel>();
            cards.AddRange(cards1);
            cards.AddRange(cards2);
            foreach (CardModel dice in await CardSelectCmd.FromSimpleGrid(choiceContext,cards, base.Owner, new CardSelectorPrefs(RelicModel.L10NLookup("从弃牌堆和抽牌堆选择一张牌加入手卡"), 1)))
            {
                await CardPileCmd.Add(dice, PileType.Hand);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/none.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            // 升级后
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(MischiefDice));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
