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

namespace YakumoAkai.character.card.common
{
    public sealed class TsubameGaeshi : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
            new DamageVar(5m, ValueProp.Move) // 伤害值
        ];
        // 动态变量

        public TsubameGaeshi()
               : base(2, CardType.Attack, CardRarity.Common, TargetType.AllAllies) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            for (int i = 0; i < 3; i++)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .Execute(choiceContext); // 执行攻击效果
                //群体攻击
            }
            //多段伤害
        }
    public override string PortraitPath => $"res://images/cards/attack/Tsubame_gaeshi.png";

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m); // 升级后
    }
    [ModInitializer(nameof(Initialize))]
    public static class YakumoakaiInitializer
    {
        public static void Initialize()
        {
            {
                ModHelper.AddModelToPool(typeof(YakumoAkaiCardPool), typeof(TsubameGaeshi));

                var harmony = new Harmony("huangjin.yakumoakai");
                harmony.PatchAll();
                // 初始化 harmony 库
            }
        }
    }
}
}

