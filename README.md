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

### Harmony Patcher
If you are using harmony to patch your methods, you may also utilize the `Tweaks.Features.HarmonyPatcher` for easier patching of your methods. It removes all of the repeating and also has a way to automatically determine the type of the class you are trying to patch as well as the type of the patcher class
Usage example:
```C#
using Tweaks.Features; // for the HarmonyPatcher

namespace PluginNamespace; // your plugin namespace

public static class YourGameClassPatch // name your pacther <ClassYouArePatching>Patch to utilize all of the features the patcher provides
{
    // if you are also using the FeatureManager, I strongly advise you to call the Init method from YourFeatureNameFeature.Initialize method
    internal static void Init() // the init method you call to apply the patches
    {
        YourPlugin.Patcher.SaveInfo(); // save the type of the current class and the type you are patching
                                       // this only works if you named your class with a suffix 'Patch'
                                       // if the method doesn't work on release build, try to use any of its overloads
                                       // if the class of the method you are patching is not inside the Assembly-CSharp.dll make sure you specify the correct assembly (without .dll extension)
                                       // if the class of the method you are patching is not inside the global namespace, make sure you specify the namespace of it (can be with or without the dot at the end)
                                       // if something still doesnt work and you cant figure out why, you can also use any of the overloads
        YourPlugin.Patcher.Patch(nameof(MethodYouAreTryingToPatch), // self explanatory
            prefix: nameof(Prefix)
        );

        // there are plenty of other ways to use this patcher. I just showed the simplest one. You can check the source code and figure out which overload is the best for you
    }

    public static bool Prefix() // this is just a normal Harmony prefix patch
    {
        return true;
    }
}

public class YourPlugin : BepInPlugin // this is only an example, not a full class
{
    public static HarmonyPatcher Patcher { get; private set; } = default!; // create the public variable and set it to default!
    private void Awake()
    {
        Patcher = new(PLUGIN_GUID, YourPlugin.Logger); // create the harmony patcher instance
    }
}
```

### Feature Manager
If you have a lot individual features in your mod, you can also try the `Tweaks.Features.FeatureManager`! It has a method `InitializeFeatures()` which must be called when the mod is loading.
Features must inherit the `Tweaks.Features.Feature` class and be marked with `Tweaks.Features.FeatureAttribute` for the FeatureManager to find them and register.
Feature manager automatically handles:
- Enabling and disabling individual features
- Has the option to mark a feature as required

Usage exaple:
```C#
using BepInEx;
using BepInEx.Configuration;
using Tweaks.Features; // for all of the necessary classes

namespace PluginNamespace; // your plugin namespace

[Feature] // add the FeatureAttribute
internal class YourFeatureNameFeature : Feature<YourFeatureNameFeature> // inherit the feature class
{
    public override BepInEx.Logging.ManualLogSource LogSource => YourPlugin.Logger; // use your logger
    public override bool Required => false; // (optional) if set to true, disallows the user to turn off the feature. The initialize method for it is always called. Default: false (user can disable your feature)
    public override string FeatureName => "YourFeatureName"; // name your feature. It can be with spaces or without
    public override string FeatureDescription => "Explain what your feature does";

    public override void Initialize()
    {
        // this function is only called if the feature is enabled
        // some code that initializes your feature. Put the SingletonNetworkComponent GameObject creating here
    }

    public override void CreateConfig(ConfigFile config)
    {
        // this function is always called. No matter if this feature is enabled or not
        // you can register your feature config here
    }
}

public class YourPlugin : BepInPlugin // this is only an example, not a full class
{
    private void Awake()
    {
        FeatureManager Manager = new(YourPlugin.Logger, Config); // initialize the feature manager
        Manager.RegisterFeaturesFromAssembly(); // find all of the features
        Manager.InitializeFeatures(); // initialize all of the features
    }
}
```

