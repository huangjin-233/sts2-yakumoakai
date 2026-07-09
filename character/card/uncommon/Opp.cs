using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool

))]
    public sealed class Opp : CardModel
    {
        // 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public Opp()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var powers = cardPlay.Target.Powers.Where(p => p.Type == PowerType.Debuff && p.Amount > 0).ToList();
            foreach (var powered in powers)
            {
                await PowerCmd.Apply(choiceContext, powered, cardPlay.Target, powered.Amount, null, cardSource: this);
            }
        }

        public override string PortraitPath => $"res://images/cards/skill/Opp.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}

