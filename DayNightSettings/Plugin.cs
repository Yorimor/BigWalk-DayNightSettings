using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mirror;
using ModSettingsMenu.Api;
using UnityEngine;

namespace DayNightSettings;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(ModSettingsMenu.PluginInfo.PLUGIN_GUID)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    internal static Harmony? Harmony { get; set; }
    
    public static ConfigEntry<bool> PauseDayNight;
    public static ConfigEntry<float> CurrentTime;

    public override void Load()
    {
        PauseDayNight = Config.Bind("General", "PauseDayNight", false, "Pause the Day/Night cycle");
        PauseDayNight.Value = false;

        PauseDayNight.SettingChanged += (sender, args) =>
        {
            if (NetworkManager.singleton.mode != NetworkManagerMode.Host) return;
            
            if (PauseDayNight.Value)
            {
                SkyManager.SetFixedTime();
            }
            else
            {
                SkyManager.ClearFixedTime();
            }
        };
        
        CurrentTime = Config.Bind("General", "CurrentTime", 10f, new ConfigDescription("Current in game time", new AcceptableValueRange<float>(0f, 24f)));
        CurrentTime.Value = 10f;

        CurrentTime.SettingChanged += (sender, args) =>
        {
            if (NetworkManager.singleton.mode != NetworkManagerMode.Host) return;
            SkyManager.SetFixedTime(CurrentTime.Value);
        };

        ModSettingsRegistry.Register(
            MyPluginInfo.PLUGIN_GUID,
            new ModSettingsModOptions
            {
                Name = "Day Night Settings",
                Description = "Settings for Day/Night in Big Walk",
                Author = "Yorimor",
                Version = MyPluginInfo.PLUGIN_VERSION,
                ThunderstoreTeam = "Yorimor",
                ThunderstoreModName = "DayNightSettings" 
            }
        );
        
        Patch();

        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Harmony.PatchAll();
    }

    internal static void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
}
