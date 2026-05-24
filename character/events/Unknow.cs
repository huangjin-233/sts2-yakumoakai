using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using YakumoAkai.character.relics;

namespace YakumoAkai.character.events
{
    [RegisterSharedEvent] // 如果需要自定义生成条件，可以注册成通用再重载isAllowed
    public sealed class Unknow : ModEventTemplate
    {
        // 背景图位置
        public override EventAssetProfile AssetProfile => new(
            InitialPortraitPath: "res://images/events/Unknow.png"
        );

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new HpLossVar(20m)
        ];

        // 什么时候会遇到。这里的条件是所有玩家的金币都大于等于60

        // 事件开始前的逻辑。这里是禁止玩家移除药水
        protected override Task BeforeEventStarted(bool isPreFinished)
        {
            Owner!.CanRemovePotions = false;
            return Task.CompletedTask;
        }

        // 事件结束后的逻辑。这里是允许玩家移除药水
        protected override void OnEventFinished()
        {
            Owner!.CanRemovePotions = true;
        }

        // 生成事件初始选项。这里是两个选项：失去生命值或者失去金币，然后进入选择奖励阶段
        // 与 CustomEventModel.Option(delegate, pageKey) 一致：textKey = Id.Entry + ".pages." + page + ".options." + Slugify(方法名)
        protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            new EventOption(this, Drink, InitialOptionKey("Drink")),
            new EventOption(this, Nodrink, InitialOptionKey("Nodrink")),
    ];

        // 失去生命
        private async Task Drink()
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue, isFromCard: false);
            Lins();
        }

        // 失去金币
        private async Task Nodrink()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.End2.description"));
        }

        // 进入事件第二阶段，两个选项：选择药水或者选择卡牌
        private void Lins()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.Drink.description"), [
                new EventOption(this, Relic, ModOptionKey("DRINK", "Lins"),HoverTipFactory.FromRelic<Lin>()).ThatDecreasesMaxHp(base.DynamicVars.HpLoss.BaseValue)
        ]);
        }

        // 选择药水奖励，然后结束事件
        private async Task Relic()
        {
            await RelicCmd.Obtain(ModelDb.Relic<Lin>().ToMutable(), base.Owner);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.End1.description"));
        }
    }
}
