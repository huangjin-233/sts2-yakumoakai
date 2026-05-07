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
using YakumoAkai.character.card.rare;
using YakumoAkai.character.card.special;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class MaidKnife : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Maidknifepower>(2)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public MaidKnife()
            : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            Maidknifepower.maid[base.Owner] = 0;
            await PowerCmd.Apply<Maidknifepower>(base.Owner.Creature, base.DynamicVars.Power<Maidknifepower>().BaseValue, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/power/MaidKnife.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<Maidknifepower>().UpgradeValueBy(1);//升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(MaidKnife));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
