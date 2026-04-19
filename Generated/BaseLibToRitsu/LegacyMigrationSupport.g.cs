#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Godot;

namespace BaseLibToRitsu.Generated;

public static class DynamicVarSetExtensions
{
    public static DynamicVar Power<T>(this DynamicVarSet vars) where T : PowerModel
    {
        return vars[typeof(T).Name];
    }
}

public static class StringExtensions
{
    private const char PrefixSplitChar = '-';

    public static string RemovePrefix(this string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return id;
        }

        var separatorIndex = id.IndexOf(PrefixSplitChar);
        return separatorIndex >= 0 && separatorIndex + 1 < id.Length ? id[(separatorIndex + 1)..] : id;
    }

    public static void RegisterSceneForConversion<TNode>(this string scenePath, Action<TNode>? postConversion = null) where TNode : Node, new()
    {
        LegacyNodeFactory.RegisterSceneType(scenePath, postConversion);
    }
}

public static class LegacyValuePropExtensions
{
    public static bool IsPoweredAttack_(this ValueProp props)
    {
        if (props.HasFlag(ValueProp.Move))
        {
            return !props.HasFlag(ValueProp.Unpowered);
        }

        return false;
    }

    public static bool IsPoweredCardOrMonsterMoveBlock_(this ValueProp props)
    {
        if (props.HasFlag(ValueProp.Move))
        {
            return !props.HasFlag(ValueProp.Unpowered);
        }

        return false;
    }

    public static bool IsCardOrMonsterMove_(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move);
    }
}

public sealed class DynamicVarSource
{
    public required DynamicVarSet DynamicVars { get; init; }
    public required Creature Owner { get; init; }
    public CardModel? Card { get; init; }
    public RelicModel? Relic { get; init; }
    public PowerModel? Power { get; init; }

    public static implicit operator DynamicVarSource(CardModel card)
    {
        return new DynamicVarSource
        {
            DynamicVars = card.DynamicVars,
            Owner = card.Owner.Creature,
            Card = card
        };
    }

    public static implicit operator DynamicVarSource(RelicModel relic)
    {
        return new DynamicVarSource
        {
            DynamicVars = relic.DynamicVars,
            Owner = relic.Owner.Creature,
            Relic = relic
        };
    }

    public static implicit operator DynamicVarSource(PowerModel power)
    {
        return new DynamicVarSource
        {
            DynamicVars = power.DynamicVars,
            Owner = power.Owner,
            Power = power
        };
    }
}

public class TooltipSource
{
    private readonly Func<CardModel, IHoverTip> _makeTip;

    public TooltipSource(Func<CardModel, IHoverTip> tip)
    {
        _makeTip = tip;
    }

    public IHoverTip Tip(CardModel card)
    {
        return _makeTip(card);
    }

    public static implicit operator TooltipSource(Type type)
    {
        if (typeof(PowerModel).IsAssignableFrom(type))
        {
            return new TooltipSource(_ => HoverTipFactory.FromPower(ModelDb.GetById<PowerModel>(ModelDb.GetId(type))));
        }

        if (typeof(CardModel).IsAssignableFrom(type))
        {
            return new TooltipSource(_ => HoverTipFactory.FromCard(ModelDb.GetById<CardModel>(ModelDb.GetId(type))));
        }

        if (typeof(PotionModel).IsAssignableFrom(type))
        {
            return new TooltipSource(_ => HoverTipFactory.FromPotion(ModelDb.GetById<PotionModel>(ModelDb.GetId(type))));
        }

        if (typeof(EnchantmentModel).IsAssignableFrom(type))
        {
            return new TooltipSource(_ => ModelDb.GetById<EnchantmentModel>(ModelDb.GetId(type)).HoverTip);
        }

        throw new InvalidOperationException($"Unable to generate hovertip from type {type}.");
    }

    public static implicit operator TooltipSource(CardKeyword keyword)
    {
        return new TooltipSource(_ => HoverTipFactory.FromKeyword(keyword));
    }

