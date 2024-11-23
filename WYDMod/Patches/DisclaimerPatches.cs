using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace WYDMod.Patches;

[HarmonyPatch(typeof(Disclaimer))]
public class DisclaimerPatches
{
    [HarmonyPatch(nameof(Disclaimer.Awake))]
    [HarmonyPrefix]
    public static bool NoDisclaimerPatch(ref Disclaimer __instance)
    {
        if(ConfigManager.Instance.SkipDisclaimer.Value)
            Object.Destroy(__instance.gameObject);
        return !ConfigManager.Instance.SkipDisclaimer.Value;
    }
}