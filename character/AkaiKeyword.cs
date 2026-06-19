using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace YakumoAkai.character
{
    [RegisterOwnedCardKeyword(nameof(Medice), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
    [RegisterOwnedCardKeyword(nameof(Mpex))]
    public class AkaiKeyword
    {
        public static readonly CardKeyword Medice = ModContentRegistry.GetQualifiedKeywordId(MyCustomModInitializer.ModId, nameof(Medice)).GetModCardKeyword();
        public static readonly CardKeyword Mpex = ModContentRegistry.GetQualifiedKeywordId(MyCustomModInitializer.ModId, nameof(Mpex)).GetModCardKeyword();
    }
}

