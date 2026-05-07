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
    public sealed class ClearMirror : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Clearmirrorpower>(1)
        ];
        // 动态变量
        public ClearMirror()
            : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<Clearmirrorpower>(base.Owner.Creature, base.DynamicVars.Power<Clearmirrorpower>().BaseValue, base.Owner.Creature, this);
        }
        public override string PortraitPath => $"res://images/cards/power/ClearMirror.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            HoverTipFactory.FromPower<Clearmirrorpower>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(ClearMirror));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
