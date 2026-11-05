using System.Reflection;
using System.Text.Json.Nodes;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Models;
using YakumoAkai.character.card.rare;
using YakumoAkai.character.power;

namespace YakumoAkai.character.single;

public class MpSingle : HookedSingletonModel
{
    // 本模组程序集，用于筛出属于 YakumoAkai 的卡牌
    private static readonly Assembly ModAssembly = typeof(YakumoAkaiTelemetrySingle).Assembly;

    public MpSingle() : base(HookType.Combat)
    {
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is mp && amount < 0 && applier?.Player != null)
        {
            Kind.mp[applier.Player] = Kind.GetValue(applier.Player) - amount;
            IronWheel.card[applier.Player] = IronWheel.GetValue(applier.Player) - amount/5;
            DivineGodIncantationPower.god[applier.Player] = DivineGodIncantationPower.GetValue(applier.Player) - amount;
            Maidknifepower.maid[applier.Player] = Maidknifepower.GetValue(applier.Player) - amount;
        } //mp 效果
    }
}