using System;
using System.Collections.Generic;
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
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.card.special;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class Mouth : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new CardsVar(2)
        ];
        // 动态变量
        public Mouth()
            : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target.HasPower<DexterityPower>() && cardPlay.Target.GetPowerAmount<DexterityPower>() > 0)
            {
                await PowerCmd.Apply<DexterityPower>(cardPlay.Target, -cardPlay.Target.GetPowerAmount<DexterityPower>()*2, base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<StrengthPower>() && cardPlay.Target.GetPowerAmount<StrengthPower>() > 0)
            {
                await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -cardPlay.Target.GetPowerAmount<StrengthPower>()*2, base.Owner.Creature, this);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Mouth.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Mouth));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
