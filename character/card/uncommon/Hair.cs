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
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    namespace YakumoAkai.character.card.uncommon
    {
        public sealed class Hair : CardModel
        {
            protected override List<DynamicVar> CanonicalVars => [
                new BlockVar(4m, ValueProp.Move)
            ];
            // 动态变量
            public static int str;
            public static int dex;
            public Hair()
                : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
            // 卡牌的构造函数，指定卡牌的相关属性

            protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
            {
                    str = base.Owner.Creature.GetPowerAmount<StrengthPower>();
                    dex = base.Owner.Creature.GetPowerAmount<DexterityPower>();
                    await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, dex, base.Owner.Creature, this);
                    await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, str, base.Owner.Creature, this);
                    await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, -dex, base.Owner.Creature, this);
                    await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, -str, base.Owner.Creature, this);
            }
            public override string PortraitPath => $"res://images/cards/skill/Hair.png";

            protected override void OnUpgrade()
            {
                base.EnergyCost.UpgradeBy(-1);
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
                        ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Hair));

                        var harmony = new Harmony("huangjin.yakumoakai");
                        harmony.PatchAll();
                        // 初始化 harmony 库
                    }
                }
            }
        }
    }
}

