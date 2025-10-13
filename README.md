# Dub's Electrolysis

A RimWorld mod that adds electrolysis systems for hydrogen fuel cells and oxygen generation, integrating with Dub's Bad Hygiene.

[![RimWorld](https://img.shields.io/badge/RimWorld-1.5%20|%201.6-blue.svg)](https://rimworldgame.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## Features

- **Electrolysis Chambers** - Split water into hydrogen and oxygen
  - Small (500W), Medium (1500W), Large (3000W)
  - Automatic shutoff when storage full
  - Internal gas storage tanks

- **Hydrogen Fuel Cells** - Clean, efficient power generation
  - Small (1x2): 1200W output, 40 H2 storage
  - Large (2x2): 3000W output, 100 H2 storage
  - More efficient than chemfuel generators
  - Acts as battery + generator hybrid

- **Oxygen Pumps** - Life support for gravships (Odyssey DLC)
  - Optional integration (not required)
  - Provides O2 to enclosed spaces
  - Essential for space travel

- **Separate Pipe Networks**
  - O2 pipes (cyan/blue)
  - H2 pipes (red/orange)
  - Integrates with DBH water pipes

## How It Works

1. **Build Electrolysis Chamber** - Connects to water pipes, consumes power
2. **Split Water** - Produces H2 and O2, stores in internal tanks
3. **Distribute Gases** - Pipe hydrogen to fuel cells, oxygen to pumps
4. **Generate Power** - Fuel cells convert H2 to electricity efficiently
5. **Support Life** - Oxygen pumps supply breathable air to gravships

**Chemistry:** 2H₂O → 2H₂ + O₂ (realistic stoichiometry)

## Requirements

- **[Dub's Bad Hygiene](https://steamcommunity.com/sharedfiles/filedetails/?id=836308268)** (required)
- RimWorld 1.5 or 1.6
- **Odyssey DLC** (optional, for oxygen pump functionality)

## Installation

### Steam Workshop
1. Subscribe to this mod on Steam Workshop
2. Subscribe to [Dub's Bad Hygiene](https://steamcommunity.com/sharedfiles/filedetails/?id=836308268)
3. Load Dub's Bad Hygiene BEFORE Dub's Electrolysis
4. Start/continue your game

### Manual Installation
1. Download from [Releases](https://github.com/sh0tybumbati/dubs-electrolysis/releases)
2. Extract to `RimWorld/Mods/` folder
3. Enable in mod manager after Dub's Bad Hygiene

## Research & Costs

**Research:** Electrolysis
- Requires: Electricity + Microelectronics
- Cost: 4000 research points

**Buildings:**
- **Small Chamber:** 50 Steel, 2 Components (500W, 100 O2 / 200 H2 storage)
- **Medium Chamber:** 100 Steel, 4 Components (1500W, 300 O2 / 600 H2 storage)
- **Large Chamber:** 200 Steel, 8 Components (3000W, 600 O2 / 1200 H2 storage)
- **Small H2 Fuel Cell:** 75 Steel, 5 Components (1200W output, 40 H2 storage, 1x2)
- **Large H2 Fuel Cell:** 150 Steel, 8 Components (3000W output, 100 H2 storage, 2x2)
- **O2 Pump:** 40 Steel, 3 Components (100W consumption)

## Game Balance

**Power Generation:**
- H2 Fuel Cell: 1200W, ~30% more efficient than chemfuel per unit
- Acts as long-term energy storage
- Requires significant investment in electrolysis infrastructure

**Resource Flow:**
- Water → Electrolysis → H2 + O2
- H2 → Fuel Cells → Power
- O2 → Oxygen Pumps → Life Support

## Building from Source

### Prerequisites
- Visual Studio 2019+ or VS Code with C#
- .NET Framework 4.7.2
- RimWorld + Dub's Bad Hygiene installed

### Steps

1. **Clone repository:**
   ```bash
   git clone https://github.com/sh0tybumbati/dubs-electrolysis.git
   cd dubs-electrolysis
   ```

2. **Add reference DLLs to `lib/` folder:**
   - `Assembly-CSharp.dll` (from RimWorld)
   - `UnityEngine.CoreModule.dll` (from RimWorld)
   - `DubsBadHygiene.dll` (from DBH mod)

3. **Build:**
   ```bash
   cd Source/DubsElectrolysis
   msbuild DubsElectrolysis.csproj /p:Configuration=Release
   ```
   Or use Visual Studio Build Solution (F6)

4. **Install:**
   - DLL outputs to `Assemblies/DubsElectrolysis.dll`
   - Copy entire folder to `RimWorld/Mods/`

## Compatibility

- **Safe to add to existing saves:** Yes
- **Safe to remove:** Remove all electrolysis buildings first
- **Multiplayer:** Should work (untested)
- **Odyssey DLC:** Optional integration for oxygen pumps

## Known Issues

- Oxygen pumps don't currently integrate with Odyssey gravship systems (placeholder)
- Pipe network visualization needs custom textures
- Gas distribution logic may need balancing

## Future Plans

- Full Odyssey DLC gravship oxygen integration
- Custom textures for buildings and pipes
- Gas pressure/flow visualization
- Additional fuel cell sizes
- Chlorine production from saltwater electrolysis

## Contributing

Pull requests welcome! Areas for contribution:
- Odyssey DLC integration
- Custom textures
- Balance tweaking
- Bug fixes
- Translations

## License

MIT License - See [LICENSE](LICENSE) file

## Credits

- **Sh0tybumbati** - Mod creator
- **Dubwise** - Creator of [Dub's Bad Hygiene](https://github.com/Dubwise56/Dubs-Bad-Hygiene)

## Links

- **GitHub:** https://github.com/sh0tybumbati/dubs-electrolysis
- **Steam Workshop:** (coming soon)
- **Dub's Bad Hygiene:** https://steamcommunity.com/sharedfiles/filedetails/?id=836308268
