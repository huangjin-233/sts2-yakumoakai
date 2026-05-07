using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    public sealed class StarlightAndGhostlySteps : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [new CardsVar(3), new PowerVar<DexterityPower>(1)];// 动态变量

        public override List<CardKeyword> CanonicalKeywords => [AkaiKeyword.Mpex];
        public StarlightAndGhostlySteps()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);//抽卡
            foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, base.Owner,new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1,10),null,this))
                //选择手牌
            await CardCmd.Discard(choiceContext, item);//丢弃1张
            if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 15)
            {
                await PowerCmd.Apply<Stardex>(base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);//临时敏捷
                await PowerCmd.Apply<mp>(base.Owner.Creature, -15m, base.Owner.Creature, this);
                Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 15;
                IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 3;
                Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 15;
                DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 15;
            }
            //mp 效果
        }
        public override string PortraitPath => $"res://images/cards/skill/Starlight_and_ghostly_steps.png";

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
            // 升级后
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<mp>(),
            HoverTipFactory.FromPower<DexterityPower>()];
        //关键词
        [ModInitializer(nameof(Initialize))]
        public static class YakumoakaiInitializer
        {
            public static void Initialize()
            {
                {
                    ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(StarlightAndGhostlySteps));

                    var harmony = new Harmony("huangjin.yakumoakai");
                    harmony.PatchAll();
                    // 初始化 harmony 库
                }
            }
        }
    }
}

