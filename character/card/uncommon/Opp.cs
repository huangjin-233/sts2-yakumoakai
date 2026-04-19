using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class Opp : CardModel
    {
        // 动态变量
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public Opp()
            : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target.HasPower<WeakPower>())
            {
                await PowerCmd.Apply<WeakPower>(cardPlay.Target,cardPlay.Target.GetPowerAmount<WeakPower>(), base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<VulnerablePower>())
            {
                await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, cardPlay.Target.GetPowerAmount<VulnerablePower>(), base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<PoisonPower>())
            {
                await PowerCmd.Apply<PoisonPower>(cardPlay.Target, cardPlay.Target.GetPowerAmount<PoisonPower>(), base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<Fire>())
            {
                await PowerCmd.Apply<Fire>(cardPlay.Target, cardPlay.Target.GetPowerAmount<Fire>(), base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<DoomPower>())
            {
                await PowerCmd.Apply<DoomPower>(cardPlay.Target, cardPlay.Target.GetPowerAmount<DoomPower>(), base.Owner.Creature, this);
            }
            if (cardPlay.Target.HasPower<StrengthPower>() && cardPlay.Target.GetPowerAmount<StrengthPower>() < 0)
            {
                await PowerCmd.Apply<StrengthPower>(cardPlay.Target, cardPlay.Target.GetPowerAmount<StrengthPower>(), base.Owner.Creature, this);
            }
        }
        public override string PortraitPath => $"res://images/cards/skill/Opp.png";

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(Opp));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