    public static implicit operator TooltipSource(StaticHoverTip staticTip)
    {
        return new TooltipSource(_ => HoverTipFactory.Static(staticTip));
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class PoolAttribute(Type poolType) : Attribute
{
    public Type PoolType { get; } = poolType;
}

public interface IWeighted
{
    int Weight { get; }
}

public class WeightedList<T> : IList<T>
{
    private readonly List<WeightedItem> _items = new();
    private int _totalWeight;

    public T GetRandom(Rng rng)
    {
        return GetRandom(rng, remove: false);
    }

    public T GetRandom(Rng rng, bool remove)
    {
        if (_items.Count == 0)
        {
            throw new IndexOutOfRangeException("Attempted to roll on empty WeightedList.");
        }

        var roll = rng.NextInt(_totalWeight);
        var currentWeight = 0;

        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            if (currentWeight + item.Weight > roll)
            {
                if (remove)
                {
                    _items.RemoveAt(index);
                    _totalWeight -= item.Weight;
                }

                return item.Value;
            }

            currentWeight += item.Weight;
        }

        throw new InvalidOperationException($"Roll {roll} failed to get a value in list of total weight {_totalWeight}.");
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.Select(item => item.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        Add(item, item is IWeighted weighted ? weighted.Weight : 1);
    }

    public void Add(T item, int weight)
    {
        _items.Add(new WeightedItem(item, weight));
        _totalWeight += weight;
    }

    public void Clear()
    {
        _items.Clear();
        _totalWeight = 0;
    }

    public bool Contains(T item)
    {
        return _items.Any(entry => EqualityComparer<T>.Default.Equals(entry.Value, item));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.Select(item => item.Value).ToList().CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        for (var index = 0; index < _items.Count; index++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[index].Value, item))
            {
                return index;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        Insert(index, item, item is IWeighted weighted ? weighted.Weight : 1);
    }

    public void Insert(int index, T item, int weight)
    {
        _items.Insert(index, new WeightedItem(item, weight));
        _totalWeight += weight;
    }

    public void RemoveAt(int index)
    {
        var item = _items[index];
        _items.RemoveAt(index);
        _totalWeight -= item.Weight;
    }

    public T this[int index]
    {
        get => _items[index].Value;
        set => _items[index].Value = value;
    }

    private sealed class WeightedItem
    {
        public WeightedItem(T value, int weight)
        {
            Value = value;
            Weight = weight;
        }

        public int Weight { get; }
        public T Value { get; set; }
    }
}

public abstract class AncientOption(int weight) : IWeighted
{
    public int Weight { get; } = weight;

    public abstract IEnumerable<RelicModel> AllVariants { get; }
    public abstract RelicModel ModelForOption { get; }

    public static explicit operator AncientOption(RelicModel model)
    {
        return new BasicAncientOption(model, 1);
    }

    private sealed class BasicAncientOption(RelicModel model, int optionWeight) : AncientOption(optionWeight)
    {
        public override IEnumerable<RelicModel> AllVariants => [model.ToMutable()];
        public override RelicModel ModelForOption => model.ToMutable();
    }
}

public sealed class AncientOption<T>(int weight) : AncientOption(weight) where T : RelicModel
{
    public Func<T, RelicModel>? ModelPrep { get; init; }
    public Func<T, IEnumerable<RelicModel>>? Variants { get; init; }

    private readonly T _model = ModelDb.Relic<T>();

    public override IEnumerable<RelicModel> AllVariants => Variants == null ? [_model.ToMutable()] : Variants(_model);

    public override RelicModel ModelForOption => ModelPrep == null ? _model.ToMutable() : ModelPrep(_model.ToMutable() as T ?? _model);
}

public sealed class OptionPools
{
    private readonly WeightedList<AncientOption>[] _pools;

    public OptionPools(WeightedList<AncientOption> pool1, WeightedList<AncientOption> pool2, WeightedList<AncientOption> pool3)
    {
        _pools = [pool1, pool2, pool3];
    }

    public OptionPools(WeightedList<AncientOption> pool12, WeightedList<AncientOption> pool3)
    {
        _pools = [pool12, pool12, pool3];
    }

    public OptionPools(WeightedList<AncientOption> pool)
    {
        _pools = [pool, pool, pool];
    }

    public IEnumerable<AncientOption> AllOptions => _pools.SelectMany(pool => pool);

    public List<AncientOption> Roll(Rng rng)
    {
        var result = new List<AncientOption>();

        var currentPool = _pools[0];
        var rollPool = ClonePool(currentPool);
        result.Add(rollPool.GetRandom(rng, remove: true));

        if (!ReferenceEquals(currentPool, _pools[1]))
        {
            currentPool = _pools[1];
            rollPool = ClonePool(currentPool);
        }

        result.Add(rollPool.GetRandom(rng, remove: true));

        if (!ReferenceEquals(currentPool, _pools[2]))
        {
            currentPool = _pools[2];
            rollPool = ClonePool(currentPool);
        }

        result.Add(rollPool.GetRandom(rng, remove: true));
        return result;
    }

    private static WeightedList<AncientOption> ClonePool(WeightedList<AncientOption> source)
    {
        var clone = new WeightedList<AncientOption>();
        foreach (var option in source)
        {
            clone.Add(option);
        }

        return clone;
    }
}

public class SpireField<TKey, TVal> where TKey : class
{
    private sealed class Box(TVal? value)
    {
        public TVal? Value { get; } = value;
    }

    private readonly ConditionalWeakTable<TKey, Box> _table = new();
    private readonly Func<TKey, TVal?> _defaultValue;

    public SpireField(Func<TVal?> defaultValue)
        : this(_ => defaultValue())
    {
    }

    public SpireField(Func<TKey, TVal?> defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public TVal? this[TKey obj]
    {
        get => Get(obj);
        set => Set(obj, value);
    }

    public TVal? Get(TKey obj)
    {
        return _table.GetValue(obj, key => new Box(_defaultValue(key))).Value;
    }

    public void Set(TKey obj, TVal? value)
    {
        _table.Remove(obj);
        _table.Add(obj, new Box(value));
    }
}

internal static class SavedSpireFieldTypeSupport
{
    private static readonly HashSet<Type> SupportedTypes =
    [
        typeof(int),
        typeof(bool),
        typeof(string),
        typeof(ModelId),
        typeof(int[]),
        typeof(SerializableCard),
        typeof(SerializableCard[]),
        typeof(List<SerializableCard>)
    ];

    public static bool IsSupported(Type type)
    {
        return SupportedTypes.Contains(type) || type.IsEnum || (type.IsArray && type.GetElementType()?.IsEnum == true);
    }
}

internal interface ISavedSpireField
{
    string Name { get; }
    Type TargetType { get; }
    void Export(object model, SavedProperties props);
    void Import(object model, SavedProperties props);
}

public class SavedSpireField<TKey, TVal> : SpireField<TKey, TVal>, ISavedSpireField where TKey : class
{
    public SavedSpireField(Func<TVal?> defaultValue, string name)
        : this(_ => defaultValue(), name)
    {
    }

    public SavedSpireField(Func<TKey, TVal?> defaultValue, string name)
        : base(defaultValue)
    {
        Name = $"{typeof(TKey).Name}_{name}";
        if (!SavedSpireFieldTypeSupport.IsSupported(typeof(TVal)))
        {
            throw new NotSupportedException($"SavedSpireField {name} uses unsupported type {typeof(TVal).Name}.");
        }

        LegacySavedSpireFieldRuntime.Register(this);
    }

    public string Name { get; }
    public Type TargetType { get; } = typeof(TKey);

    public void Export(object model, SavedProperties props)
    {
        AddToProperties(props, Name, Get((TKey)model));
    }

    public void Import(object model, SavedProperties props)
    {
        if (TryGetFromProperties(props, Name, out TVal? value))
        {
            Set((TKey)model, value);
        }
    }

    private static void AddToProperties(SavedProperties props, string name, object? value)
    {
        switch (value)
        {
            case null:
                return;
            case int intValue:
                (props.ints ??= []).Add(new(name, intValue));
                break;
            case bool boolValue:
                (props.bools ??= []).Add(new(name, boolValue));
                break;
            case string stringValue:
                (props.strings ??= []).Add(new(name, stringValue));
                break;
            case Enum enumValue:
                (props.ints ??= []).Add(new(name, Convert.ToInt32(enumValue)));
                break;
            case ModelId modelId:
                (props.modelIds ??= []).Add(new(name, modelId));
                break;
            case SerializableCard card:
                (props.cards ??= []).Add(new(name, card));
                break;
            case int[] intArray:
                (props.intArrays ??= []).Add(new(name, intArray));
                break;
            case Enum[] enumArray:
                (props.intArrays ??= []).Add(new(name, enumArray.Select(Convert.ToInt32).ToArray()));
                break;
            case SerializableCard[] cardArray:
                (props.cardArrays ??= []).Add(new(name, cardArray));
                break;
            case List<SerializableCard> cardList:
                (props.cardArrays ??= []).Add(new(name, cardList.ToArray()));
                break;
        }
    }

    private static bool TryGetFromProperties<T>(SavedProperties props, string name, out T? value)
    {
        value = default;

        if (typeof(T) == typeof(int) || typeof(T).IsEnum)
        {
            var found = props.ints?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = typeof(T).IsEnum
                ? (T)Enum.ToObject(typeof(T), found.Value.value)
                : (T)(object)found.Value.value;
            return true;
        }

        if (typeof(T) == typeof(bool))
        {
            var found = props.bools?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = (T)(object)found.Value.value;
            return true;
        }

        if (typeof(T) == typeof(string))
        {
            var found = props.strings?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = (T)(object)found.Value.value;
            return true;
        }

        if (typeof(T) == typeof(ModelId))
        {
            var found = props.modelIds?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = (T)(object)found.Value.value;
            return true;
        }

        if (typeof(T) == typeof(int[]) || (typeof(T).IsArray && typeof(T).GetElementType()?.IsEnum == true))
        {
            var found = props.intArrays?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            if (typeof(T).IsArray && typeof(T).GetElementType()?.IsEnum == true)
            {
                var enumType = typeof(T).GetElementType()!;
                var enumArray = Array.CreateInstance(enumType, found.Value.value.Length);
                for (var index = 0; index < found.Value.value.Length; index++)
                {
                    enumArray.SetValue(Enum.ToObject(enumType, found.Value.value[index]), index);
                }

                value = (T)(object)enumArray;
            }
            else
            {
                value = (T)(object)found.Value.value;
            }

            return true;
        }

        if (typeof(T) == typeof(SerializableCard))
        {
            var found = props.cards?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = (T)(object)found.Value.value;
            return true;
        }

        if (typeof(T) == typeof(SerializableCard[]) || typeof(T) == typeof(List<SerializableCard>))
        {
            var found = props.cardArrays?.FirstOrDefault(item => item.name == name);
            if (found == null)
            {
                return false;
            }

            value = typeof(T) == typeof(List<SerializableCard>)
                ? (T)(object)found.Value.value.ToList()
                : (T)(object)found.Value.value;
            return true;
        }

        return false;
    }
}

internal static class LegacySavedSpireFieldRuntime
{
    private static readonly object SyncRoot = new();
    private static readonly List<ISavedSpireField> RegisteredFields = new();
    private static bool _initialized;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            DiscoverStaticFields();
            RegisteredFields.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));

            foreach (var field in RegisteredFields)
            {
                EnsureCacheEntry(field.Name);
            }

            _initialized = true;
        }
    }

    public static void Register(ISavedSpireField field)
    {
        lock (SyncRoot)
        {
            if (RegisteredFields.Any(existing => existing.Name == field.Name && existing.TargetType == field.TargetType))
            {
                return;
            }

            RegisteredFields.Add(field);
            if (_initialized)
            {
                RegisteredFields.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));
                EnsureCacheEntry(field.Name);
            }
        }
    }

    public static void Export(object model, SavedProperties props)
    {
        foreach (var field in GetFieldsForModel(model))
        {
            field.Export(model, props);
        }
    }

    public static void Import(object model, SavedProperties props)
    {
        foreach (var field in GetFieldsForModel(model))
        {
            field.Import(model, props);
        }
    }

    private static void DiscoverStaticFields()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var fieldType = field.FieldType;
                    if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(SavedSpireField<,>))
                    {
                        continue;
                    }

                    _ = field.GetValue(null);
                }
            }
        }
    }

    private static IEnumerable<ISavedSpireField> GetFieldsForModel(object model)
    {
        lock (SyncRoot)
        {
            return RegisteredFields.Where(field => field.TargetType.IsInstanceOfType(model)).ToArray();
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type != null).Cast<Type>();
        }
    }

    private static void EnsureCacheEntry(string name)
    {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        var propertyToIdField = typeof(SavedPropertiesTypeCache).GetField("_propertyNameToNetIdMap", flags);
        var idToPropertyField = typeof(SavedPropertiesTypeCache).GetField("_netIdToPropertyNameMap", flags);
        if (propertyToIdField?.GetValue(null) is not Dictionary<string, int> propertyToId ||
            idToPropertyField?.GetValue(null) is not List<string> idToProperty)
        {
            return;
        }

        if (propertyToId.ContainsKey(name))
        {
            return;
        }

        propertyToId[name] = idToProperty.Count;
        idToProperty.Add(name);

        var netIdBitSizeProperty = typeof(SavedPropertiesTypeCache).GetProperty("NetIdBitSize", flags);
        if (netIdBitSizeProperty == null || idToProperty.Count <= 1)
        {
            return;
        }

        var newBitSize = (int)Math.Ceiling(Math.Log(idToProperty.Count, 2));
        netIdBitSizeProperty.SetValue(null, newBitSize);
    }
}

