#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace BaseLibToRitsu.Generated;

public interface ICustomModel
{
}

public interface ITranscendenceCard
{
    CardModel GetTranscendenceTransformedCard();
}

public interface ISceneConversions
{
    void RegisterSceneConversions();
}

public interface ILocalizationProvider
{
    string? LocTable => null;

    List<(string, string)>? Localization { get; }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomEnumAttribute(string? name = null) : Attribute
{
    public string? Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class KeywordPropertiesAttribute : Attribute
{
    public KeywordPropertiesAttribute(AutoKeywordPosition position) : this(position, true)
    {
    }

    public KeywordPropertiesAttribute(AutoKeywordPosition position, bool richKeyword)
    {
        Position = position;
        RichKeyword = richKeyword;
    }

    public AutoKeywordPosition Position { get; }

    public bool RichKeyword { get; }
}

public enum AutoKeywordPosition
{
    None,
    Before,
    After
}

public static class CustomKeywords
{
    private static readonly Dictionary<int, KeywordInfo> KeywordIds = [];

    public readonly struct KeywordInfo(string canonicalKey, string? fallbackKey = null)
    {
        public string CanonicalKey { get; } = canonicalKey;

        public string? FallbackKey { get; } = fallbackKey;

        public required AutoKeywordPosition AutoPosition { get; init; }

        public required bool RichKeyword { get; init; }

        public string ResolveLocKeyPrefix()
        {
            if (!string.IsNullOrWhiteSpace(FallbackKey) &&
                LocString.GetIfExists("card_keywords", FallbackKey + ".title") != null)
            {
                return FallbackKey;
            }

            return CanonicalKey;
        }
    }

    public static void Register(CardKeyword keyword, string canonicalKey, string? fallbackKey, AutoKeywordPosition autoPosition, bool richKeyword)
    {
        KeywordIds[(int)keyword] = new KeywordInfo(canonicalKey, fallbackKey)
        {
            AutoPosition = autoPosition,
            RichKeyword = richKeyword
        };
    }

    public static bool TryGet(CardKeyword keyword, out KeywordInfo info)
    {
        return KeywordIds.TryGetValue((int)keyword, out info);
    }
}

public static class CustomEnums
{
    private static readonly HashAlgorithm Md5 = MD5.Create();
    private static readonly Dictionary<string, int> HashCache = [];
    private static readonly HashSet<int> ExistingHashes = [];
    private static readonly Dictionary<Type, KeyGenerator> KeyGenerators = [];

    public static object GenerateKey(FieldInfo field, string modId)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentException.ThrowIfNullOrWhiteSpace(modId);

        return GenerateKey(
            field.FieldType,
            GetTypeRoot(field.DeclaringType, modId),
            field.Name);
    }

    private static object GenerateKey(Type enumType, string namespaceStem, string name)
    {
        if (!KeyGenerators.TryGetValue(enumType, out var generator))
        {
            generator = new KeyGenerator(enumType);
            KeyGenerators[enumType] = generator;
        }

        return generator.GetKey(ComputeBasicHash(namespaceStem), ComputeBasicHash(name));
    }

    private static string GetTypeRoot(Type? type, string modId)
    {
        if (type?.Namespace is not string ns || string.IsNullOrWhiteSpace(ns))
        {
            return modId;
        }

        var dotIndex = ns.IndexOf('.');
        return dotIndex < 0 ? ns : ns[..dotIndex];
    }

    private static int ComputeBasicHash(string value)
    {
        if (!HashCache.TryGetValue(value, out var hash))
        {
            var data = Md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            unchecked
            {
                const int multiplier = 16777619;
                hash = (int)2166136261;
                foreach (var current in data)
                {
                    hash = (hash ^ current) * multiplier;
                }
            }

            HashCache[value] = hash;
            if (!ExistingHashes.Add(hash))
            {
                foreach (var entry in HashCache)
                {
                    if (!string.Equals(entry.Key, value, StringComparison.Ordinal) && entry.Value == hash)
                    {
                        LegacyCompatibilityBootstrap.Logger.Warn(
                            $"Duplicate custom enum hash detected for '{entry.Key}' and '{value}': {hash}");
                    }
                }
            }
        }

        return hash;
    }

    private sealed class KeyGenerator
    {
        private static readonly Dictionary<Type, Func<object, object>> Incrementers = new()
        {
            { typeof(byte), value => (byte)value + 1 },
            { typeof(sbyte), value => (sbyte)value + 1 },
            { typeof(short), value => (short)value + 1 },
            { typeof(ushort), value => (ushort)value + 1 },
            { typeof(int), value => (int)value + 1 },
            { typeof(uint), value => (uint)value + 1 },
            { typeof(long), value => (long)value + 1L },
            { typeof(ulong), value => (ulong)value + 1UL }
        };

        private static readonly Dictionary<Type, Func<object, object>> FlagIncrementers = new()
        {
            { typeof(byte), CreateFlagIncrementer<byte>() },
            { typeof(sbyte), CreateFlagIncrementer<sbyte>() },
            { typeof(short), CreateFlagIncrementer<short>() },
            { typeof(ushort), CreateFlagIncrementer<ushort>() },
            { typeof(int), CreateFlagIncrementer<int>() },
            { typeof(uint), CreateFlagIncrementer<uint>() },
            { typeof(long), CreateFlagIncrementer<long>() },
            { typeof(ulong), CreateFlagIncrementer<ulong>() }
        };

        private static readonly Dictionary<Type, int> TypeHalfSizes = new()
        {
            { typeof(byte), sizeof(byte) * 4 },
            { typeof(sbyte), sizeof(sbyte) * 4 },
            { typeof(short), sizeof(short) * 4 },
            { typeof(ushort), sizeof(ushort) * 4 },
            { typeof(int), sizeof(int) * 4 },
            { typeof(uint), sizeof(uint) * 4 },
            { typeof(long), sizeof(long) * 4 },
            { typeof(ulong), sizeof(ulong) * 4 }
        };

        private readonly int _halfBits;
        private readonly Func<object, object> _increment;
        private readonly bool _isFlag;
        private readonly Type _underlyingType;
        private readonly HashSet<object> _values = [];
        private object _nextKey;

        public KeyGenerator(Type enumType)
        {
            ArgumentNullException.ThrowIfNull(enumType);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Custom enum key generation requires an enum type.", nameof(enumType));
            }

            _isFlag = enumType.GetCustomAttribute<FlagsAttribute>() != null;
            _underlyingType = Enum.GetUnderlyingType(enumType);
            _nextKey = Convert.ChangeType(0, _underlyingType);
            _increment = _isFlag ? FlagIncrementers[_underlyingType] : Incrementers[_underlyingType];
            _halfBits = TypeHalfSizes[_underlyingType];

            foreach (var value in enumType.GetEnumValuesAsUnderlyingType())
            {
                _values.Add(value);
                if (((IComparable)value).CompareTo(_nextKey) >= 0)
                {
                    _nextKey = _increment(value);
                }
            }
        }

        public object GetKey(int namespaceHash, int nameHash)
        {
            if (_isFlag)
            {
                var current = _nextKey;
                _nextKey = _increment(_nextKey);
                return current;
            }

            var mask = (1 << _halfBits) - 1;
            var upper = namespaceHash & mask;
            var lower = nameHash & mask;
            var result = (upper << _halfBits) | lower;

            _nextKey = Convert.ChangeType(result, _underlyingType);
            while (_values.Contains(_nextKey))
            {
                _nextKey = _increment(_nextKey);
            }

            _values.Add(_nextKey);
            return _nextKey;
        }

        private static Func<object, object> CreateFlagIncrementer<T>() where T : struct, IBinaryInteger<T>
        {
            return value =>
            {
                var current = (T)value;
                var result = T.One;
                while (result <= current && result != T.Zero)
                {
                    result <<= 1;
                }

                return result;
            };
        }
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public sealed class ConfigSectionAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public sealed class ConfigHoverTipAttribute(bool enabled = true) : Attribute
{
    public bool Enabled { get; } = enabled;
}

[AttributeUsage(AttributeTargets.Class)]
public class ConfigHoverTipsByDefaultAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
[Obsolete("Use [ConfigHoverTipsByDefault] instead.")]
public sealed class HoverTipsByDefaultAttribute : ConfigHoverTipsByDefaultAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigIgnoreAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigHideInUI : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigColorPickerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ConfigButtonAttribute(string buttonLabelKey) : Attribute
{
    public string ButtonLabelKey { get; } = buttonLabelKey;

    public string Color { get; set; } = "#4a7f5a";
}

public static class ModConfigRegistry
{
    private static readonly Dictionary<string, ModConfig> Configs = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(string modId, ModConfig config)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modId);
        ArgumentNullException.ThrowIfNull(config);

        config.AttachModId(modId);
        Configs[modId] = config;
    }

    public static ModConfig? Get(string? modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
        {
            return null;
        }

        return Configs.TryGetValue(modId, out var config) ? config : null;
    }

    public static T? Get<T>() where T : ModConfig
    {
        return Configs.Values.OfType<T>().FirstOrDefault();
    }

    public static IReadOnlyList<ModConfig> GetAll()
    {
        return Configs.Values.OrderBy(static config => config.ModId, StringComparer.OrdinalIgnoreCase).ToArray();
    }
}

public abstract class ModConfig
{
    private readonly Dictionary<string, object?> _defaultValues = new(StringComparer.Ordinal);
    private readonly PropertyInfo[] _configProperties;
    private readonly string _configPath;

