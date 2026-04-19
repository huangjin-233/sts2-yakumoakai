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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class Shackles : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Guai>(3),new PowerVar<StrengthPower>(2),new HpLossVar(3)
        ];
        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public Shackles()
            : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            await PowerCmd.Apply<Guai>(base.Owner.Creature, base.DynamicVars.Power<Guai>().BaseValue, base.Owner.Creature, this);
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 30)
            {
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);
                await PowerCmd.Apply<mp>(base.Owner.Creature, -30m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 30;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 30;
            }
        }
        public override string PortraitPath => $"res://images/cards/power/Shackles.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.HpLoss.UpgradeValueBy(-1);
            base.DynamicVars.Power<Guai>().UpgradeValueBy(1);// 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Guai>(),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<mp>()
        ];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Shackles));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

