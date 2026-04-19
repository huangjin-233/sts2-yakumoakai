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
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class DeadAndLife : CardModel
    {
        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
        protected override List<DynamicVar> CanonicalVars => [
            new HealVar(8) // 伤害值
        ];
        private static int h;
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Mpex];
        public DeadAndLife()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<mp>())
            {
                h = base.Owner.Creature.GetPowerAmount<mp>();
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + base.Owner.Creature.GetPowerAmount<mp>();
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + base.Owner.Creature.GetPowerAmount<mp>()/10;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + base.Owner.Creature.GetPowerAmount<mp>();
                await PowerCmd.Remove<mp>(base.Owner.Creature);
                await CreatureCmd.Heal(cardPlay.Target, h/15);//回血
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/DeadAndLife.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
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
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DeadAndLife));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

