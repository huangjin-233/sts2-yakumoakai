using System.IO;
using BaseLibToRitsu.Generated;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Content;
using STS2RitsuLib.Keywords;

namespace YakumoAkai.character
{
    public static class AkaiKeyword
    {
        // 自定义枚举的名字。最终会变成{前缀}-{枚举值大写}的形式，例如TEST-UNIQUE
        [CustomEnum("MEDICE")]
        // 放在原版卡牌描述的位置，这里是卡牌描述的前面
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword Medice;
        public static CardKeyword Mpex = (CardKeyword)8178;
    }
}

