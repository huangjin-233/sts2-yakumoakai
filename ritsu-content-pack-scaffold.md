# Ritsu content-pack scaffold

- Generated from source patterns after removing the BaseLib project dependency.
- This scaffold does not recreate BaseLib APIs; it only proposes RitsuLib registration calls.
- You still need to rewrite BaseLib-based model classes to native game or Ritsu-compatible types before this code can compile.

## Suggested registration skeleton

```csharp
RitsuLibFramework.CreateContentPack("yakumo-akai")
    .Character<YakumoAkai.character.Akai>()
    .Card<YakumoAkai.character.YakumoAkaiCardPool, YakumoAkai.character.card.basic.GodGungnir>()
    .Relic<YakumoAkai.character.YakumoAkaiRelicPool, YakumoAkai.character.relics.GensokyoOnlineBad>()
    .Apply();

var content = RitsuLibFramework.GetContentRegistry("yakumo-akai");
// No custom monster classes were detected.
```

## Detected types

- Characters: 1
- Pool-bound cards: 1
- Pool-bound relics: 1
- Pool-bound potions: 0
- Powers: 0
- Monsters: 0
- Encounters: 0
- Ancients: 0

## Content model inventory

- These classes were detected as still inheriting BaseLib content abstractions or depending on BaseLib pool markers.
- Treat this as the rewrite backlog after dependency conversion and Harmony bootstrap migration.

### Shared Card Pools

- None detected.

### Shared Relic Pools

- None detected.

### Shared Potion Pools

- None detected.

### Characters

- YakumoAkai.character.Akai | base: PlaceholderCharacterModel | file: D:\mod\ctf9\tools\yakumo-akai\character\Akai.cs

### Cards

- YakumoAkai.character.card.basic.GodGungnir | base: CustomCardModel | pool: YakumoAkaiCardPool | file: D:\mod\ctf9\tools\yakumo-akai\character\card\basic\GodGungnir.cs

### Relics

- YakumoAkai.character.relics.GensokyoOnlineBad | base: CustomRelicModel | pool: YakumoAkaiRelicPool | file: D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnlineBad.cs

### Potions

- None detected.

### Powers

- None detected.

### Monsters

- None detected.

### Encounters

- None detected.

### Ancients

- None detected.

