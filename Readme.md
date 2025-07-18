﻿## AntiCheat ACE Mod
An anti-cheat mod for ACE.

### Features:
- AntiBlink: Detects / prevents moving through closed doors (and monster doors).
- Admins / Cloaked characters are immune from AntiCheat

### Install:
- Download from the latest ACE.Mods.AntiCheat.zip from github [releases](https://github.com/trevis/ACE.Mods.AntiCheat/releases)
- Extract to `C:/ACE/Mods/` (or whatever your mod directory is.) (eg `C:/ACE/Mods/ACE.Mods.AntiCheat/ACE.Mods.AntiCheat.dll`)
- Modify Settings.json as needed

### Settings:
- **AdminsAreImmune**: Whether or not admins are immune to the anti-cheat
- **CloakedPlayersAreImmune**: Whether or not cloaked players are immune to the anti-cheat
- **EnableAntiBlink**: Enable anti-blink, which prevents players from teleporting through closed doors
- **AntiBlinkLogIntervalMilliseconds**: The number of milliseconds between anti-blink logging for a specific player
- **AntiBlinkMonsterDoors**: "Monster Doors" like Mana Barrier will be considered during the anti-blink check when enabled.
- **AntiBlinkZHeightLimit**: The height limit to check for doors when anti-blink is enabled

### TODO:
- Speed hack / other detection?
- AntiBlink currently doesn't work across landblock boundaries, so there's probably a couple edgecases it fails on.
- AntiBlink should be fast enough, but could use better filtering for doors to check against.
- Configurable way to deal with offenders. Right now it just logs.
- Only apply applicable harmony hooks optionally based on settings.
