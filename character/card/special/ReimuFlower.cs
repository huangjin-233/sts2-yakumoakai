using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    [RegisterCard(typeof(YakumoakaiTokenCardPool))]
    public sealed class ReimuFlower : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => 
            [new PowerVar<ArtifactPower>(1), 
            new PowerVar<StrengthPower>(3), 
            new PowerVar<DexterityPower>(3),
            new PowerVar<ThornsPower>(1),
            new PowerVar<PlatingPower>(5),
            new PowerVar<IntangiblePower>(1),
            new PowerVar<mp>(90),
            new EnergyVar(2),
            new CardsVar(2)];// 动态变量
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public ReimuFlower()
        : base(0, CardType.Power, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ArtifactPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<ArtifactPower>().BaseValue, base.Owner.Creature, this);//人工制品
            await PowerCmd.Apply<StrengthPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);//力量
            await PowerCmd.Apply<DexterityPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<DexterityPower>().BaseValue, base.Owner.Creature, this);//敏捷
            await PowerCmd.Apply<ThornsPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<ThornsPower>().BaseValue, base.Owner.Creature, this);//荆棘
            await PowerCmd.Apply<PlatingPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<PlatingPower>().BaseValue, base.Owner.Creature, this);//覆甲
            await PowerCmd.Apply<IntangiblePower>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<IntangiblePower>().BaseValue, base.Owner.Creature, this);//无实体
            await PowerCmd.Apply<mp>(choiceContext,base.Owner.Creature, base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, this);//mp
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);//下回合能量
            await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext,base.Owner.Creature, base.DynamicVars.Cards.BaseValue, base.Owner.Creature, this);//下回合抽牌
        }
        public override string PortraitPath => $"res://images/cards/power/Reimu_flower.png";

        protected override void OnUpgrade()
        {
        }
        public static async Task<CardModel> CreateInHand(Player owner, ICombatState combatState)
        {
            return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
        }
        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState)
        {
            if (count == 0)
            {
                return Array.Empty<CardModel>();
            }
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return Array.Empty<CardModel>();
            }
            List<CardModel> ReimuFlower = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                ReimuFlower.Add(combatState.CreateCard<ReimuFlower>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(ReimuFlower, PileType.Hand, owner);
            return ReimuFlower;
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<ArtifactPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<IntangiblePower>(),
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromPower<EnergyNextTurnPower>(),
            HoverTipFactory.FromPower<Nextmp>()];
        //关键词
    }
}

