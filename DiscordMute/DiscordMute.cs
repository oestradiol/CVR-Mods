using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using BuildInfo = DiscordMute.BuildInfo;

[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: MelonInfo(typeof(DiscordMute.DiscordMute), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace DiscordMute;

public static class BuildInfo
{
    public const string Name = "DiscordMute";
    public const string Author = "Davi";
    public const string Version = "1.0.0";
}

public class DiscordMute : MelonMod
{
    internal static MelonLogger.Instance Logger;
    internal static MelonPreferences_Entry<List<byte>> MuteKey; // Todo: Check if MelonPrefs work for List<byte>

    public override void OnApplicationStart()
    {
        Logger = LoggerInstance;
        
        MelonPreferences.CreateCategory("DiscordMute");
        MuteKey = MelonPreferences.CreateEntry("DiscordMute", nameof(MuteKey), new List<byte>(), is_hidden: true);

        UiManager.OnApplicationStart();
    }
}