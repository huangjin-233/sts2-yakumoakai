using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    [RegisterCard(typeof(YakumoakaiTokenCardPool))]
    public sealed class Medice1 : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [new CardsVar(2)];// 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Medice.GetModCardKeyword()];
        public Medice1()
        : base(0, CardType.Power, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Penglai>(choiceContext,base.Owner.Creature, 1, base.Owner.Creature, this);//蓬莱
        }
        public override string PortraitPath => $"res://images/cards/power/Medice1.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<Penglai>()];
        //关键词
    }
}