### NetworkComponents
**NetworkComponent<THandler, TParent>** transient abstract class for simpler networking.
Usage example:
```C#
using MyceliumNetworking;  // mycelium networking for CustomRPCAttribute
using Tweaks.Features;     // for NetworkComponent class
using UnityEngine;         // for this example to compile

namespace PluginNamespace; // your plugin namespace

//              your class name     inherit the NetworkComponent      the GameObject you are networking on
internal class YourNetworkHandler : NetworkComponent<YourNetworkHandler, Player>
{
    protected override uint MOD_ID => YourPlugin.MOD_ID; // your plugin MOD_ID. You can set it to any random number or you can use the provided hashing function with your PLUGIN_GUID
    //             in your mod you might not need to write the namespace of the ManualLogSource, but if you get an Ambiguous reference error, add it.
    protected override BepInEx.Logging.ManualLogSource LogSource => YourPlugin.Logger; // change this to your real logger

    [CustomRPC]
    // the custom RPC you define which is fired for the specific Player (in this case, it depends from TParent) when the SendOxygen calls the Send function
    public void SetOxygen(float oxygen)
    {
        if (ParentComponent == null || ParentComponent.data == null) return;

        // ParentComponent is of type Player (the type which is passed when inheriting the NetworkComponent (TParent)
        ParentComponent.data.remainingOxygen = Mathf.Clamp(oxygen, 0f, ParentComponent.data.maxOxygen); // for this example, we set the player oxygen to the given amount
    }
    public static void SendOxygen(Player targetPlayer, float oxygen)
    {
        // use the Send function to send the custom RPC
        Send(targetPlayer, nameof(SetOxygen), ReliableType.Reliable,
            oxygen // after the first 3 arguments, you can pass your custom arguments that will be available to the SetOxygen function
        );
    }
}
```

**SingletonNetwokHandler\<TParent>** singleton abstract class for simpler networking
Usage example:
```C#
using BepInEx;             // for BepInPlugin class
using MyceliumNetworking;  // mycelium networking for CustomRPCAttribute
using Tweaks.Features;     // for SingletonNetworkComponent class
using UnityEngine;         // for this example to compile

namespace PluginNamespace; // your plugin namespace

//              your class name     inherit the SingletonNetworkComponent
internal class YourNetworkHandler : SingletonNetworkComponent<YourNetworkHandler>
{
    protected override uint MOD_ID => YourPlugin.MOD_ID; // your plugin MOD_ID. You can set it to any random number or you can use the provided hashing function with your PLUGIN_GUID
    //             in your mod you might not need to write the namespace of the ManualLogSource, but if you get an Ambiguous reference error, add it.
    protected override BepInEx.Logging.ManualLogSource LogSource => YourPlugin.Logger; // change this to your real logger

    [CustomRPC]
    // the custom RPC you define which is fired once when the SendMaxHealth calls the Send function
    public void SetMaxHealth(float maxHealth)
    {
        Player.PlayerData.maxHealth = maxHealth; // sets the static maxHealth value for this example
        PlayerHandler.instance.playersAlive.ForEach(p => p.data.health = Mathf.Clamp(p.data.health, 0f, maxHealth)); // removes health if the player has more than the maxHealth
    }
    public static void SendMaxHealth(float maxHealth)
    {
        // use the Send function to send the custom RPC
        Send(nameof(SetMaxHealth), ReliableType.Reliable,
            maxHealth // after the first 2 arguments, you can pass your custom arguments that will be available to the SetOxygen function
        );
    }
}

public class YourPlugin : BepInPlugin // this is only an example, not a full class
{
    private void Awake()
    {
        new GameObject("YourNetworkHandler", typeof(YourNetworkHandler)); // you must register the game object for this network component to work
    }
}
```

Code to generate your MOD_ID:
```C#
using System.Security.Cryptography;
using System.Text;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        byte[] bytes = Encoding.UTF8.GetBytes("YOUR.PLUGIN.GUID"); // enter your mod plugin guid here and run this code. You can run this in a browser
        uint hash = 0x811c9dc5;
        foreach (byte b in bytes)
        {
            hash ^= b;
            hash *= 0x01000193;
        }
        Console.WriteLine(hash);
    }
}
```

## Testing Note
I've tested this mod with two game instances (just me), but it *should* work fine with more players

## Credits
me.