public static class CommonActions
{
    public static AttackCommand CardAttack(CardModel card, CardPlay play, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        return CardAttack(card, play.Target, hitCount, vfx, sfx, tmpSfx);
    }

    public static AttackCommand CardAttack(CardModel card, Creature? target, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        if (card.DynamicVars.ContainsKey(CalculatedDamageVar.defaultName))
        {
            return CardAttack(card, target, card.DynamicVars.CalculatedDamage, hitCount, vfx, sfx, tmpSfx);
        }

        if (card.DynamicVars.ContainsKey(DamageVar.defaultName))
        {
            return CardAttack(card, target, card.DynamicVars.Damage.BaseValue, hitCount, vfx, sfx, tmpSfx);
        }

        throw new InvalidOperationException($"Card {card.Title} does not have a damage variable supported by CommonActions.CardAttack.");
    }

    public static AttackCommand CardAttack(CardModel card, Creature? target, decimal damage, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        AttackCommand command = DamageCmd.Attack(damage).WithHitCount(hitCount).FromCard(card);
        var combatState = card.CombatState;

        switch (card.TargetType)
        {
            case TargetType.AnyEnemy:
                if (target != null)
                {
                    command.Targeting(target);
                }
                break;
            case TargetType.AllEnemies:
                if (combatState != null)
                {
                    command.TargetingAllOpponents(combatState);
                }
                break;
            case TargetType.RandomEnemy:
                if (combatState != null)
                {
                    command.TargetingRandomOpponents(combatState);
                }
                break;
            default:
                throw new InvalidOperationException($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}.");
        }

        if (vfx != null || sfx != null || tmpSfx != null)
        {
            command.WithHitFx(vfx: vfx, sfx: sfx, tmpSfx: tmpSfx);
        }

        return command;
    }