    protected ModConfig(string? filename = null)
    {
        _configProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(static property =>
                property.CanRead &&
                property.CanWrite &&
                property.GetCustomAttribute<ConfigIgnoreAttribute>() == null)
            .ToArray();

        foreach (var property in _configProperties)
        {
            _defaultValues[property.Name] = property.GetValue(null);
        }

        var configName = string.IsNullOrWhiteSpace(filename)
            ? (GetType().Namespace?.Split('.')[0] ?? GetType().Assembly.GetName().Name ?? GetType().Name)
            : filename;
        _configPath = Path.Combine(OS.GetUserDataDir(), "mod_configs", SanitizeFileName(configName) + ".json");
        Load();
    }

    public event EventHandler? ConfigChanged;

    public event Action? OnConfigReloaded;

    public string? ModId { get; private set; }

    public bool HasSettings()
    {
        return _configProperties.Length > 0;
    }

    public bool HasVisibleSettings()
    {
        return _configProperties.Any(static property => property.GetCustomAttribute<ConfigHideInUI>() == null);
    }

    internal void AttachModId(string modId)
    {
        ModId = modId;
    }

    public virtual void SetupConfigUI(Control optionContainer)
    {
    }

    public void Changed()
    {
        Save();
        ConfigChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ConfigReloaded()
    {
        OnConfigReloaded?.Invoke();
    }

    public void RestoreDefaultsNoConfirm()
    {
        foreach (var property in _configProperties)
        {
            if (_defaultValues.TryGetValue(property.Name, out var value))
            {
                property.SetValue(null, value);
            }
        }

        Save();
        ConfigReloaded();
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                Save();
                return;
            }

            using var stream = File.OpenRead(_configPath);
            using var document = JsonDocument.Parse(stream);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            foreach (var property in _configProperties)
            {
                if (!document.RootElement.TryGetProperty(property.Name, out var element))
                {
                    continue;
                }

                try
                {
                    var value = JsonSerializer.Deserialize(element.GetRawText(), property.PropertyType);
                    property.SetValue(null, value);
                }
                catch (Exception ex)
                {
                    LegacyCompatibilityBootstrap.Logger.Warn(
                        $"Failed to load config property '{property.Name}' for {GetType().FullName}: {ex.Message}");
                }
            }

            ConfigReloaded();
        }
        catch (Exception ex)
        {
            LegacyCompatibilityBootstrap.Logger.Warn(
                $"Failed to load config '{GetType().FullName}' from '{_configPath}': {ex.Message}");
        }
    }

    public void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var property in _configProperties)
            {
                payload[property.Name] = property.GetValue(null);
            }

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_configPath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LegacyCompatibilityBootstrap.Logger.Warn(
                $"Failed to save config '{GetType().FullName}' to '{_configPath}': {ex.Message}");
        }
    }

    public static void Load<T>() where T : ModConfig
    {
        ModConfigRegistry.Get<T>()?.Load();
    }

    public static void SaveDebounced<T>(int delayMs = 1000) where T : ModConfig
    {
        _ = delayMs;
        ModConfigRegistry.Get<T>()?.Save();
    }

    private static string SanitizeFileName(string input)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            builder.Append(invalidChars.Contains(c) ? '_' : c);
        }

        return builder.ToString();
    }
}

public class SimpleModConfig : ModConfig
{
    public SimpleModConfig(string? filename = null) : base(filename)
    {
    }

    public override void SetupConfigUI(Control optionContainer)
    {
    }
}

public static class LegacyDynamicVarExtensions
{
    internal static readonly SpireField<DynamicVar, decimal?> UpgradeValues = new(static () => null);

    public static TDynamicVar WithUpgrade<TDynamicVar>(this TDynamicVar dynamicVar, decimal upgradeValue)
        where TDynamicVar : DynamicVar
    {
        if (upgradeValue != 0m)
        {
            UpgradeValues[dynamicVar] = upgradeValue;
        }

        return dynamicVar;
    }
}

public abstract class CustomCardModel : CardModel, ICustomModel, ILocalizationProvider
{
    private bool _customFrameMaterialInitialized;
    private Material? _customFrameMaterial;

    protected CustomCardModel(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true,
        bool autoAdd = true)
        : base(baseCost, type, rarity, target, showInCardLibrary)
    {
        _ = autoAdd;
    }

    public override bool GainsBlock => DynamicVars.Any(static pair => pair.Value is BlockVar or CalculatedBlockVar);

    public virtual Texture2D? CustomFrame => null;

    public virtual Material? CreateCustomFrameMaterial => null;

    public Material? CustomFrameMaterial
    {
        get
        {
            if (!_customFrameMaterialInitialized)
            {
                _customFrameMaterialInitialized = true;
                _customFrameMaterial = CreateCustomFrameMaterial;
            }

            return _customFrameMaterial;
        }
    }

    public virtual string? CustomPortraitPath => null;

    public virtual Texture2D? CustomPortrait => null;

    public virtual List<(string, string)>? Localization => null;
}

public abstract class ConstructedCardModel : CustomCardModel
{
    protected enum UpgradeType
    {
        None,
        Add,
        Remove
    }

    private readonly List<CardKeyword> _cardKeywords = [];
    private readonly List<(CardKeyword Keyword, UpgradeType UpgradeType)> _upgradeKeywords = [];
    private readonly List<DynamicVar> _constructedDynamicVars = [];
    private readonly List<TooltipSource> _hoverTips = [];
    private readonly HashSet<CardTag> _constructedTags = [];

