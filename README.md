# Content Warning Tweaks

A mod with some quality-of-life changes for oxygen management and a better console for developers.
And more tweaks to come!

## Features
- **Oxygen on Revival:** Give the revived player set amount of oxygen
- **No Oxygen Loss When Dead:** Change oxygen consumption on dead players
- **Diving Bell Refill:** Refill oxygen when in diving bell
- **Configurable Oxygen:** Adjust how much oxygen you have
- **Return Items:** Returns any items that fall off the surface
- **Console Improvements:** Enhancements for the in-game console

## Commands
### Player
- **`.SetRemainingOxygen <Player (Player)> <Value (Single)> [Percent (Boolean)]`**: Sets the remaining oxygen for the specified player
- **`.Heal <Player (Player)> <Value (Single)> [Percent (Boolean)]`**: Heals the specified player specified value
- **`.SetThrowStrengthMultiplier <Player (Player)> <Multiplier (float)>`**: Sets the throw strength multiplier for the specified player
### Players
- **`.SetMaxHealth <Value (Single)>`**: Sets the max health for all players to the specified value

## For Developers
### Better Console
If you're making mods and want a better console experience, you can reference this mod's assembly. This lets you use the `Tweaks.Features.BetterConsole.ConsoleCommandAttribute`

This attribute is a better alternative to `Zorro.Core.CLI.ConsoleCommandAttribute` because it handles:
- Automatic help command generation
- Optional arguments
- Command overloading

There is also a class `Tweaks.Features.BetterConsole.CommandsClass` to specifically enable or disable commands in a specific class

### Harmony Patcher
If you are using harmony to patch your methods, you may also utilize the `Tweaks.Features.HarmonyPatcher` for easier patching of your methods. It removes all of the repeating and also has a way to automatically determine the type of the class you are trying to patch as well as the type of the patcher class

### Feature Manager
If you have a lot individual features in your mod, you can also try the `Tweaks.Features.FeatureManager`! It has a method `InitializeFeatures()` which must be called when the mod is loading.
Features must inherit the `Tweaks.Features.Feature` class and be marked with `Tweaks.Features.ModFeatureAttribute` for the FeatureManager to find them and register.
Feature manager automatically handles:
- Enabling and disabling individual features
- Has the option to mark a feature as required

## Testing Note
I've tested this mod with two game instances (just me), but it *should* work fine with more players

## Credits
me.