    public static AttackCommand CardAttack(CardModel card, Creature? target, CalculatedDamageVar calculatedDamage, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        AttackCommand command = DamageCmd.Attack(calculatedDamage).WithHitCount(hitCount).FromCard(card);
        var combatState = card.CombatState;

        switch (card.TargetType)
        {
            case TargetType.AnyEnemy:
                if (target != null)
                {
                    command.Targeting(target);
                }
                break;
            case TargetType.AllEnemies:
                if (combatState != null)
                {
                    command.TargetingAllOpponents(combatState);
                }
                break;
            case TargetType.RandomEnemy:
                if (combatState != null)
                {
                    command.TargetingRandomOpponents(combatState);
                }
                break;
            default:
                throw new InvalidOperationException($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}.");
        }

        if (vfx != null || sfx != null || tmpSfx != null)
        {
            command.WithHitFx(vfx: vfx, sfx: sfx, tmpSfx: tmpSfx);
        }

        return command;
    }

    public static Task<decimal> CardBlock(CardModel card, CardPlay play)
    {
        return CardBlock(card, card.DynamicVars.Block, play);
    }

    public static Task<decimal> CardBlock(CardModel card, BlockVar blockVar, CardPlay play)
    {
        return CreatureCmd.GainBlock(card.Owner.Creature, blockVar, play);
    }

