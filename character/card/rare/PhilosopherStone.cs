using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class PhilosopherStone : CardModel
    {
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public PhilosopherStone()
            : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
            {
                if (!card.EnergyCost.CostsX)
                {
                    card.SetToFreeThisTurn();
                }
            }
            await PowerCmd.Apply<mp>(base.Owner.Creature, 200, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/skill/Philosopher_stone.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            // 升级后
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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(PhilosopherStone));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