    protected ConstructedCardModel(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true,
        bool autoAdd = true)
        : base(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => _constructedDynamicVars;

    public override IEnumerable<CardKeyword> CanonicalKeywords => _cardKeywords;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(static tip => tip.Tip(null!));

    protected override HashSet<CardTag> CanonicalTags => _constructedTags;

    internal int? CostUpgrade { get; private set; }

    protected ConstructedCardModel WithVars(params DynamicVar[] vars)
    {
        foreach (var dynamicVar in vars)
        {
            _constructedDynamicVars.Add(dynamicVar);
        }

        return this;
    }

    protected ConstructedCardModel WithVar(string name, int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new DynamicVar(name, baseVal).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithVar(DynamicVar var)
    {
        _constructedDynamicVars.Add(var);
        return this;
    }

    protected ConstructedCardModel WithBlock(int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new BlockVar(baseVal, ValueProp.Move).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithDamage(int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new DamageVar(baseVal, ValueProp.Move).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithCards(int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new CardsVar(baseVal).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithEnergy(int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new EnergyVar(baseVal).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithHeal(int baseVal, int upgrade = 0)
    {
        _constructedDynamicVars.Add(new HealVar(baseVal).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithPower<T>(int baseVal, int upgrade = 0) where T : PowerModel
    {
        _constructedDynamicVars.Add(new PowerVar<T>(baseVal).WithUpgrade(upgrade));
        _hoverTips.Add(typeof(T));
        return this;
    }

    protected ConstructedCardModel WithPower<T>(string name, int baseVal, int upgrade = 0) where T : PowerModel
    {
        _constructedDynamicVars.Add(new PowerVar<T>(name, baseVal).WithUpgrade(upgrade));
        _hoverTips.Add(typeof(T));
        return this;
    }

    protected ConstructedCardModel WithTags(params CardTag[] tags)
    {
        foreach (var tag in tags)
        {
            _constructedTags.Add(tag);
        }

        return this;
    }

    protected ConstructedCardModel WithKeywords(params CardKeyword[] keywords)
    {
        _cardKeywords.AddRange(keywords);
        return this;
    }

    protected ConstructedCardModel WithKeyword(CardKeyword keyword, UpgradeType upgradeType = UpgradeType.None)
    {
        if (upgradeType != UpgradeType.Add)
        {
            _cardKeywords.Add(keyword);
        }

        if (upgradeType != UpgradeType.None)
        {
            _upgradeKeywords.Add((keyword, upgradeType));
        }

        return this;
    }

    protected ConstructedCardModel WithCostUpgradeBy(int amount)
    {
        CostUpgrade = amount;
        return this;
    }

    protected ConstructedCardModel WithTip(TooltipSource tipSource)
    {
        _hoverTips.Add(tipSource);
        return this;
    }

    protected ConstructedCardModel WithEnergyTip()
    {
        return this;
    }

    public void ConstructedUpgrade()
    {
        foreach (var upgrade in _upgradeKeywords)
        {
            if (upgrade.UpgradeType == UpgradeType.Add)
            {
                AddKeyword(upgrade.Keyword);
            }
            else if (upgrade.UpgradeType == UpgradeType.Remove)
            {
                RemoveKeyword(upgrade.Keyword);
            }
        }

        if (CostUpgrade.HasValue)
        {
            EnergyCost.UpgradeBy(CostUpgrade.Value);
        }
    }
}

public abstract class CustomRelicModel : RelicModel, ICustomModel, ILocalizationProvider
{
    protected CustomRelicModel(bool autoAdd = true)
    {
        _ = autoAdd;
    }

    public virtual RelicModel? GetUpgradeReplacement() => null;

    public virtual List<(string, string)>? Localization => null;
}

public interface ICustomPower : ICustomModel, IModPowerAssetOverrides
{
    string? CustomPackedIconPath => null;

    new string? CustomBigIconPath => null;

    string? CustomBigBetaIconPath => null;

    PowerAssetProfile IModPowerAssetOverrides.AssetProfile => PowerAssetProfile.Empty;

    string? IModPowerAssetOverrides.CustomIconPath => CustomPackedIconPath;

    string? IModPowerAssetOverrides.CustomBigIconPath => CustomBigIconPath ?? CustomBigBetaIconPath;
}

public abstract class CustomPowerModel : PowerModel, ICustomPower, ILocalizationProvider, IHealthBarForecastSource
{
    public virtual string? CustomPackedIconPath => null;

    public virtual string? CustomBigIconPath => null;

    public virtual string? CustomBigBetaIconPath => null;

    public virtual IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        return [];
    }

    public virtual List<(string, string)>? Localization => null;
}

public abstract class CustomTemporaryPowerModel : CustomPowerModel
{
    private const string RepeatVarName = "Repeat";
    private const string EndOtherSideTurnVarName = "UntilEndOfOtherSideTurn";
    private bool _ignoreNextInstance;

    protected abstract Func<Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc { get; }

    public abstract PowerModel InternallyAppliedPower { get; }

    public abstract AbstractModel OriginModel { get; }

    protected virtual bool UntilEndOfOtherSideTurn => false;

    protected virtual int LastForXExtraTurns => 0;

    public override PowerType Type => InternallyAppliedPower.Type;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool AllowNegative => true;

    public override bool IsInstanced => LastForXExtraTurns != 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(RepeatVarName, 0m),
        new DynamicVar(EndOtherSideTurnVarName, 0m)
    ];

    public void IgnoreNextInstance()
    {
        _ignoreNextInstance = true;
    }

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_ignoreNextInstance)
        {
            _ignoreNextInstance = false;
            return;
        }

        if (DynamicVars.TryGetValue(RepeatVarName, out var repeat))
        {
            repeat.BaseValue = LastForXExtraTurns;
        }

        if (DynamicVars.TryGetValue(EndOtherSideTurnVarName, out var sideTurn))
        {
            sideTurn.BaseValue = UntilEndOfOtherSideTurn ? 1m : 0m;
        }

        await ApplyPowerFunc(target, amount, applier, cardSource, true);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!ReferenceEquals(power, this))
        {
            return;
        }

        if (_ignoreNextInstance)
        {
            _ignoreNextInstance = false;
            return;
        }

        await ApplyPowerFunc(Owner, amount, applier, cardSource, true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if ((!UntilEndOfOtherSideTurn && side != Owner.Side) || (UntilEndOfOtherSideTurn && side == Owner.Side))
        {
            return;
        }

        if (DynamicVars.TryGetValue(RepeatVarName, out var repeat) && repeat.BaseValue > 0m)
        {
            repeat.UpgradeValueBy(-1m);
            return;
        }

        await ApplyPowerFunc(Owner, -Amount, Owner, null, true);
        await PowerCmd.Remove(this);
    }
}

public abstract class CustomTemporaryPowerModelWrapper<TModel, TPower> : CustomTemporaryPowerModel
    where TModel : AbstractModel
    where TPower : PowerModel
{
    public override string? CustomBigBetaIconPath => InternallyAppliedPower.ResolvedBigIconPath;

    public override string? CustomPackedIconPath => InternallyAppliedPower.PackedIconPath;

    public override string? CustomBigIconPath => InternallyAppliedPower.ResolvedBigIconPath;

    public override AbstractModel OriginModel => ModelDb.GetById<AbstractModel>(ModelDb.GetId<TModel>());

    public override PowerModel InternallyAppliedPower => ModelDb.Power<TPower>();

    protected override Func<Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc => PowerCmd.Apply<TPower>;

    public override LocString Title => InternallyAppliedPower.Title;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower(InternallyAppliedPower)];

    public override LocString Description => InternallyAppliedPower.Description;
}

public abstract class CustomCardPoolModel : CardPoolModel, ICustomModel, IModBigEnergyIconPool, IModTextEnergyIconPool
{
    public virtual Texture2D? CustomFrame(CustomCardModel card)
    {
        _ = card;
        return null;
    }

    public override string CardFrameMaterialPath => "card_frame_red";

    public virtual Color ShaderColor => new("FFFFFF");

    public virtual float H => ShaderColor.H;

    public virtual float S => ShaderColor.S;

    public virtual float V => ShaderColor.V;

    protected override CardModel[] GenerateAllCards() => [];

    public virtual bool IsShared => false;

    public override string EnergyColorName => $"{Id.Category}∴{Id.Entry}";

    public virtual string? BigEnergyIconPath => null;

    public virtual string? TextEnergyIconPath => null;
}

public abstract class CustomRelicPoolModel : RelicPoolModel, ICustomModel, IModBigEnergyIconPool, IModTextEnergyIconPool
{
    protected override IEnumerable<RelicModel> GenerateAllRelics() => [];

    public virtual bool IsShared => false;

    public override string EnergyColorName => "colorless";

    public virtual string? BigEnergyIconPath => null;

    public virtual string? TextEnergyIconPath => null;
}

public abstract class CustomPotionPoolModel : PotionPoolModel, ICustomModel, IModBigEnergyIconPool, IModTextEnergyIconPool
{
    protected override IEnumerable<PotionModel> GenerateAllPotions() => [];

    public virtual bool IsShared => false;

    public override string EnergyColorName => "colorless";

    public virtual string? BigEnergyIconPath => null;

    public virtual string? TextEnergyIconPath => null;
}

public abstract class CustomPotionModel : PotionModel, ICustomModel, ILocalizationProvider, IModPotionAssetOverrides
{
    [Obsolete("Pass value in constructor instead. Field will be deleted.")]
    public virtual bool AutoAdd => true;

    protected CustomPotionModel() : this(true)
    {
    }

    protected CustomPotionModel(bool autoAdd = true)
    {
        _ = autoAdd;
    }

    public virtual string? CustomPackedImagePath => null;

    public virtual string? CustomPackedOutlinePath => null;

    public virtual PotionAssetProfile AssetProfile => PotionAssetProfile.Empty;

    public virtual string? CustomImagePath => CustomPackedImagePath ?? AssetProfile.ImagePath;

    public virtual string? CustomOutlinePath => CustomPackedOutlinePath ?? AssetProfile.OutlinePath;

    public virtual List<(string, string)>? Localization => null;
}

public abstract class CustomOrbModel : OrbModel, ICustomModel, ILocalizationProvider, IModOrbAssetOverrides, IModOrbSpriteFactory
{
    public virtual OrbAssetProfile AssetProfile => OrbAssetProfile.Empty;

    public virtual string? CustomIconPath => AssetProfile.IconPath;

    public virtual string? CustomSpritePath => null;

    public virtual string? CustomVisualsScenePath => CustomSpritePath ?? AssetProfile.VisualsScenePath;

    public virtual bool IncludeInRandomPool => false;

    public virtual string? CustomPassiveSfx => null;

    public virtual string? CustomEvokeSfx => null;

    public virtual string? CustomChannelSfx => null;

    protected override string PassiveSfx => CustomPassiveSfx ?? base.PassiveSfx;

    protected override string EvokeSfx => CustomEvokeSfx ?? base.EvokeSfx;

    protected override string ChannelSfx => CustomChannelSfx ?? base.ChannelSfx;

    public virtual Node2D? CreateCustomSprite() => null;

    Node2D? IModOrbSpriteFactory.TryCreateOrbSprite() => CreateCustomSprite();

    public virtual List<(string, string)>? Localization => null;
}

public abstract class CustomMonsterModel : MonsterModel, ICustomModel, ISceneConversions
{
    public virtual string? CustomVisualPath => null;

    public virtual NCreatureVisuals? CreateCustomVisuals() => null;

    public virtual string? CustomAttackSfx => null;

    public virtual string? CustomCastSfx => null;

    public virtual string? CustomDeathSfx => null;

    public virtual CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        _ = controller;
        return null;
    }

    public static CreatureAnimator SetupAnimationState(
        MegaSprite controller,
        string idleName,
        string? deadName = null,
        bool deadLoop = false,
        string? hitName = null,
        bool hitLoop = false,
        string? attackName = null,
        bool attackLoop = false,
        string? castName = null,
        bool castLoop = false)
    {
        var idleAnim = new AnimState(idleName, true);
        var deadAnim = deadName == null ? idleAnim : new AnimState(deadName, deadLoop);
        var hitAnim = hitName == null ? idleAnim : new AnimState(hitName, hitLoop) { NextState = idleAnim };
        var attackAnim = attackName == null ? idleAnim : new AnimState(attackName, attackLoop) { NextState = idleAnim };
        var castAnim = castName == null ? idleAnim : new AnimState(castName, castLoop) { NextState = idleAnim };

        var animator = new CreatureAnimator(idleAnim, controller);
        animator.AddAnyState("Idle", idleAnim);
        animator.AddAnyState("Dead", deadAnim);
        animator.AddAnyState("Hit", hitAnim);
        animator.AddAnyState("Attack", attackAnim);
        animator.AddAnyState("Cast", castAnim);
        return animator;
    }

    public void RegisterSceneConversions()
    {
        if (!string.IsNullOrWhiteSpace(CustomVisualPath))
        {
            LegacyNodeFactory.RegisterSceneType<NCreatureVisuals>(CustomVisualPath);
        }
    }
}

public abstract class CustomEncounterModel : EncounterModel, ICustomModel, IModEncounterAssetOverrides
{
    private BackgroundAssets? _customBackgroundAssets;

    protected CustomEncounterModel(RoomType roomType, bool autoAdd = true)
    {
        _ = autoAdd;
        RoomType = roomType;
    }

    public override RoomType RoomType { get; }

    public abstract bool IsValidForAct(ActModel act);

    public virtual string? CustomScenePath => null;

    public EncounterAssetProfile AssetProfile => EncounterAssetProfile.Empty;

    string? IModEncounterAssetOverrides.CustomEncounterScenePath => CustomScenePath;

    string? IModEncounterAssetOverrides.CustomBackgroundScenePath => null;

    string? IModEncounterAssetOverrides.CustomBackgroundLayersDirectoryPath => null;

    string? IModEncounterAssetOverrides.CustomBossNodePath => null;

    IEnumerable<string>? IModEncounterAssetOverrides.CustomExtraAssetPaths => null;

    IEnumerable<string>? IModEncounterAssetOverrides.CustomMapNodeAssetPaths => null;

    string? IModEncounterAssetOverrides.CustomRunHistoryIconPath => CustomRunHistoryIconPath;

    string? IModEncounterAssetOverrides.CustomRunHistoryIconOutlinePath => CustomRunHistoryIconOutlinePath;

    public override bool HasScene =>
        (!string.IsNullOrWhiteSpace(CustomScenePath) && ResourceLoader.Exists(CustomScenePath)) ||
        (!string.IsNullOrWhiteSpace(GetFallbackEncounterScenePath()) && ResourceLoader.Exists(GetFallbackEncounterScenePath()!));

    protected override bool HasCustomBackground => _customBackgroundAssets != null;

    public virtual BackgroundAssets? CustomEncounterBackground(ActModel parentAct, Rng rng)
    {
        _ = parentAct;
        _ = rng;
        return null;
    }

    public virtual string? CustomRunHistoryIconPath => null;

    public virtual string? CustomRunHistoryIconOutlinePath => null;

    protected internal void PrepCustomBackground(ActModel parentAct, Rng rng)
    {
        _customBackgroundAssets = CustomEncounterBackground(parentAct, rng);
    }

    internal BackgroundAssets? GetPreparedBackgroundAssets()
    {
        return _customBackgroundAssets;
    }

    private string? GetFallbackEncounterScenePath()
    {
        var reflected = AccessTools.Property(typeof(EncounterModel), "ScenePath")?.GetValue(this) as string;
        if (!string.IsNullOrWhiteSpace(reflected))
        {
            return reflected;
        }

        return Id == default || string.IsNullOrWhiteSpace(Id.Entry)
            ? null
            : SceneHelper.GetScenePath($"encounters/{Id.Entry.ToLowerInvariant()}");
    }
}

public static class AncientDialogueUtil
{
    private const string ArchitectKey = "THE_ARCHITECT";
    private const string AttackKey = "-attack";
    private const string VisitIndexKey = "-visit";

    public static string SfxPath(string dialogueLoc)
    {
        return LocString.GetIfExists("ancients", dialogueLoc + ".sfx")?.GetRawText() ?? string.Empty;
    }

    public static string BaseLocKey(string ancientId, string charId)
    {
        return $"{ancientId}.talk.{charId}.";
    }

    public static List<AncientDialogue> GetDialoguesForKey(string locTable, string baseKey, StringBuilder? log = null)
    {
        log?.AppendLine($"Looking for dialogues for '{baseKey}' in {locTable}.json");
        var dialogues = new List<AncientDialogue>();
        var isArchitect = baseKey.StartsWith(ArchitectKey, StringComparison.Ordinal);

        var index = 0;
        while (DialogueExists(locTable, baseKey, index))
        {
            var visitIndex = isArchitect
                ? index
                : index switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 4,
                    _ => index + 3
                };

            var visitIndexLoc = LocString.GetIfExists(locTable, $"{baseKey}{index}{VisitIndexKey}");
            if (visitIndexLoc != null && int.TryParse(visitIndexLoc.GetRawText(), out var explicitVisitIndex))
            {
                visitIndex = explicitVisitIndex;
            }

            var sfxPaths = new List<string>();
            var lineIndex = 0;
            string? lineKey;
            while ((lineKey = ExistingLine(locTable, baseKey, index, lineIndex)) != null)
            {
                sfxPaths.Add(SfxPath(lineKey));
                lineIndex++;
            }

            var attackers = ArchitectAttackers.None;
            if (isArchitect)
            {
                attackers = ArchitectAttackers.Architect;
                var attackString = LocString.GetIfExists(locTable, $"{baseKey}{index}{AttackKey}")?.GetRawText();
                if (Enum.TryParse<ArchitectAttackers>(attackString, true, out var parsedAttackers))
                {
                    attackers = parsedAttackers;
                }
            }

            dialogues.Add(new AncientDialogue(sfxPaths.ToArray())
            {
                VisitIndex = visitIndex,
                EndAttackers = attackers
            });
            index++;
        }

        return dialogues;
    }

    private static bool DialogueExists(string locTable, string baseKey, int index)
    {
        return LocString.Exists(locTable, $"{baseKey}{index}-0.ancient") ||
               LocString.Exists(locTable, $"{baseKey}{index}-0r.ancient") ||
               LocString.Exists(locTable, $"{baseKey}{index}-0.char") ||
               LocString.Exists(locTable, $"{baseKey}{index}-0r.char");
    }

    private static string? ExistingLine(string locTable, string baseKey, int dialogueIndex, int lineIndex)
    {
        var candidates = new[]
        {
            $"{baseKey}{dialogueIndex}-{lineIndex}r.ancient",
            $"{baseKey}{dialogueIndex}-{lineIndex}r.char",
            $"{baseKey}{dialogueIndex}-{lineIndex}.ancient",
            $"{baseKey}{dialogueIndex}-{lineIndex}.char"
        };

        return candidates.FirstOrDefault(candidate => LocString.Exists(locTable, candidate));
    }
}

public abstract class CustomAncientModel : AncientEventModel, ICustomModel, ILocalizationProvider
{
    private readonly bool _logDialogueLoad;
    private OptionPools? _optionPools;

