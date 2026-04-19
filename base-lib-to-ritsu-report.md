# BaseLib -> RitsuLib migration report

## Scope

- This converter removes the BaseLib project/package dependency and points the project to STS2-RitsuLib.
- It can generate project-local compatibility shims for commonly used BaseLib abstract/config/utils APIs.
- Migrated projects also get STS001/STS003 analyzer suppression because the stock analyzers do not understand the generated BaseLib compatibility surface.

## Dependency target

- Project root: D:\mod\ctf9\tools\yakumo-akai
- Old library root: D:\mod\ctf9\tools\BaseLib-StS2-master
- New library root: D:\mod\ctf9\tools\STS2-RitsuLib-main
- New project file: STS2-RitsuLib.csproj
- New manifest id: STS2-RitsuLib
- Safe C# rewrites enabled: True
- Legacy Harmony bootstrap requested: True
- Patch bootstrap rewrite enabled: True
- Migration support requested: True
- Migration support rewrite enabled: True
- Ritsu scaffold enabled: True
- Legacy Harmony bootstrap path: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyHarmonyPatchBootstrap.g.cs
- Migration support path: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyMigrationSupport.g.cs
- Compatibility support path: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyCompatibility.g.cs

## Changed files

- D:\mod\ctf9\tools\yakumo-akai\Ancientcard.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\build\vfx\CardTrailAkai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\build\vfx\Inner.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\build\vfx\Out.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\Akai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\AkaiKeyword.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\Ancinent.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\ancient\Gungnir.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\ancient\MarlboroGavel.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\basic\GodGungnir.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\basic\StrikeAkai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\AgniShine.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\FlanrdeWing.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\Leaf.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\LochNessFort.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\Mirror.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\OriginalFormOfSuwa.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\PagodaOfKunsamana.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\SaigyoujiTree.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\SealingMagicNeedle.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\SevenStarSword.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\Sleep.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\StoneFragments.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\TsubameGaeshi.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\common\YakumoClothes.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\BloodBat.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\DestroyRoar.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\DoubleBarrier.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\DreamBirth.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Fate.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\FriendOfDevil.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Gap.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\GatesOfMordor.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\GoldenDragonClaw.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Hillman.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\ImmortalWieldsEnergy.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\IronWheel.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Medice.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\PhilosopherStone.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\PhilosopherStonePirate.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Phoenix.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\QueenOfBubbles.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\RelyOn.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\SaintElmosFireColumn.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\Shackles.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\rare\YukaSmile.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Curse.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Ghost.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\KindSoul.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\ManiacHighspeedFlyer.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice1.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice2.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice3.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice4.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice5.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice6.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\Medice7.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\ReimuFlower.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\WeakMedice.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\White.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\special\YukaDress.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\BarrierFix.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\DeadAndLife.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\DivineGodIncantation.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Doulong.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\EightEyedEelGrill.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\EightTrugramsFurnace.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\FoldingFanOfSaigyouji.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\FxSword.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Fxtz.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\GodShose.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\GreatSage.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Hair.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\InfinteMaidShiv.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\KomeijiEye.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Lenlautneedle.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\LilyOfVengeance.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Maid.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Mimi.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Opp.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\PalmLeaf.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Peach.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\PeachMedice.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\PenglaiJadeBranch.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Pfp.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\StarlightAndGhostlySteps.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\Time.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\YukaAngry.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\card\uncommon\ZombieHat.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\blood.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\DivineGodIncantationPower.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Dreambirth.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Fire.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Ghostpower.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Guai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Kind.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Money.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\mp.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Nextmp.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Num.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Penglai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Rebirth.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Sm.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\stardex.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Zombiedex.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\power\Zombiestrength.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\relics\Aptx4869.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnline.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnlineBad.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\relics\SeventeenFoldingFan.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\YakumoAkaiCardPool.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\YakumoAkaiPotionPool.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\character\YakumoAkaiRelicPool.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyCompatibility.g.cs: generated BaseLib abstract/config compatibility support
- D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyHarmonyPatchBootstrap.g.cs: generated legacy Harmony patch bootstrap
- D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyMigrationSupport.g.cs: generated BaseLib utils migration support
- D:\mod\ctf9\tools\yakumo-akai\images\powers\yakumo_akai_power_god_incantation_power.png: added asset aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\images\relics\yakumo_akai_relic_folding_fan.png: added asset aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\images\relics\yakumo_akai_relic_online.png: added asset aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\images\relics\yakumo_akai_relic_online_bad.png: added asset aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\MyCustomModInitializer.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\orobas.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\path.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\ritsu-content-pack-scaffold.md: generated Ritsu content-pack scaffold
- D:\mod\ctf9\tools\yakumo-akai\scenes\vfx\CardTrailAkai.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\scenes\vfx\CustomCardTrail.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\src\Core\Nodes\Combat\AkaiEnergyCounter.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\src\Core\Nodes\Combat\CustomCreatureVisuals.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\src\Core\Nodes\Combat\CustomMegaLabel.cs: rewrote BaseLib.Utils usage to generated migration support
- D:\mod\ctf9\tools\yakumo-akai\YakumoAkai\localization\zhs\ancients.json: added localization aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\YakumoAkai\localization\zhs\cards.json: added localization aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\YakumoAkai\localization\zhs\characters.json: added localization aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\YakumoAkai\localization\zhs\powers.json: added localization aliases for Ritsu public entries
- D:\mod\ctf9\tools\yakumo-akai\YakumoAkai\localization\zhs\relics.json: added localization aliases for Ritsu public entries

