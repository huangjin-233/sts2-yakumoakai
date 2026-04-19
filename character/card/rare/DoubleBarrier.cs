using System;
using System.Collections.Generic;
using System.Linq;
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
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class DoubleBarrier : CardModel
    {
        public override bool GainsBlock => true;
        protected override List<DynamicVar> CanonicalVars => [
            new BlockVar(4m, ValueProp.Move)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Mpex];
        public DoubleBarrier()
            : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);//防御
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 40)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.Owner.Creature.Block, ValueProp.Unpowered | ValueProp.Move, cardPlay);
                await PowerCmd.Apply<mp>(base.Owner.Creature, -40m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 40;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 4;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 40;
            }
            //mp 效果
        }
        public override string PortraitPath => $"res://images/cards/skill/Double_barrier.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(3m);
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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DoubleBarrier));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

