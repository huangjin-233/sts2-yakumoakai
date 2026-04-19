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
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class DivineGodIncantation : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<DivineGodIncantationPower>(4) //能力
        ];
        // 动态变量
        public DivineGodIncantation()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            DivineGodIncantationPower.god[base.Owner] = 0;
            await PowerCmd.Apply<DivineGodIncantationPower>(base.Owner.Creature, base.DynamicVars.Power<DivineGodIncantationPower>().BaseValue, base.Owner.Creature, this);//mp
        }
        public override string PortraitPath => $"res://images/cards/skill/DivineGodIncantation.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<DivineGodIncantationPower>().UpgradeValueBy(2);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.Static(StaticHoverTip.Block),
            HoverTipFactory.FromPower<mp>()];
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DivineGodIncantation));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