## Generated scaffold

- Wrote Ritsu registration scaffold: D:\mod\ctf9\tools\yakumo-akai\ritsu-content-pack-scaffold.md
- The scaffold now includes a per-type content model inventory for the remaining BaseLib abstract classes.

## Generated migration support

- Wrote project-local migration support file: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyMigrationSupport.g.cs
- Rewritten C# files: 121
- Wrote project-local compatibility support file: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyCompatibility.g.cs
- Scope: BaseLib.Utils / NodeFactory wrappers, project-local SpireField support, BaseLib.Abstracts / BaseLib.Config compatibility shims, and runtime registration / asset patches.

## Legacy Harmony bootstrap

- Detected legacy Harmony patch classes: 3
- TryPatchAll call sites found before rewrite: 0
- TryPatchAll call sites rewritten: 0
- Wrote legacy Harmony bootstrap file: D:\mod\ctf9\tools\yakumo-akai\Generated\BaseLibToRitsu\LegacyHarmonyPatchBootstrap.g.cs
- YakumoAkai.character.relics.TransBaseRelic -> D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnlineBad.cs
- YakumoAkai.path.ModelDbAllRelicPoolsPatch -> D:\mod\ctf9\tools\yakumo-akai\path.cs
- YakumoAkai.TransBaseRelic -> D:\mod\ctf9\tools\yakumo-akai\orobas.cs

## Manual Harmony patch sites

- No direct harmony.Patch(...) sites were found.

## Remaining BaseLib code references

- No BaseLib C# usages were found.

## Migration buckets

### Config and settings

- Recommendation: Rewrite to ModConfig-STS2 or native Ritsu settings. RitsuLib does not expose BaseLib.Config as a drop-in API.
- No matches found.

### Patch bootstrap

- Recommendation: Prefer the generated legacy Harmony bootstrap for generic migration: replace TryPatchAll with CreateClassProcessor(...).Patch() calls per annotated class, then review any manual harmony.Patch(...) sites separately. Full Ritsu ModPatcher migration is still manual.
- No matches found.

### Godot node factories

- Recommendation: Prefer the generated LegacyNodeFactory wrappers for generic migration, or rewrite directly to STS2RitsuLib.Scaffolding.Godot.RitsuGodotNodeFactories.
- No matches found.

### Saved runtime fields

- Recommendation: Prefer the generated project-local SpireField/SavedSpireField helpers for generic migration, or replace them with another save strategy.
- No matches found.

### Content models and pool markers

- Recommendation: Rewrite BaseLib abstract models to vanilla or Ritsu-native model types, then register them through CreateContentPack / ModContentRegistry.
- Match count: 7
- D:\mod\ctf9\tools\yakumo-akai\character\Akai.cs:11 -> public class Akai : PlaceholderCharacterModel
- D:\mod\ctf9\tools\yakumo-akai\character\YakumoAkaiCardPool.cs:11 -> public class YakumoAkaiCardPool : CustomCardPoolModel
- D:\mod\ctf9\tools\yakumo-akai\character\YakumoAkaiCardPool.cs:28 -> // public override Texture2D? CustomFrame(CustomCardModel card)
- D:\mod\ctf9\tools\yakumo-akai\character\card\basic\GodGungnir.cs:24 -> [Pool(typeof(YakumoAkaiCardPool))]
- D:\mod\ctf9\tools\yakumo-akai\character\card\basic\GodGungnir.cs:25 -> public sealed class GodGungnir : CustomCardModel, ITranscendenceCard
- D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnlineBad.cs:32 -> [Pool(typeof(YakumoAkaiRelicPool))]
- D:\mod\ctf9\tools\yakumo-akai\character\relics\GensokyoOnlineBad.cs:33 -> public class GensokyoOnlineBad : CustomRelicModel

## Remaining documentation and manifest mentions

- D:\mod\ctf9\tools\yakumo-akai\build\BaseLib\BaseLib.json:2 -> "id": "BaseLib",
- D:\mod\ctf9\tools\yakumo-akai\build\BaseLib\BaseLib.json:3 -> "name": "BaseLib",
- D:\mod\ctf9\tools\yakumo-akai\libs\BaseLib.json:2 -> "id": "BaseLib",
- D:\mod\ctf9\tools\yakumo-akai\libs\BaseLib.json:3 -> "name": "BaseLib",

## How to run

.\tools\Convert-BaseLibToRitsuLib.ps1 -ProjectRoot "D:\mod\ctf9\tools\yakumo-akai" -Apply -RewriteSafeCode -RewritePatchBootstrap -GenerateMigrationSupport -RewriteMigrationSupportUsings -GenerateRitsuScaffold
