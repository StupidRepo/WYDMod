using HarmonyLib;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(BabyStats))]
public class StatsPatches
{
	[HarmonyPatch(nameof(BabyStats.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(ref BabyStats __instance)
	{
		if (__instance.GetComponent<PhotonView>().isMine)
		{
			WYDClassicMod.babyStats = __instance;
		}
	}
}

[HarmonyPatch(typeof(DadPowerUps))]
public class DadPowerUpsPatches
{
	[HarmonyPatch(nameof(DadPowerUps.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(ref DadPowerUps __instance)
	{
		if (__instance.GetComponent<PhotonView>().isMine)
		{
			WYDClassicMod.dadPowerUps = __instance;
		}
	}
}