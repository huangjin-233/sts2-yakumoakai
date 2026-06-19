using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BaseLibToRitsu.Generated;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.card.uncommon
{
    [RegisterCard(typeof(YakumoAkaiCardPool

))]
    public sealed class WindFan : ModCardTemplate
    {
        protected override List<DynamicVar> CanonicalVars => [
            new PowerVar<Fire>(6)
        ];
        // 动态变量
        public WindFan()
            : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            if (cardPlay.Target.HasPower<Fire>() && cardPlay.Target.GetPowerAmount<Fire>() >= 0)
            {
                Color color = new Color("FFFFFF80");
                double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + (double)Mathf.Min(8, 1) * num2));
                NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
                await PowerCmd.Apply<Fire>(choiceContext,cardPlay.Target, base.DynamicVars.Power<Fire>().BaseValue, base.Owner.Creature, this);//燃烧
            }
        }
        public override string PortraitPath => $"res://images/cards/attack/WindFan.png";

        protected override void OnUpgrade()
        {
            base.DynamicVars.Power<Fire>().UpgradeValueBy(2);
        }
        protected override IEnumerable<IHoverTip> AdditionalHoverTips  => [
            HoverTipFactory.FromPower<Fire>()
            ];
        //关键词
    }
}
