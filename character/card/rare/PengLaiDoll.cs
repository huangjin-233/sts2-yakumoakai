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
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class PengLaiDoll : CardModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new PowerVar<Fire>(5)];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public override CardPoolModel VisualCardPool => ModelDb.CardPool<YakumoAkaiCardPool>();
        public PengLaiDoll()
            : base(5, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
           await PowerCmd.Apply<Restart>(base.Owner.Creature, 1, base.Owner.Creature, this);//复活
        }
        public override string PortraitPath => $"res://images/cards/power/PengLaiDoll.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Restart>()
            ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(PengLaiDoll));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}
