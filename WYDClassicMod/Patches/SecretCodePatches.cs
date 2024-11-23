using HarmonyLib;
using UnityEngine;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(SecretCodes))]
public class SecretCodePatches
{
	// [HarmonyPatch(nameof(SecretCodes.SubmitCode))]
	// [HarmonyPrefix]
	// public static void SubmitCodePrefix(ref SecretCodes __instance, string code)
	// {
	// 	if (code.ToUpper() == "GOLDEN")
	// 	{
	// 		PlayerPrefs.SetInt("GoldenBaby", 1);
	// 		__instance.goldenBabyToggle.SetActive(true);
	// 		__instance.StartCoroutine(__instance.codeInfo.ActionDone("Unlocked Golden Baby"));
	// 	}
	// }
}