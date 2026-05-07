using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class Glasses : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<IntangiblePower>(1)
        ];
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public Glasses()
            : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IReadOnlyList<CardModel> cards = PileType.Draw.GetPile(base.Owner).Cards;
            foreach (CardModel glass in await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 1)))
            {
                glass.EnergyCost.SetUntilPlayed(0);
                await CardPileCmd.Add(glass, PileType.Draw, CardPilePosition.Top);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Glasses.png";

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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Glasses));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
