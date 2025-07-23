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
*Install [TipeMod](https://thunderstore.io/c/content-warning/p/Tipe/TipeMod/) to use*
### Player
- **`.SetRemainingOxygen <Player (Player)> <Value (Single)> [Percent (Boolean)]`**: Sets the remaining oxygen for the specified player
- **`.Heal <Player (Player)> <Value (Single)> [Percent (Boolean)]`** : Heals the specified player specified value
### Players
- **`.SetMaxHealth <Value (Single)>`**: Sets the max health for all players to the specified value

## For Developers: BetterConsole
If you're making mods and want a better console experience, you can reference this mod's assembly. This lets you use the `Tweaks.Features.BetterConsole.ConsoleCommandAttribute`

This attribute is a better alternative to `Zorro.Core.CLI.ConsoleCommandAttribute` because it handles:
- Automatic help command generation
- Optional arguments
- Command overloading

There is also a class `Tweaks.Features.BetterConsole.CommandsClass` to specifically enable or disable commands in a specific class

## Testing Note
I've tested this mod with two game instances (just me), but it *should* work fine with more players

## Credits
me.