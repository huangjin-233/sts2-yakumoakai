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
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.common;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    public sealed class AkaiAndYukari_1 : CardModel
    {

        protected override List<DynamicVar> CanonicalVars => [
          new DamageVar(9m,ValueProp.Move)
        ];
        // 动态变量
        public AkaiAndYukari_1()
            : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public bool benzi = false;
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
             .Execute(choiceContext); // 执行攻击效果
            await CardCmd.Exhaust(choiceContext, this);//消耗
            await AkaiAndYukari_0.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌
        }
        public override string PortraitPath => $"res://images/cards/attack/AkaiAndYukari.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(3);
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
            List<CardModel> AkaiAndYukari_1 = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                AkaiAndYukari_1.Add(combatState.CreateCard<AkaiAndYukari_1>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(AkaiAndYukari_1, PileType.Hand, addedByPlayer: true);
            return AkaiAndYukari_1;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<AkaiAndYukari_0>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoakaiTokenCardPool), typeof(AkaiAndYukari_1));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
