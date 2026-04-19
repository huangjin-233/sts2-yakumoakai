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
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    public sealed class ReimuFlower : CardModel
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
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, AkaiKeyword.Medice];
        public ReimuFlower()
        : base(0, CardType.Power, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ArtifactPower>(base.Owner.Creature, base.DynamicVars.Power<ArtifactPower>().BaseValue, base.Owner.Creature, this);//人工制品
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);//力量
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars.Power<DexterityPower>().BaseValue, base.Owner.Creature, this);//敏捷
            await PowerCmd.Apply<ThornsPower>(base.Owner.Creature, base.DynamicVars.Power<ThornsPower>().BaseValue, base.Owner.Creature, this);//荆棘
            await PowerCmd.Apply<PlatingPower>(base.Owner.Creature, base.DynamicVars.Power<PlatingPower>().BaseValue, base.Owner.Creature, this);//覆甲
            await PowerCmd.Apply<IntangiblePower>(base.Owner.Creature, base.DynamicVars.Power<IntangiblePower>().BaseValue, base.Owner.Creature, this);//无实体
            await PowerCmd.Apply<mp>(base.Owner.Creature, base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, this);//mp
            await PowerCmd.Apply<EnergyNextTurnPower>(base.Owner.Creature, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);//下回合能量
            await PowerCmd.Apply<DrawCardsNextTurnPower>(base.Owner.Creature, base.DynamicVars.Cards.BaseValue, base.Owner.Creature, this);//下回合抽牌
        }
        public override string PortraitPath => $"res://images/cards/power/Reimu_flower.png";

        protected override void OnUpgrade()
        {
        }
        public static async Task<CardModel> CreateInHand(Player owner, CombatState combatState)
        {
            return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
        }
        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, CombatState combatState)
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
            await CardPileCmd.AddGeneratedCardsToCombat(ReimuFlower, PileType.Hand, addedByPlayer: true);
            return ReimuFlower;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
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
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(ReimuFlower));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

