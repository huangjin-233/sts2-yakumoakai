using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.special;

namespace YakumoAkai.character.card.ancient
{
    [RegisterCard(typeof(YakumoAkaiCardPool))]
    public sealed class MarlboroGavel : CardModel
    {
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public MarlboroGavel()
            : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IEnumerable<CardModel> curse = await Curse.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌
            if (base.IsUpgraded)
            {
                foreach (CardModel item in curse)
                {
                    CardCmd.Upgrade(item);//升级
                }
            }
            IEnumerable<CardModel> ghost = await Ghost.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌                                                                                                                                      
            if (base.IsUpgraded)
            {
                foreach (CardModel item in ghost)
                {
                    CardCmd.Upgrade(item);//升级
                }
            }
            IEnumerable<CardModel> white = await White.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌                                                                                                                                      
            if (base.IsUpgraded)
            {
                foreach (CardModel item in white)
                {
                    CardCmd.Upgrade(item);//升级
                }
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Marlboro_gavel.png";

        protected override void OnUpgrade()
        {
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<Curse>(),
            HoverTipFactory.FromCard<Ghost>(),
            HoverTipFactory.FromCard<White>()];
        //关键词
    }
}

