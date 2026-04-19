using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.rare
{
    public sealed class DestroyRoar : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<VulnerablePower>(2),new PowerVar<StrengthPower>(4),
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Mpex];
        public DestroyRoar()
            : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<StrengthPower>(base.CombatState.HittableEnemies, -base.DynamicVars.Power<StrengthPower>().BaseValue, base.Owner.Creature, this);//力量
            foreach(Creature Enemy in base.CombatState.HittableEnemies)
            { 
                await CreatureCmd.Stun(Enemy);
            }
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                await PowerCmd.Apply<VulnerablePower>(base.CombatState.HittableEnemies, base.DynamicVars.Power<VulnerablePower>().BaseValue, base.Owner.Creature, this);//易伤
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 1;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/DestroyRoar.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<VulnerablePower>().UpgradeValueBy(1);
            base.DynamicVars.Power<StrengthPower>().UpgradeValueBy(2);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.Static(StaticHoverTip.Stun)];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(DestroyRoar));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