    public static Task<decimal> CardBlock(CardModel card, DynamicVar dynamicVar, CardPlay play, bool fast = false)
    {
        if (dynamicVar is CalculatedBlockVar calculated)
        {
            return CreatureCmd.GainBlock(card.Owner.Creature, calculated.Calculate(play.Target), calculated.Props, play, fast);
        }

        return CreatureCmd.GainBlock(card.Owner.Creature, dynamicVar.BaseValue, (dynamicVar as BlockVar)?.Props ?? ValueProp.Move, play, fast);
    }

    public static Task<IEnumerable<CardModel>> Draw(CardModel card, PlayerChoiceContext context)
    {
        return CardPileCmd.Draw(context, card.DynamicVars.Cards.BaseValue, card.Owner);
    }

    public static Task<T?> Apply<T>(Creature target, DynamicVarSource source, bool silent = false) where T : PowerModel
    {
        return PowerCmd.Apply<T>(target, source.DynamicVars.Power<T>().BaseValue, source.Owner, source.Card, silent);
    }

    public static Task<IReadOnlyList<T>> Apply<T>(IEnumerable<Creature> targets, DynamicVarSource source, bool silent = false) where T : PowerModel
    {
        return PowerCmd.Apply<T>(targets, source.DynamicVars.Power<T>().BaseValue, source.Owner, source.Card, silent);
    }

