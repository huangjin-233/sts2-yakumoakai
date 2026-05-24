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
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class Medice : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new PowerVar<IntangiblePower>(1)
        ];
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        // 动态变量
        public Medice()
            : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardModel cardModel = CardFactory.GetDistinctForCombat(base.Owner, from c in ModelDb.CardPool<YakumoakaiTokenCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                                                                               where c.Keywords.Contains(AkaiKeyword.Medice.GetModCardKeyword())
                                                                               select c, 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);

        }
        public override string PortraitPath => $"res://images/cards/skill/Medice.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            ModKeywordRegistry.CreateHoverTip(AkaiKeyword.Medice),
         ];
        //关键词
    }
}

