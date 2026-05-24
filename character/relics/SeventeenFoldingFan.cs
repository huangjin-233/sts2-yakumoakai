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
using STS2RitsuLib.Interop.AutoRegistration;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.relics
{
    [RegisterRelic(typeof(YakumoAkaiRelicPool))]
    public sealed class SeventeenFoldingFan : RelicModel
    {
        // 小图标（原版85x85）
        public override string PackedIconPath => $"res://images/relic/seventeen_folding_fan.png";
        // 轮廓图标（原版85x85）
        protected override string PackedIconOutlinePath => $"res://images/relic/outline/seventeen_folding_fan.png";
        // 大图标（原版256x256）
        protected override string BigIconPath => $"res://images/relic/large/seventeen_folding_fan.png";
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
                    if (base.Owner.Creature.HasPower<mp>() && base.Owner.Creature.GetPowerAmount<mp>() >= 20)
                    {
                        await PowerCmd.Apply<mp>(context,base.Owner.Creature, -20m, base.Owner.Creature, null);
                        Kind.mp[base.Owner] = Kind.GetValue(base.Owner) + 20;
                        IronWheel.card[base.Owner] = IronWheel.GetValue(base.Owner) + 4;
                        Maidknifepower.maid[base.Owner] = Maidknifepower.GetValue(base.Owner) + 20;
                        DivineGodIncantationPower.god[base.Owner] = DivineGodIncantationPower.GetValue(base.Owner) + 20;
                        TaskHelper.RunSafely(DoActivateVisuals());
                        await PowerCmd.Apply<IntangiblePower>(context,base.Owner.Creature, 1m, base.Owner.Creature, null);//无实体                                                                                          
                        Played -= Threshold;
                    }
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
    }
}

