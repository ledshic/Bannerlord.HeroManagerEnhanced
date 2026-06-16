# Bannerlord.HeroManagerEnhanced

A quality-of-life mod for Mount & Blade II: Bannerlord combining three hero-focused features into a single module.

**Target**: Bannerlord 1.2.12+ (e1.2 branch) and later.

## Features (all configurable via MCM)

### 1. Auto Ammo Pickup
- Automatically collects battlefield ammo (arrows, bolts, stones) during combat.
- Configurable: enable/disable per ammo type, minimum stack threshold.

### 2. Perk Concierge (GimeAllPerks)
- Grants all perks to the player hero on demand.
- Force-apply button in MCM; optionally auto-apply on game load.

### 3. Adjustable Leveling
- Customizable XP multipliers for combat, skills, and smithing research.
- Independent sliders for player and companion rates.
- Smithing research rate configurable separately.

All features are independent toggles under separate MCM groups.

## Dependencies (load these **before** this mod)

- Bannerlord.Harmony
- Bannerlord.ButterLib (recommended)
- Bannerlord.UIExtenderEx (recommended)
- Bannerlord.MCM (Mod Configuration Menu) v5+

## Installation

1. Install the dependencies above (Workshop or Nexus).
2. Download the latest `Bannerlord.HeroManagerEnhanced-*.zip`.
3. Extract the `Bannerlord.HeroManagerEnhanced` folder into `Modules/`.
4. Enable in Launcher and place it **after** the MCM/Harmony entries in load order.
5. Start a campaign. Open **Mod Options** (ESC → Mod Options) to configure.

## Load Order (example)

1. Native
2. SandBoxCore
3. Sandbox
4. StoryMode (optional)
5. Bannerlord.Harmony
6. Bannerlord.ButterLib
7. Bannerlord.UIExtenderEx
8. Bannerlord.MBOptionScreen (MCM)
9. **Bannerlord.HeroManagerEnhanced**
10. Everything else

## Localization (l10n)

All setting names, group headers, hints, and descriptions are localized via `ModuleData/Languages/`.

**Included**:
- English (complete)
- 简体中文 (Simplified Chinese)

Additional languages: add a new folder under `ModuleData/Languages/<ISO>/` with `sta_strings.xml` + `language_data.xml` following the existing pattern.

## MCM Settings

After loading a campaign, go to **Mod Options**. Three separate setting groups:

- **Auto Ammo Pickup** — enable, per-type toggles, stack threshold
- **Perk Concierge** — enable, auto-apply on load, force-apply button
- **Adjustable Leveling** — enable, XP multipliers for player/companion/smithing

Settings are **global** (JSON) — not per-save.

## Building from Source (Unified Layout)

```
dev/
├── build.ps1
├── module/
│   ├── SubModule.xml          (uses __VERSION__)
│   └── ModuleData/Languages/...
└── src/
    └── Bannerlord.HeroManagerEnhanced/
        ├── Bannerlord.HeroManagerEnhanced.csproj
        ├── Features/
        │   ├── AutoAmmoPickup/
        │   ├── GimeAllPerks/
        │   └── AdjustableLeveling/
        └── SubModule.cs
```

From the mod root:

```powershell
# Windows
.\dev\build.ps1 -Version v1.0.0

# macOS / Linux (PowerShell Core)
pwsh ./dev/build.ps1 -Version v1.0.0
```

Outputs:
- `out/Bannerlord.HeroManagerEnhanced/` (ready module)
- `out/Bannerlord.HeroManagerEnhanced-v1.0.0.zip`

Configure `GameFolder` (or `GAMEFOLDER` env var) on the command line for your local Bannerlord install. This project uses `Krafs.Publicizer` + `Bannerlord.ReferenceAssemblies` for accessing internal game members at compile time.

## Development Notes

- Features are organized under `Features/<FeatureName>/` for clean separation.
- Uses `Krafs.Publicizer` to access internal TaleWorlds members without reflection, targeting `TaleWorlds.MountAndBlade`, `TaleWorlds.Core`, and `TaleWorlds.CampaignSystem`.
- `Lib.Harmony` is referenced directly (v2.3.6) for advanced patch scenarios.
- `AllowUnsafeBlocks` is enabled for low-level Harmony patching needs.
- Full multi-language scaffolding (EN primary + SC, with CN/CNs fallbacks).

## Credits

- Auto Ammo Pickup & Perk Concierge: [ledshic](https://github.com/ledshic)
- Adjustable Leveling: based on [muneeb-mashhood/MnB_AdjustableLeveling](https://github.com/muneeb-mashhood/MnB_AdjustableLeveling)