    protected CustomAncientModel(bool autoAdd = true, bool logDialogueLoad = false)
    {
        _ = autoAdd;
        _logDialogueLoad = logDialogueLoad;
    }

    public virtual List<(string, string)>? Localization => null;

    public virtual bool IsValidForAct(ActModel act)
    {
        _ = act;
        return true;
    }

    public virtual bool ShouldForceSpawn(ActModel act, AncientEventModel? rngChosenAncient)
    {
        _ = act;
        _ = rngChosenAncient;
        return false;
    }

    protected abstract OptionPools MakeOptionPools { get; }

    public OptionPools OptionPools => _optionPools ??= MakeOptionPools;

    public override IEnumerable<EventOption> AllPossibleOptions
    {
        get
        {
            foreach (var option in OptionPools.AllOptions)
            {
                foreach (var variant in option.AllVariants)
                {
                    yield return RelicOption(variant);
                }
            }
        }
    }

    public virtual string? CustomScenePath => null;

    public virtual string? CustomMapIconPath => null;

    public virtual string? CustomMapIconOutlinePath => null;

    public virtual string? CustomRunHistoryIconPath => null;

    public virtual string? CustomRunHistoryIconOutlinePath => null;

    public override IEnumerable<string> GetAssetPaths(IRunState runState)
    {
        foreach (var path in base.GetAssetPaths(runState))
        {
            yield return path;
        }

        if (!string.IsNullOrWhiteSpace(CustomScenePath))
        {
            yield return CustomScenePath;
        }
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return OptionPools.Roll(Rng).Select(option => RelicOption(option.ModelForOption)).ToList();
    }

    public static WeightedList<AncientOption> MakePool(params RelicModel[] options)
    {
        var pool = new WeightedList<AncientOption>();
        foreach (var option in options)
        {
            pool.Add((AncientOption)option);
        }

        return pool;
    }

    public static WeightedList<AncientOption> MakePool(params AncientOption[] options)
    {
        var pool = new WeightedList<AncientOption>();
        foreach (var option in options)
        {
            pool.Add(option);
        }

        return pool;
    }

    public static AncientOption AncientOption<T>(
        int weight = 1,
        Func<T, RelicModel>? relicPrep = null,
        Func<T, IEnumerable<RelicModel>>? makeAllVariants = null) where T : RelicModel
    {
        return new AncientOption<T>(weight)
        {
            ModelPrep = relicPrep,
            Variants = makeAllVariants
        };
    }

    protected override AncientDialogueSet DefineDialogues()
    {
        var firstVisitKey = $"{Id.Entry}.talk.firstvisitEver.0-0.ancient";
        var firstVisit = new AncientDialogue(AncientDialogueUtil.SfxPath(firstVisitKey));
        var characterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(StringComparer.OrdinalIgnoreCase);

        foreach (var character in ModelDb.AllCharacters)
        {
            var baseKey = AncientDialogueUtil.BaseLocKey(Id.Entry, character.Id.Entry);
            characterDialogues[character.Id.Entry] = AncientDialogueUtil.GetDialoguesForKey("ancients", baseKey);
        }

        if (_logDialogueLoad)
        {
            LegacyCompatibilityBootstrap.Logger.Debug($"Prepared ancient dialogue set for {Id.Entry}.");
        }

        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = firstVisit,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = AncientDialogueUtil.GetDialoguesForKey(
                "ancients",
                AncientDialogueUtil.BaseLocKey(Id.Entry, "ANY"))
        };
    }
}

