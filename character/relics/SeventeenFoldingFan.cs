using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    public sealed class SeventeenFoldingFan : RelicModel
    {
        private bool _isActivating;

        private int _Played;
        public override RelicRarity Rarity => RelicRarity.Shop;
        // 稀有度
        public override bool ShowCounter => true;
        // 动态变量
        protected override List<DynamicVar> CanonicalVars => [
            new CardsVar(17)
        ];
        public override int DisplayAmount
        {
            get
            {
                if (!IsActivating)
                {
                    return Played;
                }
                return base.DynamicVars.Cards.IntValue;
            }
        }

        private bool IsActivating
        {
            get
            {
                return _isActivating;
            }
            set
            {
                AssertMutable();
                _isActivating = value;
                UpdateDisplay();
            }
        }

        [SavedProperty]
        public int Played
        {
            get
            {
                return _Played;
            }
            private set
            {
                AssertMutable();
                if (_Played != value)
                {
                    _Played = value;
                    UpdateDisplay();
                }
            }
        }

        private int Threshold => base.DynamicVars.Cards.IntValue;

        private void UpdateDisplay()
        {
            if (IsActivating)
            {
                base.Status = RelicStatus.Normal;
            }
            else
            {
                base.Status = ((Played == Threshold - 1) ? RelicStatus.Active : RelicStatus.Normal);
            }
            InvokeDisplayAmountChanged();
        }

        public void NotifySkillPlayed()
        {
            Played++;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == base.Owner)
            {
                Played++;
                if (Played >= Threshold)
                {
                    TaskHelper.RunSafely(DoActivateVisuals());
                    await PowerCmd.Apply<IntangiblePower>(base.Owner.Creature, 1m, base.Owner.Creature, null);//无实体
                    Played -= Threshold;
                }
            }
        }

        private async Task DoActivateVisuals()
        {
            IsActivating = true;
            Flash();
            await Cmd.Wait(1f);
            IsActivating = false;
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<IntangiblePower>(),
            HoverTipFactory.FromPower<mp>()];
        [ModInitializer(nameof(Initialize))]
        public static class MyCustomModInitializer
        {
            public static void Initialize()
            {

                ModHelper.AddModelToPool(typeof(YakumoAkaiRelicPool), typeof(SeventeenFoldingFan));

                var harmony = new Harmony("huangjin.yakumoakai");
                harmony.PatchAll();
                // 初始化 harmony 库
            }
        }
    }
}

