using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;
using static System.Collections.Specialized.BitVector32;

namespace YakumoAkai.character.card.rare
{
    internal class Laevatain : CardModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
        // 动态变量
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal,AkaiKeyword.Mpex];
        
        protected override bool IsPlayable => base.Owner.Creature.GetPowerAmount<mp>() >= 45
            //打出条件
            ;
        public Laevatain()
            : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardModel cardModel = (
                await CardSelectCmd.FromHand(
                    choiceContext,
                    base.Owner,
                    new CardSelectorPrefs(new LocString("gameplay_ui", ""), 1),
                    null,
                    this)
                ).FirstOrDefault();
            //选择手牌
            if (cardModel != null)
            {
                for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
                {
                    CardModel card = cardModel.CreateClone();
                    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
                    //复制3张
                    card.EnergyCost.SetThisTurnOrUntilPlayed(0);
                    //能耗变为0
                }
            }
            await PowerCmd.Apply<mp>(base.Owner.Creature, -45m, base.Owner.Creature, this);
            Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 45;
            IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 9;
            Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 45;
            DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 45;
        }
        public override string PortraitPath => $"res://images/cards/skill/Laevatain.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Ethereal);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Laevatain));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}