public abstract class CustomCharacterModel : CharacterModel, ICustomModel, ILocalizationProvider, ISceneConversions
{
    public virtual List<(string, string)>? Localization => null;

    public virtual string? CustomVisualPath => null;

    public virtual string? CustomTrailPath => null;

    public virtual string? CustomIconTexturePath => null;

    public virtual string? CustomIconPath => null;

    public virtual Control? CustomIcon => null;

    public virtual string? CustomEnergyCounterPath => null;

    public virtual string? CustomRestSiteAnimPath => null;

    public virtual string? CustomMerchantAnimPath => null;

    public virtual string? CustomArmPointingTexturePath => null;

    public virtual string? CustomArmRockTexturePath => null;

    public virtual string? CustomArmPaperTexturePath => null;

    public virtual string? CustomArmScissorsTexturePath => null;

    public virtual string? CustomCharacterSelectBg => null;

    public virtual string? CustomCharacterSelectIconPath => null;

    public virtual string? CustomCharacterSelectLockedIconPath => null;

    public virtual string? CustomCharacterSelectTransitionPath => null;

    public virtual string? CustomMapMarkerPath => null;

    public virtual string? CustomAttackSfx => null;

    public virtual string? CustomCastSfx => null;

    public virtual string? CustomDeathSfx => null;

    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    protected override CharacterModel? UnlocksAfterRunAs => null;

    public virtual NCreatureVisuals? CreateCustomVisuals() => null;

    public virtual CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        _ = controller;
        return null;
    }

    public static CreatureAnimator SetupAnimationState(
        MegaSprite controller,
        string idleName,
        string? deadName = null,
        bool deadLoop = false,
        string? hitName = null,
        bool hitLoop = false,
        string? attackName = null,
        bool attackLoop = false,
        string? castName = null,
        bool castLoop = false,
        string? relaxedName = null,
        bool relaxedLoop = true)
    {
        var idleAnim = new AnimState(idleName, true);
        var deadAnim = deadName == null ? idleAnim : new AnimState(deadName, deadLoop);
        var hitAnim = hitName == null ? idleAnim : new AnimState(hitName, hitLoop) { NextState = idleAnim };
        var attackAnim = attackName == null ? idleAnim : new AnimState(attackName, attackLoop) { NextState = idleAnim };
        var castAnim = castName == null ? idleAnim : new AnimState(castName, castLoop) { NextState = idleAnim };
        var relaxedAnim = relaxedName == null ? idleAnim : new AnimState(relaxedName, relaxedLoop);
        if (!ReferenceEquals(relaxedAnim, idleAnim))
        {
            relaxedAnim.AddBranch("Idle", idleAnim);
        }

        var animator = new CreatureAnimator(idleAnim, controller);
        animator.AddAnyState("Idle", idleAnim);
        animator.AddAnyState("Dead", deadAnim);
        animator.AddAnyState("Hit", hitAnim);
        animator.AddAnyState("Attack", attackAnim);
        animator.AddAnyState("Cast", castAnim);
        animator.AddAnyState("Relaxed", relaxedAnim);
        return animator;
    }

    public void RegisterSceneConversions()
    {
        if (!string.IsNullOrWhiteSpace(CustomVisualPath))
        {
            LegacyNodeFactory.RegisterSceneType<NCreatureVisuals>(CustomVisualPath);
        }

        if (!string.IsNullOrWhiteSpace(CustomRestSiteAnimPath))
        {
            LegacyNodeFactory.RegisterSceneType<NRestSiteCharacter>(CustomRestSiteAnimPath);
        }

        if (!string.IsNullOrWhiteSpace(CustomMerchantAnimPath))
        {
            LegacyNodeFactory.RegisterSceneType<NMerchantCharacter>(CustomMerchantAnimPath);
        }
    }
}

public abstract class PlaceholderCharacterModel : CustomCharacterModel
{
    public virtual string PlaceholderID => "ironclad";

    public override string CustomVisualPath => SceneHelper.GetScenePath("creature_visuals/" + PlaceholderID);

    public override string CustomTrailPath => SceneHelper.GetScenePath("vfx/card_trail_" + PlaceholderID);

    public override string? CustomMapMarkerPath =>
        ImageHelper.GetImagePath("packed/map/icons/map_marker_" + PlaceholderID + ".png");

    public override string CustomIconPath => SceneHelper.GetScenePath("ui/character_icons/" + PlaceholderID + "_icon");

    public override string? CustomIconTexturePath =>
        ImageHelper.GetImagePath("ui/top_panel/character_icon_" + PlaceholderID + ".png");

    public override string CustomEnergyCounterPath =>
        SceneHelper.GetScenePath("combat/energy_counters/" + PlaceholderID + "_energy_counter");

    public override string CustomRestSiteAnimPath =>
        SceneHelper.GetScenePath("rest_site/characters/" + PlaceholderID + "_rest_site");

    public override string CustomMerchantAnimPath =>
        SceneHelper.GetScenePath("merchant/characters/" + PlaceholderID + "_merchant");

    public override string CustomArmPointingTexturePath =>
        ImageHelper.GetImagePath("ui/hands/" + PlaceholderID + "_arm_point.png");

    public override string CustomArmRockTexturePath =>
        ImageHelper.GetImagePath("ui/hands/" + PlaceholderID + "_arm_rock.png");

    public override string CustomArmPaperTexturePath =>
        ImageHelper.GetImagePath("ui/hands/" + PlaceholderID + "_arm_paper.png");

    public override string CustomArmScissorsTexturePath =>
        ImageHelper.GetImagePath("ui/hands/" + PlaceholderID + "_arm_scissors.png");

    public override string CustomCharacterSelectBg =>
        SceneHelper.GetScenePath("screens/char_select/char_select_bg_" + PlaceholderID);

    public override string CustomCharacterSelectTransitionPath =>
        "res://materials/transitions/" + PlaceholderID + "_transition_mat.tres";

    public override string? CustomCharacterSelectIconPath =>
        ImageHelper.GetImagePath("packed/character_select/char_select_" + PlaceholderID + ".png");

    public override string? CustomCharacterSelectLockedIconPath =>
        ImageHelper.GetImagePath("packed/character_select/char_select_" + PlaceholderID + "_locked.png");

    public override string CharacterSelectSfx => $"event:/sfx/characters/{PlaceholderID}/{PlaceholderID}_select";

    public override string CustomAttackSfx => $"event:/sfx/characters/{PlaceholderID}/{PlaceholderID}_attack";

    public override string CustomCastSfx => $"event:/sfx/characters/{PlaceholderID}/{PlaceholderID}_cast";

    public override string CustomDeathSfx => $"event:/sfx/characters/{PlaceholderID}/{PlaceholderID}_die";

    public override List<string> GetArchitectAttackVfx()
    {
        const int count = 5;
        var results = new List<string>(count);
        CollectionsMarshal.SetCount(results, count);

        var span = CollectionsMarshal.AsSpan(results);
        span[0] = "vfx/vfx_attack_blunt";
        span[1] = "vfx/vfx_heavy_blunt";
        span[2] = "vfx/vfx_attack_slash";
        span[3] = "vfx/vfx_bloody_impact";
        span[4] = "vfx/vfx_rock_shatter";
        return results;
    }
}