    public static Task<T?> Apply<T>(Creature target, CardModel card, bool silent = false) where T : PowerModel
    {
        return PowerCmd.Apply<T>(target, card.DynamicVars.Power<T>().BaseValue, card.Owner.Creature, card, silent);
    }

    public static Task<T?> Apply<T>(Creature target, CardModel? card, decimal amount, bool silent = false) where T : PowerModel
    {
        return PowerCmd.Apply<T>(target, amount, card?.Owner.Creature, card, silent);
    }

    public static Task<T?> ApplySelf<T>(CardModel card, bool silent = false) where T : PowerModel
    {
        return ApplySelf<T>(card, card.DynamicVars.Power<T>().BaseValue, silent);
    }

    public static Task<T?> ApplySelf<T>(CardModel card, decimal amount, bool silent = false) where T : PowerModel
    {
        return PowerCmd.Apply<T>(card.Owner.Creature, amount, card.Owner.Creature, card, silent);
    }

    public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int count = 1)
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, count);
        var pile = pileType.GetPile(card.Owner);
        var cards = pile.Cards;
        if (pile.Type == PileType.Draw)
        {
            cards = cards
                .OrderBy(model => model.Rarity)
                .ThenBy(model => model.Id)
                .ToList();
        }

        return await CardSelectCmd.FromSimpleGrid(context, cards, card.Owner, prefs);
    }

    public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int minCount, int maxCount)
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, minCount, maxCount);
        var pile = pileType.GetPile(card.Owner);
        var cards = pile.Cards;
        if (pile.Type == PileType.Draw)
        {
            cards = cards
                .OrderBy(model => model.Rarity)
                .ThenBy(model => model.Id)
                .ToList();
        }

        return await CardSelectCmd.FromSimpleGrid(context, cards, card.Owner, prefs);
    }

    public static async Task<CardModel?> SelectSingleCard(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType)
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, 1);
        var pile = pileType.GetPile(card.Owner);
        var cards = pile.Cards;
        if (pile.Type == PileType.Draw)
        {
            cards = cards
                .OrderBy(model => model.Rarity)
                .ThenBy(model => model.Id)
                .ToList();
        }

        return (await CardSelectCmd.FromSimpleGrid(context, cards, card.Owner, prefs)).FirstOrDefault();
    }
}

