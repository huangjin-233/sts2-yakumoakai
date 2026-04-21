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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.common
{
    public sealed class AkaiAndYukari_0 : CardModel
    {

        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(8m,ValueProp.Move)
        ];
        // 动态变量
        public AkaiAndYukari_0()
            : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) {}
        // 卡牌的构造函数，指定卡牌的相关属性
        public bool benzi = false;
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
            await CardCmd.Exhaust(choiceContext, this);//消耗
            await AkaiAndYukari_1.CreateInHand(base.Owner, 1, base.CombatState);//添加手牌
        }
        public override string PortraitPath => $"res://images/cards/attack/AkaiAndYukari.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(2);
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
            List<CardModel> AkaiAndYukari_0 = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                AkaiAndYukari_0.Add(combatState.CreateCard<AkaiAndYukari_0>(owner));
            }
            await CardPileCmd.AddGeneratedCardsToCombat(AkaiAndYukari_0, PileType.Hand, addedByPlayer: true);
            return AkaiAndYukari_0;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromCard<AkaiAndYukari_1>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(AkaiAndYukari_0));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
