# Content Warning Tweaks

A mod with some quality-of-life changes for oxygen management and a better console for developers.
And more tweaks to come!

## Features
- **Oxygen on Revival:** Give the revived player set amount of oxygen
- **No Oxygen Loss When Dead:** Change oxygen consumption on dead players
- **Diving Bell Refill:** Refill oxygen when in diving bell
- **Configurable Oxygen:** Adjust how much oxygen you have
- **Console Improvements:** Enhancements for the in-game console

## Commands
*Install [TipeMod](https://thunderstore.io/c/content-warning/p/Tipe/TipeMod/) to use*
### Player
- **`.SetOxygen <player_name> <amount_of_oxygen>`**: Change a player's oxygen level

## For Developers: BetterConsole
If you're making mods and want a better console experience, you can reference this mod's assembly. This lets you use the `Tweaks.Features.BetterConsole.ConsoleCommandAttribute`

This attribute is a better alternative to `Zorro.Core.CLI.ConsoleCommandAttribute` because it handles:
- Automatic help command generation
- Optional arguments
- Command overloading

## Testing Note
I've tested this mod with two game instances (just me), but it *should* work fine with more players

## Credits
me.