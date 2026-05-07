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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.special
{
    public sealed class Medice5 : CardModel
    {
        public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,AkaiKeyword.Medice];
        protected override List<DynamicVar> CanonicalVars => [new EnergyVar(2)];// 动态变量
        public Medice5()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<mp>(base.Owner.Creature, 50, base.Owner.Creature, this);//mp
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);//能量
            await PowerCmd.Apply<Nextmp>(base.Owner.Creature, 30, base.Owner.Creature, this);//下回合mp
            await PowerCmd.Apply<EnergyNextTurnPower>(base.Owner.Creature, 1, base.Owner.Creature, this);//下回合能量
        }
        public override string PortraitPath => $"res://images/cards/skill/Medice5.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
                HoverTipFactory.FromPower<mp>(),
                HoverTipFactory.FromPower<Nextmp>(),
                HoverTipFactory.FromPower<EnergyNextTurnPower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoakaiTokenCardPool), typeof(Medice5));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

