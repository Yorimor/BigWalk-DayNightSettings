using HarmonyLib;
using UnityEngine;
using Enviro;

namespace DayNightSettings.Patches;

[HarmonyPatch(typeof(SkyManager))]
public class GameNetPatches
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void PatchUpdate(SkyManager __instance)
    {
        Plugin.CurrentTime.Value = Mathf.Round(SkyManager.GetCurrentTime() * 10.0f) * 0.1f;
    }
    
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void PatchAwake(SkyManager __instance)
    {
        Plugin.PauseDayNight.Value = false;
        Plugin.CurrentTime.Value = Mathf.Round(SkyManager.GetCurrentTime() * 10.0f) * 0.1f;
    }
}