using System;
using System.Collections.Generic;
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

namespace YakumoAkai.character.card.uncommon
{
    public sealed class MeiLingHat : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<StrengthPower>(3), new PowerVar<DexterityPower>(3),new PowerVar<BufferPower>(1)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [ CardKeyword.Ethereal];
        public MeiLingHat()
            : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars.Power<DexterityPower>().BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<BufferPower>(base.Owner.Creature, base.DynamicVars.Power<BufferPower>().BaseValue, base.Owner.Creature, this);
            PlayerCmd.EndTurn(base.Owner, canBackOut: false);
        }
        public override string PortraitPath => $"res://images/cards/power/MeiLingHat.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Eternal);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<BufferPower>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(MeiLingHat));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
