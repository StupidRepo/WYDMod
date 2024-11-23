using HarmonyLib;
using Il2CppPhoton.Pun;
using Il2CppWYD;
using MelonLoader;
using UnityEngine;

namespace WYDMod.Patches;

[HarmonyPatch(typeof(Stats))]
public class StatsPatches
{
    private static float DefaultEnergyUseMultiplier;
    
    [HarmonyPatch(nameof(Stats.Start))]
    [HarmonyPrefix]
    public static void StatsStart(ref Stats __instance)
    {
        DefaultEnergyUseMultiplier = 123f;
        Melon<WYDMod>.Instance.SearchForBuffs();
        
        Melon<WYDMod>.Logger.Msg("Setting currentStats");
        Melon<WYDMod>.Instance.currentStats = __instance;
    }
    
    [HarmonyPatch(nameof(Stats.Update))]
    [HarmonyPrefix]
    public static void StatsUpdate(ref Stats __instance)
    {
        if (Mathf.Approximately(DefaultEnergyUseMultiplier, 123f))
        {
            Melon<WYDMod>.Logger.Warning($"DefaultEnergyUseMultiplier is 123f! Changing to: {__instance.myCharacter.energyUseMultiplier}");
            DefaultEnergyUseMultiplier = __instance.myCharacter.energyUseMultiplier;
        }

        var useMult = DefaultEnergyUseMultiplier;
        if (ConfigManager.Instance.UnlimitedEnergy.Value)
            useMult = 0f;
        else if (ConfigManager.Instance.ShouldChangeRates.Value)
        {
            useMult = ConfigManager.Instance.EnergyDrainRate.Value;
            __instance.energyRechargeRate = ConfigManager.Instance.EnergyRechargeRate.Value;
        }
        
        __instance.myCharacter.energyUseMultiplier = useMult;
        
        if (Input.GetKeyDown(ConfigManager.Instance.RagdollKey.Value)) 
            __instance.myCharacter.ragdoll.PlayWithForce(2f, 3f * __instance.myCharacter.transform.forward);
        
        if(Input.GetKeyDown(ConfigManager.Instance.SuicideKey.Value))
            __instance.myCharacter.Death();
    }
}