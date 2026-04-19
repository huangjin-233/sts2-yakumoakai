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

namespace YakumoAkai.character.card.rare
{
    public sealed class BloodBat : CardModel
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(3m, ValueProp.Move),new PowerVar<BufferPower>(1)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,AkaiKeyword.Mpex];
        public BloodBat()
            : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
            await PowerCmd.Apply<BufferPower>(base.Owner.Creature, base.DynamicVars.Power<BufferPower>().BaseValue, base.Owner.Creature, this);
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 25)
            {
                await PowerCmd.Apply<blood>(base.Owner.Creature, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<mp>(base.Owner.Creature, -10m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 25;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 2;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 25;
            }
        }
        public override string PortraitPath => $"res://images/cards/power/Blood_bat.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(3m);
            base.DynamicVars.Power<BufferPower>().UpgradeValueBy(1);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromPower<BufferPower>(),
            HoverTipFactory.FromPower<blood>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(BloodBat));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