[ModInitializer(nameof(Initialize))]
public static class LegacyCompatibilityBootstrap
{
    private static readonly object SyncRoot = new();
    private static readonly HashSet<Type> PendingPoolTypes = [];
    private static bool _initialized;
    private static string _modId = "BaseLibToRitsu";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; private set; } =
        new MegaCrit.Sts2.Core.Logging.Logger("BaseLibToRitsu", LogType.Generic);

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _modId = ResolveModId();
            Logger = new MegaCrit.Sts2.Core.Logging.Logger(_modId, LogType.Generic);

            var harmony = new Harmony(_modId + ".BaseLibToRitsuCompat");
            foreach (var patchType in GetPatchTypes())
            {
                TryPatchClass(harmony, patchType);
            }

            TryRegisterCustomEnums();
            TryRegisterContent();
        }
    }

    internal static void AfterModelDbInit()
    {
        TryInjectPendingPoolTypes(afterModelDbInit: true);
        TryRegisterSceneConversions();
    }

    private static IEnumerable<Type> GetPatchTypes()
    {
        yield return typeof(CardKeywordLocKeyPatch);
        yield return typeof(CustomKeywordHoverTipPatch);
        yield return typeof(DynamicVarClonePatch);
        yield return typeof(CardUpgradeInternalPatch);
        yield return typeof(ModelDbInitPatch);
        yield return typeof(CardFramePatch);
        yield return typeof(CardFrameMaterialPatch);
        yield return typeof(CardPortraitPngPathPatch);
        yield return typeof(CardPortraitPatch);
        yield return typeof(CardPortraitPathPatch);
        yield return typeof(PowerPackedIconPathPatch);
        yield return typeof(PowerBigIconPathPatch);
        yield return typeof(PowerBigBetaIconPathPatch);
        yield return typeof(MonsterCreateVisualsPatch);
        yield return typeof(MonsterVisualsPathPatch);
        yield return typeof(MonsterGenerateAnimatorPatch);
        yield return typeof(MonsterAttackSfxPatch);
        yield return typeof(MonsterCastSfxPatch);
        yield return typeof(MonsterDeathSfxPatch);
        yield return typeof(EncounterScenePathPatch);
        yield return typeof(EncounterBackgroundPrepPatch);
        yield return typeof(EncounterBackgroundCreatePatch);
        yield return typeof(AncientBackgroundScenePathPatch);
        yield return typeof(AncientMapIconPathPatch);
        yield return typeof(AncientMapIconOutlinePathPatch);
        yield return typeof(AncientRunHistoryIconOutlinePathPatch);
        yield return typeof(RoomIconPathPatch);
        yield return typeof(RoomIconOutlinePathPatch);
        yield return typeof(CharacterVisualsPathPatch);
        yield return typeof(CharacterCreateVisualsPatch);
        yield return typeof(CharacterGenerateAnimatorPatch);
        yield return typeof(CharacterTrailPathPatch);
        yield return typeof(CharacterIconTexturePathPatch);
        yield return typeof(CharacterIconPatch);
        yield return typeof(CharacterIconPathPatch);
        yield return typeof(CharacterEnergyCounterPathPatch);
        yield return typeof(CharacterRestSiteAnimPathPatch);
        yield return typeof(CharacterMerchantAnimPathPatch);
        yield return typeof(CharacterArmPointingTexturePathPatch);
        yield return typeof(CharacterArmRockTexturePathPatch);
        yield return typeof(CharacterArmPaperTexturePathPatch);
        yield return typeof(CharacterArmScissorsTexturePathPatch);
        yield return typeof(CharacterTransitionPathPatch);
        yield return typeof(CharacterSelectBgPatch);
        yield return typeof(CharacterSelectIconPathPatch);
        yield return typeof(CharacterSelectLockedIconPathPatch);
        yield return typeof(CharacterMapMarkerPathPatch);
        yield return typeof(CharacterAttackSfxPatch);
        yield return typeof(CharacterCastSfxPatch);
        yield return typeof(CharacterDeathSfxPatch);
    }

    private static void TryPatchClass(Harmony harmony, Type patchType)
    {
        try
        {
            harmony.CreateClassProcessor(patchType).Patch();
        }
        catch (Exception ex)
        {
            Logger.Warn($"Failed to patch compatibility class '{patchType.FullName}': {ex.Message}");
        }
    }

    private static void TryRegisterContent()
    {
        try
        {
            var registry = RitsuLibFramework.GetContentRegistry(_modId);
            var projectTypes = GetProjectTypes().ToArray();

            foreach (var poolType in projectTypes.Where(static type => typeof(CustomCardPoolModel).IsAssignableFrom(type)))
            {
                RememberPoolType(poolType);
                if (IsSharedPool(poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterSharedCardPool), poolType);
                }
            }

            foreach (var poolType in projectTypes.Where(static type => typeof(CustomRelicPoolModel).IsAssignableFrom(type)))
            {
                RememberPoolType(poolType);
                if (IsSharedPool(poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterSharedRelicPool), poolType);
                }
            }

            foreach (var poolType in projectTypes.Where(static type => typeof(CustomPotionPoolModel).IsAssignableFrom(type)))
            {
                RememberPoolType(poolType);
                if (IsSharedPool(poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterSharedPotionPool), poolType);
                }
            }

            foreach (var cardType in projectTypes.Where(static type => typeof(CustomCardModel).IsAssignableFrom(type)))
            {
                if (TryGetPoolType(cardType, typeof(CardPoolModel), out var poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterCard), poolType, cardType);
                    RememberPoolType(poolType);
                }
            }

            foreach (var relicType in projectTypes.Where(static type => typeof(CustomRelicModel).IsAssignableFrom(type)))
            {
                if (TryGetPoolType(relicType, typeof(RelicPoolModel), out var poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterRelic), poolType, relicType);
                    RememberPoolType(poolType);
                }
            }

            foreach (var potionType in projectTypes.Where(static type => typeof(PotionModel).IsAssignableFrom(type)))
            {
                if (TryGetPoolType(potionType, typeof(PotionPoolModel), out var poolType))
                {
                    InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterPotion), poolType, potionType);
                    RememberPoolType(poolType);
                }
            }

            foreach (var powerType in projectTypes.Where(static type => typeof(CustomPowerModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterPower), powerType);
            }

            foreach (var orbType in projectTypes.Where(static type => typeof(CustomOrbModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterOrb), orbType);
            }

            foreach (var monsterType in projectTypes.Where(static type => typeof(CustomMonsterModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterMonster), monsterType);
            }

            foreach (var characterType in projectTypes.Where(static type => typeof(CustomCharacterModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterCharacter), characterType);
            }

            foreach (var encounterType in projectTypes.Where(static type => typeof(CustomEncounterModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterGlobalEncounter), encounterType);
            }

            foreach (var ancientType in projectTypes.Where(static type => typeof(CustomAncientModel).IsAssignableFrom(type)))
            {
                InvokeGeneric(registry, nameof(STS2RitsuLib.Content.ModContentRegistry.RegisterSharedAncient), ancientType);
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"Compatibility content registration failed: {ex.Message}");
        }
    }

    private static void TryRegisterCustomEnums()
    {
        var customEnumFields = GetProjectTypes()
            .SelectMany(static type => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            .Where(static field => field.GetCustomAttribute<CustomEnumAttribute>() != null)
            .OrderBy(static field => field.Name, StringComparer.Ordinal)
            .ThenBy(static field => field.DeclaringType?.FullName, StringComparer.Ordinal);

        foreach (var field in customEnumFields)
        {
            if (!field.IsStatic)
            {
                Logger.Warn($"Skipping [CustomEnum] field '{field.DeclaringType?.FullName}.{field.Name}' because it is not static.");
                continue;
            }

            if (!field.FieldType.IsEnum)
            {
                Logger.Warn($"Skipping [CustomEnum] field '{field.DeclaringType?.FullName}.{field.Name}' because it is not an enum type.");
                continue;
            }

            try
            {
                var generated = CustomEnums.GenerateKey(field, _modId);
                field.SetValue(null, generated);

                if (field.FieldType == typeof(CardKeyword))
                {
                    RegisterLegacyCardKeyword(field, (CardKeyword)generated);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to assign [CustomEnum] field '{field.DeclaringType?.FullName}.{field.Name}': {ex.Message}");
            }
        }
    }

    private static void RegisterLegacyCardKeyword(FieldInfo field, CardKeyword keyword)
    {
        var customEnum = field.GetCustomAttribute<CustomEnumAttribute>();
        var keywordStem = string.IsNullOrWhiteSpace(customEnum?.Name) ? field.Name : customEnum!.Name!.Trim();
        var fallbackKey = keywordStem.ToUpperInvariant();
        var canonicalKey = GetTypePrefix(field.DeclaringType) + fallbackKey;
        var props = field.GetCustomAttribute<KeywordPropertiesAttribute>();

        CustomKeywords.Register(
            keyword,
            canonicalKey,
            string.Equals(canonicalKey, fallbackKey, StringComparison.Ordinal) ? null : fallbackKey,
            props?.Position ?? AutoKeywordPosition.None,
            props?.RichKeyword ?? true);
    }

    private static string GetTypePrefix(Type? type)
    {
        if (type?.Namespace is not string ns || string.IsNullOrWhiteSpace(ns))
        {
            return string.Empty;
        }

        var dotIndex = ns.IndexOf('.');
        if (dotIndex < 0)
        {
            dotIndex = ns.Length;
        }

        return ns[..dotIndex].ToUpperInvariant() + "-";
    }

    private static void TryRegisterSceneConversions()
    {
        var seenTypes = new HashSet<Type>();
        foreach (var sceneConversions in GetRegisteredSceneConversionModels())
        {
            if (!seenTypes.Add(sceneConversions.GetType()))
            {
                continue;
            }

            try
            {
                sceneConversions.RegisterSceneConversions();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to register scene conversions for '{sceneConversions.GetType().FullName}': {ex.Message}");
            }
        }
    }

    private static IEnumerable<ISceneConversions> GetRegisteredSceneConversionModels()
    {
        foreach (var character in ModelDb.AllCharacters.OfType<ISceneConversions>())
        {
            yield return character;
        }

        foreach (var monster in ModelDb.Monsters.OfType<ISceneConversions>())
        {
            yield return monster;
        }
    }

    private static void TryInjectPendingPoolTypes(bool afterModelDbInit)
    {
        foreach (var poolType in PendingPoolTypes.ToArray())
        {
            if (!ShouldInjectPendingPoolType(poolType, afterModelDbInit))
            {
                continue;
            }

            try
            {
                ModelDb.Inject(poolType);
                PendingPoolTypes.Remove(poolType);
            }
            catch (Exception ex)
            {
                Logger.Debug($"Pool injection skipped for '{poolType.FullName}': {ex.Message}");
            }
        }
    }

    private static IEnumerable<Type> GetProjectTypes()
    {
        return typeof(LegacyCompatibilityBootstrap).Assembly
            .GetTypes()
            .Where(static type =>
                type.IsClass &&
                !type.IsAbstract &&
                !string.Equals(type.Namespace, typeof(LegacyCompatibilityBootstrap).Namespace, StringComparison.Ordinal) &&
                !(type.Namespace?.StartsWith(typeof(LegacyCompatibilityBootstrap).Namespace + ".", StringComparison.Ordinal) ?? false));
    }

    private static void RememberPoolType(Type poolType)
    {
        if (IsInjectableProjectPoolType(poolType))
        {
            PendingPoolTypes.Add(poolType);
        }
    }

    private static bool IsInjectableProjectPoolType(Type poolType)
    {
        if (!IsProjectType(poolType))
        {
            return false;
        }

        return typeof(CustomCardPoolModel).IsAssignableFrom(poolType) ||
               typeof(CustomRelicPoolModel).IsAssignableFrom(poolType) ||
               typeof(CustomPotionPoolModel).IsAssignableFrom(poolType);
    }

    private static bool ShouldInjectPendingPoolType(Type poolType, bool afterModelDbInit)
    {
        if (!IsInjectableProjectPoolType(poolType))
        {
            return false;
        }

        return !afterModelDbInit || !ModelDb.Contains(poolType);
    }

    private static bool IsProjectType(Type type)
    {
        return type.Assembly == typeof(LegacyCompatibilityBootstrap).Assembly &&
               type.IsClass &&
               !type.IsAbstract &&
               !string.Equals(type.Namespace, typeof(LegacyCompatibilityBootstrap).Namespace, StringComparison.Ordinal) &&
               !(type.Namespace?.StartsWith(typeof(LegacyCompatibilityBootstrap).Namespace + ".", StringComparison.Ordinal) ?? false);
    }

    private static bool TryGetPoolType(Type modelType, Type expectedBaseType, out Type poolType)
    {
        var attribute = modelType.GetCustomAttribute<PoolAttribute>();
        if (attribute != null && expectedBaseType.IsAssignableFrom(attribute.PoolType))
        {
            poolType = attribute.PoolType;
            return true;
        }

        poolType = null!;
        return false;
    }

    private static bool IsSharedPool(Type poolType)
    {
        if (TryReadBooleanConstantProperty(poolType, "IsShared", out var isShared))
        {
            return isShared;
        }

        Logger.Debug($"Could not statically determine IsShared for '{poolType.FullName}'. Defaulting to false.");
        return false;
    }

    private static bool TryReadBooleanConstantProperty(Type type, string propertyName, out bool value)
    {
        value = false;

        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var getter = property?.GetGetMethod(true);
        if (getter == null || getter.IsStatic)
        {
            return false;
        }

        var body = getter.GetMethodBody();
        var il = body?.GetILAsByteArray();
        if (il == null || il.Length == 0)
        {
            return false;
        }

        var sawBooleanLiteral = false;
        foreach (var opcode in il)
        {
            switch (opcode)
            {
                case 0x16:
                    value = false;
                    sawBooleanLiteral = true;
                    break;
                case 0x17:
                    value = true;
                    sawBooleanLiteral = true;
                    break;
                case 0x2A when sawBooleanLiteral:
                    return true;
            }
        }

        return false;
    }

    private static void InvokeGeneric(object instance, string methodName, params Type[] typeArguments)
    {
        var method = instance.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(methodInfo =>
                methodInfo.Name == methodName &&
                methodInfo.IsGenericMethodDefinition &&
                methodInfo.GetGenericArguments().Length == typeArguments.Length &&
                methodInfo.GetParameters().Length == 0);

        method.MakeGenericMethod(typeArguments).Invoke(instance, null);
    }

    private static string ResolveModId()
    {
        foreach (var type in typeof(LegacyCompatibilityBootstrap).Assembly.GetTypes())
        {
            var field = type.GetField("ModId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null && field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
            {
                var value = field.GetRawConstantValue() as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return typeof(LegacyCompatibilityBootstrap).Assembly.GetName().Name ?? "BaseLibToRitsu";
    }

    [HarmonyPatch]
    private static class CardKeywordLocKeyPatch
    {
        private static MethodBase? TargetMethod()
        {
            return AccessTools.Method(
                "MegaCrit.Sts2.Core.Entities.Cards.CardKeywordExtensions:GetLocKeyPrefix",
                [typeof(CardKeyword)]);
        }

        [HarmonyPrefix]
        private static bool Prefix(CardKeyword keyword, ref string? __result)
        {
            if (!CustomKeywords.TryGet(keyword, out var info))
            {
                return true;
            }

            __result = info.ResolveLocKeyPrefix();
            return false;
        }
    }

    [HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))]
    private static class CustomKeywordHoverTipPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardKeyword keyword, ref IHoverTip __result)
        {
            if (!CustomKeywords.TryGet(keyword, out var info) || !info.RichKeyword)
            {
                return true;
            }

            var locKeyPrefix = info.ResolveLocKeyPrefix();
            var description = LocString.GetIfExists("card_keywords", locKeyPrefix + ".description") ??
                              LocString.GetIfExists("card_keywords", locKeyPrefix + ".desc");
            if (description == null)
            {
                return true;
            }

            description.Add("energyPrefix", string.Empty);
            var title = LocString.GetIfExists("card_keywords", locKeyPrefix + ".title") ??
                        new LocString("card_keywords", locKeyPrefix + ".title");
            __result = new HoverTip(title, description);
            return false;
        }
    }

    [HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.Clone))]
    private static class DynamicVarClonePatch
    {
        [HarmonyPostfix]
        private static void Postfix(DynamicVar __instance, DynamicVar __result)
        {
            LegacyDynamicVarExtensions.UpgradeValues[__result] = LegacyDynamicVarExtensions.UpgradeValues[__instance];
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.UpgradeInternal))]
    private static class CardUpgradeInternalPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CardModel __instance)
        {
            foreach (var dynamicVarEntry in __instance.DynamicVars)
            {
                var upgradeValue = LegacyDynamicVarExtensions.UpgradeValues[dynamicVarEntry.Value];
                if (upgradeValue.HasValue)
                {
                    dynamicVarEntry.Value.UpgradeValueBy(upgradeValue.Value);
                }
            }

            if (__instance is ConstructedCardModel constructedCard)
            {
                constructedCard.ConstructedUpgrade();
            }
        }
    }

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
    private static class ModelDbInitPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            AfterModelDbInit();
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Frame), MethodType.Getter)]
    private static class CardFramePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardModel __instance, ref Texture2D? __result)
        {
            if (__instance is not CustomCardModel customCard)
            {
                return true;
            }

            __result = customCard.CustomFrame;
            if (__result != null)
            {
                return false;
            }

            if (__instance.Pool is CustomCardPoolModel customPool)
            {
                __result = customPool.CustomFrame(customCard);
                return __result == null;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.FrameMaterial), MethodType.Getter)]
    private static class CardFrameMaterialPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardModel __instance, ref Material? __result)
        {
            if (__instance is not CustomCardModel customCard)
            {
                return true;
            }

            __result = customCard.CustomFrameMaterial;
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(CardModel), "PortraitPngPath", MethodType.Getter)]
    private static class CardPortraitPngPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardModel __instance, ref string? __result)
        {
            if (__instance is not CustomCardModel customCard || string.IsNullOrWhiteSpace(customCard.CustomPortraitPath))
            {
                return true;
            }

            __result = customCard.CustomPortraitPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Portrait), MethodType.Getter)]
    private static class CardPortraitPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardModel __instance, ref Texture2D? __result)
        {
            if (__instance is not CustomCardModel customCard)
            {
                return true;
            }

            if (customCard.CustomPortrait != null)
            {
                __result = customCard.CustomPortrait;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(customCard.CustomPortraitPath))
            {
                __result = ResourceLoader.Load<Texture2D>(customCard.CustomPortraitPath);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.PortraitPath), MethodType.Getter)]
    private static class CardPortraitPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CardModel __instance, ref string? __result)
        {
            if (__instance is not CustomCardModel customCard)
            {
                return true;
            }

            if (customCard.CustomPortrait != null)
            {
                __result = customCard.CustomPortrait.ResourcePath;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(customCard.CustomPortraitPath))
            {
                __result = customCard.CustomPortraitPath;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PowerModel), "PackedIconPath", MethodType.Getter)]
    private static class PowerPackedIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PowerModel __instance, ref string? __result)
        {
            if (__instance is not ICustomPower customPower || string.IsNullOrWhiteSpace(customPower.CustomPackedIconPath))
            {
                return true;
            }

            __result = customPower.CustomPackedIconPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(PowerModel), "BigIconPath", MethodType.Getter)]
    private static class PowerBigIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PowerModel __instance, ref string? __result)
        {
            if (__instance is not ICustomPower customPower || string.IsNullOrWhiteSpace(customPower.CustomBigIconPath))
            {
                return true;
            }

            __result = customPower.CustomBigIconPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(PowerModel), "BigBetaIconPath", MethodType.Getter)]
    private static class PowerBigBetaIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PowerModel __instance, ref string? __result)
        {
            if (__instance is not ICustomPower customPower || string.IsNullOrWhiteSpace(customPower.CustomBigBetaIconPath))
            {
                return true;
            }

            __result = customPower.CustomBigBetaIconPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.CreateVisuals))]
    private static class MonsterCreateVisualsPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, ref NCreatureVisuals? __result)
        {
            if (__instance is not CustomMonsterModel monster)
            {
                return true;
            }

            __result = monster.CreateCustomVisuals();
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), "VisualsPath", MethodType.Getter)]
    private static class MonsterVisualsPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, ref string? __result)
        {
            if (__instance is not CustomMonsterModel monster || string.IsNullOrWhiteSpace(monster.CustomVisualPath))
            {
                return true;
            }

            __result = monster.CustomVisualPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.GenerateAnimator))]
    private static class MonsterGenerateAnimatorPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, MegaSprite controller, ref CreatureAnimator? __result)
        {
            if (__instance is not CustomMonsterModel monster)
            {
                return true;
            }

            __result = monster.SetupCustomAnimationStates(controller);
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), "AttackSfx", MethodType.Getter)]
    private static class MonsterAttackSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, ref string? __result)
        {
            if (__instance is not CustomMonsterModel monster || string.IsNullOrWhiteSpace(monster.CustomAttackSfx))
            {
                return true;
            }

            __result = monster.CustomAttackSfx;
            return false;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), "CastSfx", MethodType.Getter)]
    private static class MonsterCastSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, ref string? __result)
        {
            if (__instance is not CustomMonsterModel monster || string.IsNullOrWhiteSpace(monster.CustomCastSfx))
            {
                return true;
            }

            __result = monster.CustomCastSfx;
            return false;
        }
    }

    [HarmonyPatch(typeof(MonsterModel), "DeathSfx", MethodType.Getter)]
    private static class MonsterDeathSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MonsterModel __instance, ref string? __result)
        {
            if (__instance is not CustomMonsterModel monster || string.IsNullOrWhiteSpace(monster.CustomDeathSfx))
            {
                return true;
            }

            __result = monster.CustomDeathSfx;
            return false;
        }
    }

    [HarmonyPatch(typeof(EncounterModel), "ScenePath", MethodType.Getter)]
    private static class EncounterScenePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(EncounterModel __instance, ref string? __result)
        {
            if (__instance is not CustomEncounterModel encounter || string.IsNullOrWhiteSpace(encounter.CustomScenePath))
            {
                return true;
            }

            __result = encounter.CustomScenePath;
            return false;
        }
    }

    [HarmonyPatch(typeof(EncounterModel), "GetBackgroundAssets")]
    private static class EncounterBackgroundPrepPatch
    {
        [HarmonyPrefix]
        private static void Prefix(EncounterModel __instance, ActModel parentAct, Rng rng)
        {
            if (__instance is CustomEncounterModel encounter)
            {
                encounter.PrepCustomBackground(parentAct, rng);
            }
        }
    }

    [HarmonyPatch(typeof(EncounterModel), "CreateBackgroundAssetsForCustom")]
    private static class EncounterBackgroundCreatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(EncounterModel __instance, ref BackgroundAssets? __result)
        {
            if (__instance is not CustomEncounterModel encounter)
            {
                return true;
            }

            __result = encounter.GetPreparedBackgroundAssets();
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(EventModel), "BackgroundScenePath", MethodType.Getter)]
    private static class AncientBackgroundScenePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(AncientEventModel __instance, ref string? __result)
        {
            if (__instance is not CustomAncientModel ancient || string.IsNullOrWhiteSpace(ancient.CustomScenePath))
            {
                return true;
            }

            __result = ancient.CustomScenePath;
            return false;
        }
    }

    [HarmonyPatch(typeof(AncientEventModel), "MapIconPath", MethodType.Getter)]
    private static class AncientMapIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(AncientEventModel __instance, ref string? __result)
        {
            if (__instance is not CustomAncientModel ancient || string.IsNullOrWhiteSpace(ancient.CustomMapIconPath))
            {
                return true;
            }

            __result = ancient.CustomMapIconPath;
            return false;
        }
    }

    [HarmonyPatch(typeof(AncientEventModel), "MapIconOutlinePath", MethodType.Getter)]
    private static class AncientMapIconOutlinePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(AncientEventModel __instance, ref string? __result)
        {
            if (__instance is not CustomAncientModel ancient || string.IsNullOrWhiteSpace(ancient.CustomMapIconOutlinePath))
            {
                return true;
            }

            __result = ancient.CustomMapIconOutlinePath;
            return false;
        }
    }

    [HarmonyPatch(typeof(AncientEventModel), "RunHistoryIconOutlinePath", MethodType.Getter)]
    private static class AncientRunHistoryIconOutlinePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(AncientEventModel __instance, ref string? __result)
        {
            if (__instance is not CustomAncientModel ancient || string.IsNullOrWhiteSpace(ancient.CustomRunHistoryIconOutlinePath))
            {
                return true;
            }

            __result = ancient.CustomRunHistoryIconOutlinePath;
            return false;
        }
    }

    [HarmonyPatch(typeof(ImageHelper), nameof(ImageHelper.GetRoomIconPath))]
    private static class RoomIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MapPointType mapPointType, RoomType roomType, ModelId? modelId, ref string? __result)
        {
            _ = mapPointType;
            _ = roomType;

            if (modelId == null)
            {
                return true;
            }

            var model = ModelDb.GetById<AbstractModel>(modelId);
            switch (model)
            {
                case CustomAncientModel ancient when !string.IsNullOrWhiteSpace(ancient.CustomRunHistoryIconPath):
                    __result = ancient.CustomRunHistoryIconPath;
                    return false;
                case CustomEncounterModel encounter when !string.IsNullOrWhiteSpace(encounter.CustomRunHistoryIconPath):
                    __result = encounter.CustomRunHistoryIconPath;
                    return false;
                default:
                    return true;
            }
        }
    }

    [HarmonyPatch(typeof(ImageHelper), nameof(ImageHelper.GetRoomIconOutlinePath))]
    private static class RoomIconOutlinePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MapPointType mapPointType, RoomType roomType, ModelId? modelId, ref string? __result)
        {
            _ = mapPointType;
            _ = roomType;

            if (modelId == null)
            {
                return true;
            }

            var model = ModelDb.GetById<AbstractModel>(modelId);
            switch (model)
            {
                case CustomAncientModel ancient when !string.IsNullOrWhiteSpace(ancient.CustomRunHistoryIconOutlinePath):
                    __result = ancient.CustomRunHistoryIconOutlinePath;
                    return false;
                case CustomEncounterModel encounter when !string.IsNullOrWhiteSpace(encounter.CustomRunHistoryIconOutlinePath):
                    __result = encounter.CustomRunHistoryIconOutlinePath;
                    return false;
                default:
                    return true;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "VisualsPath", MethodType.Getter)]
    private static class CharacterVisualsPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomVisualPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.CreateVisuals))]
    private static class CharacterCreateVisualsPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref NCreatureVisuals? __result)
        {
            if (__instance is not CustomCharacterModel character)
            {
                return true;
            }

            __result = character.CreateCustomVisuals();
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.GenerateAnimator))]
    private static class CharacterGenerateAnimatorPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, MegaSprite controller, ref CreatureAnimator? __result)
        {
            if (__instance is not CustomCharacterModel character)
            {
                return true;
            }

            __result = character.SetupCustomAnimationStates(controller);
            return __result == null;
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.TrailPath), MethodType.Getter)]
    private static class CharacterTrailPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomTrailPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "IconTexturePath", MethodType.Getter)]
    private static class CharacterIconTexturePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomIconTexturePath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "Icon", MethodType.Getter)]
    private static class CharacterIconPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref Control? __result)
        {
            if (__instance is not CustomCharacterModel character || character.CustomIcon == null)
            {
                return true;
            }

            __result = character.CustomIcon;
            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "IconPath", MethodType.Getter)]
    private static class CharacterIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomIconPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "EnergyCounterPath", MethodType.Getter)]
    private static class CharacterEnergyCounterPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomEnergyCounterPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "RestSiteAnimPath", MethodType.Getter)]
    private static class CharacterRestSiteAnimPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomRestSiteAnimPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.MerchantAnimPath), MethodType.Getter)]
    private static class CharacterMerchantAnimPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomMerchantAnimPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "ArmPointingTexturePath", MethodType.Getter)]
    private static class CharacterArmPointingTexturePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomArmPointingTexturePath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "ArmRockTexturePath", MethodType.Getter)]
    private static class CharacterArmRockTexturePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomArmRockTexturePath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "ArmPaperTexturePath", MethodType.Getter)]
    private static class CharacterArmPaperTexturePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomArmPaperTexturePath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "ArmScissorsTexturePath", MethodType.Getter)]
    private static class CharacterArmScissorsTexturePathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomArmScissorsTexturePath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "CharacterSelectTransitionPath", MethodType.Getter)]
    private static class CharacterTransitionPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomCharacterSelectTransitionPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.CharacterSelectBg), MethodType.Getter)]
    private static class CharacterSelectBgPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomCharacterSelectBg, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "CharacterSelectIconPath", MethodType.Getter)]
    private static class CharacterSelectIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomCharacterSelectIconPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "CharacterSelectLockedIconPath", MethodType.Getter)]
    private static class CharacterSelectLockedIconPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomCharacterSelectLockedIconPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "MapMarkerPath", MethodType.Getter)]
    private static class CharacterMapMarkerPathPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomMapMarkerPath, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "AttackSfx", MethodType.Getter)]
    private static class CharacterAttackSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomAttackSfx, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "CastSfx", MethodType.Getter)]
    private static class CharacterCastSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomCastSfx, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "DeathSfx", MethodType.Getter)]
    private static class CharacterDeathSfxPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CharacterModel __instance, ref string? __result)
        {
            return TryAssignCharacterString(__instance, static character => character.CustomDeathSfx, ref __result);
        }
    }

    private static bool TryAssignCharacterString(CharacterModel instance, Func<CustomCharacterModel, string?> selector, ref string? result)
    {
        if (instance is not CustomCharacterModel character)
        {
            return true;
        }

        var value = selector(character);
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        result = value;
        return false;
    }
}