public static class LegacyNodeFactory
{
    private sealed record SceneRegistration(Type NodeType, Action<Node>? PostConversionAction);

    private static readonly ConcurrentDictionary<string, SceneRegistration> RegisteredScenes = new();
    private static readonly MethodInfo CreateFromSceneGeneric = typeof(RitsuGodotNodeFactories)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(method => method.Name == nameof(RitsuGodotNodeFactories.CreateFromScene) && method.IsGenericMethodDefinition);

    [ThreadStatic]
    private static int _suspendAutoConvertDepth;

    public static void Init()
    {
        LegacyMigrationSupport.Initialize();
    }

    public static TNode CreateFromScene<TNode>(string scenePath) where TNode : Node, new()
    {
        return RitsuGodotNodeFactories.CreateFromScenePath<TNode>(scenePath);
    }

    public static TNode CreateFromScenePath<TNode>(string scenePath) where TNode : Node, new()
    {
        return RitsuGodotNodeFactories.CreateFromScenePath<TNode>(scenePath);
    }

    public static TNode CreateFromScene<TNode>(PackedScene scene) where TNode : Node, new()
    {
        return RitsuGodotNodeFactories.CreateFromScene<TNode>(scene);
    }

    public static TNode CreateFromResource<TNode>(object resource) where TNode : Node, new()
    {
        return RitsuGodotNodeFactories.CreateFromResource<TNode>(resource);
    }

    public static void RegisterSceneType<TNode>(string scenePath, Action<TNode>? postConversionAction = null) where TNode : Node, new()
    {
        LegacyMigrationSupport.Initialize();

        var normalized = NormalizeScenePath(scenePath);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        RegisteredScenes[normalized] = new SceneRegistration(
            typeof(TNode),
            postConversionAction == null ? null : node => postConversionAction((TNode)node)
        );
    }

    public static void RegisterSceneType(string scenePath, Type nodeType)
    {
        LegacyMigrationSupport.Initialize();

        if (nodeType == null)
        {
            throw new ArgumentNullException(nameof(nodeType));
        }

        if (!typeof(Node).IsAssignableFrom(nodeType))
        {
            throw new ArgumentException($"Registered scene type must inherit {nameof(Node)}.", nameof(nodeType));
        }

        var normalized = NormalizeScenePath(scenePath);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        RegisteredScenes[normalized] = new SceneRegistration(nodeType, null);
    }

    public static bool IsRegistered(string scenePath)
    {
        return RegisteredScenes.ContainsKey(NormalizeScenePath(scenePath));
    }

    public static bool UnregisterSceneType(string scenePath)
    {
        return RegisteredScenes.TryRemove(NormalizeScenePath(scenePath), out _);
    }

    internal static bool TryConvertRegisteredScene(PackedScene scene, ref Node? result)
    {
        if (_suspendAutoConvertDepth > 0 || result == null)
        {
            return false;
        }

        var scenePath = NormalizeScenePath(scene.ResourcePath);
        if (string.IsNullOrWhiteSpace(scenePath) || !RegisteredScenes.TryGetValue(scenePath, out var registration))
        {
            return false;
        }

        if (registration.NodeType.IsInstanceOfType(result))
        {
            return false;
        }

        var original = result;
        try
        {
            _suspendAutoConvertDepth++;
            var converted = CreateFromSceneByType(registration.NodeType, scene);
            registration.PostConversionAction?.Invoke(converted);
            result = converted;
            original.QueueFree();
            return true;
        }
        finally
        {
            _suspendAutoConvertDepth--;
        }
    }

