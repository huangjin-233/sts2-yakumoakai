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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class YukaAngry : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(50m, ValueProp.Move),new PowerVar<mp>(80)
        ];
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Mpex];
        public YukaAngry()
            : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllAllies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {            
                if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= base.DynamicVars.Power<mp>().BaseValue)
                {
                    await PowerCmd.Apply<IntangiblePower>(base.Owner.Creature, 1, base.Owner.Creature, this);//无实体
                    await PowerCmd.Apply<mp>(base.Owner.Creature, -base.DynamicVars.Power<mp>().BaseValue, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + (int)base.DynamicVars.Power<mp>().BaseValue;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + (int)base.DynamicVars.Power<mp>().BaseValue/10;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + (int)base.DynamicVars.Power<mp>().BaseValue;
            }
            Creature creature = base.Owner.Creature;
            DamageVar damage = base.DynamicVars.Damage;
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature.CombatState.Creatures, damage.BaseValue, damage.Props, creature, null);
            }
        public override string PortraitPath => $"res://images/cards/attack/Yuka_angry.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<mp>().UpgradeValueBy(-20);
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
             HoverTipFactory.FromPower<IntangiblePower>(),
             HoverTipFactory.FromPower<mp>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(YukaAngry));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

