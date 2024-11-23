using HarmonyLib;
using Il2CppPhoton.Pun;
using Il2CppWYD;

namespace WYDMod.Patches;

[HarmonyPatch(typeof(Battery))]
public class BatteryPatches
{
    [HarmonyPatch(nameof(Battery.AddPower))]
    [HarmonyPrefix]
    public static bool InfinitePower(ref Battery __instance, ref float power)
    {
        if (!ConfigManager.Instance.InfinitePower.Value) return true;
        
        var pv = __instance.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return true;
            
        __instance.maxPower = 10000f;
        __instance.currentPower = __instance.maxPower;
        
        return false;
    }
}