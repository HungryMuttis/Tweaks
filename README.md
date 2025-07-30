# Content Warning Tweaks

A mod with some quality-of-life changes for oxygen management and some good tools for developers.
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
- **`.Heal <Player (Player)> <Value (Single)> [Percent (Boolean) = false]`**: Heals the specified player specified value
- **`.SetMaxOxygen <Player (Player)> <Value (Single)> [Percent (Boolean) = false]`**: Sets the max oxygen for the specified player
- **`.SetRemainingOxygen <Player (Player)> <Value (Single)> [Percent (Boolean) = false]`**: Sets the remaining oxygen for the specified player
- **`.SetThrowStrengthMultiplier <Player (Player)> <Multiplier (float)>`**: Sets the throw strength multiplier for the specified player
### Players
- **`.Heal <Value (Single)> [Percent (Boolean) = false]`**: Heals all of the players the specified amount
- **`.SetMaxOxygen <Value (Single)> [Percent (Boolean) = false]`**: Sets the max oxygen for all of the players
- **`.SetRemainingOxygen <Value (Single)> [Percent (Boolean) = false]`**: Sets the remaining oxygen for all of the players
- **`.SetThrowStrengthMultiplier <Multiplier (float)>`**: Sets the throw strength multiplier for the specified player
- **`.SetMaxHealth <Value (Single)>`**: Sets max health for all players to the specified value

## For developers
### Better Console
If you're making mods and want a better console experience, you can reference this mod's assembly. This lets you use the `Tweaks.Features.BetterConsole.ConsoleCommandAttribute`

This attribute is a better alternative to `Zorro.Core.CLI.ConsoleCommandAttribute` because it handles:
- Automatic help command generation
- Optional arguments
- Command overloading

Usage example:
```C#
using Tweaks.Features.BetterConsole; // for all of the classes
using UnityEngine; // for Debug.Log

namespace PluginNamespace; // your plugin namespace

public class ExampleCommandsClass : ICommandsClass // the class can be named however you like. However, it will appear exactly like you name it in the console
                                                   // the inheriting of CommandsClass is optional, but it lets for the commands in the class to be enabled or disabled
{
    public static bool Enabled => true; // sets if the commands in this class are enabled.

    [ConsoleCommand("The description of what this function does", "The description of the first argument", "The description of the second argument")] // all the information about the function. If an arument description is set to "" (empty), then when the user is writing the argument, its name will not be changed
    public static void PrintEnteredText(string Text, int Times) // an example function
    {
        for (int i = 0; i < Times; i++)
        {
            Debug.Log(Text);
        }
    }
}
```

## Testing Note
I've tested this mod with two game instances (just me), but it *should* work fine with more players

## Credits
me.
