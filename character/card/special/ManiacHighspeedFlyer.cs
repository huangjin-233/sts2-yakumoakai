using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace YakumoAkai.character.card.special
{
    internal class ManiacHighspeedFlyer : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(5m, ValueProp.Move) // 伤害值
        ];        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


        public ManiacHighspeedFlyer()
            : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
        }
        public override string PortraitPath => $"res://images/cards/attack/Maniac_highspeed_flyer.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m); // 升级后
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
            List<CardModel> ManiacHighspeedFlyer = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                ManiacHighspeedFlyer.Add(combatState.CreateCard<ManiacHighspeedFlyer>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(ManiacHighspeedFlyer, PileType.Hand, addedByPlayer: true);
            return ManiacHighspeedFlyer;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<ManiacHighspeedFlyer>()];
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoakaiTokenCardPool), typeof(ManiacHighspeedFlyer));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