    private static Node CreateFromSceneByType(Type nodeType, PackedScene scene)
    {
        return (Node)CreateFromSceneGeneric.MakeGenericMethod(nodeType).Invoke(null, new object?[] { scene })!;
    }

    private static string NormalizeScenePath(string scenePath)
    {
        if (string.IsNullOrWhiteSpace(scenePath))
        {
            return string.Empty;
        }

        var normalized = scenePath.Trim().Replace('\\', '/');
        if (!normalized.StartsWith("res://", StringComparison.OrdinalIgnoreCase) &&
            !normalized.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
        {
            normalized = "res://" + normalized.TrimStart('/');
        }

        return normalized;
    }
}

public static class LegacyMigrationSupport
{
    private static readonly object SyncRoot = new();
    private static readonly HashSet<Type> InjectedSavedPropertyTypes = new();
    private static bool _initialized;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            LegacySavedSpireFieldRuntime.Initialize();

            var harmony = new Harmony("BaseLibToRitsu.Generated.Support");

            var instantiate = typeof(PackedScene).GetMethod("Instantiate", 0, new[] { typeof(PackedScene.GenEditState) });
            if (instantiate != null)
            {
                harmony.Patch(
                    instantiate,
                    postfix: new HarmonyMethod(typeof(LegacyMigrationSupport), nameof(OnPackedSceneInstantiate))
                );
            }

            var fromInternal = AccessTools.Method(typeof(SavedProperties), "FromInternal");
            if (fromInternal != null)
            {
                harmony.Patch(
                    fromInternal,
                    prefix: new HarmonyMethod(typeof(LegacyMigrationSupport), nameof(OnSavedPropertiesFromInternalPrefix)),
                    postfix: new HarmonyMethod(typeof(LegacyMigrationSupport), nameof(OnSavedPropertiesFromInternalPostfix))
                );
            }

            var fillInternal = AccessTools.Method(typeof(SavedProperties), "FillInternal");
            if (fillInternal != null)
            {
                harmony.Patch(
                    fillInternal,
                    postfix: new HarmonyMethod(typeof(LegacyMigrationSupport), nameof(OnSavedPropertiesFillInternalPostfix))
                );
            }

            _initialized = true;
        }
    }

    public static void OnPackedSceneInstantiate(PackedScene __instance, ref Node? __result)
    {
        LegacyNodeFactory.TryConvertRegisteredScene(__instance, ref __result);
    }

    public static void OnSavedPropertiesFromInternalPrefix(object model)
    {
        LegacySavedSpireFieldRuntime.Initialize();
        EnsureSavedPropertyTypeInjected(model);
    }

    public static void OnSavedPropertiesFromInternalPostfix(ref SavedProperties? __result, object model)
    {
        var props = __result ?? new SavedProperties();
        LegacySavedSpireFieldRuntime.Export(model, props);

        if (__result == null)
        {
            __result = props;
        }
    }

    public static void OnSavedPropertiesFillInternalPostfix(SavedProperties __instance, object model)
    {
        LegacySavedSpireFieldRuntime.Import(model, __instance);
    }

    private static void EnsureSavedPropertyTypeInjected(object model)
    {
        if (model is not AbstractModel abstractModel)
        {
            return;
        }

        var modelType = abstractModel.GetType();
        lock (SyncRoot)
        {
            if (InjectedSavedPropertyTypes.Contains(modelType))
            {
                return;
            }
        }

        var hasSavedProperty = modelType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(property => property.GetCustomAttribute<SavedPropertyAttribute>() != null);

        if (!hasSavedProperty)
        {
            return;
        }

        if (SavedPropertiesTypeCache.GetJsonPropertiesForType(modelType) == null)
        {
            SavedPropertiesTypeCache.InjectTypeIntoCache(modelType);
        }

        lock (SyncRoot)
        {
            InjectedSavedPropertyTypes.Add(modelType);
        }
    }
}
