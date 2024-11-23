using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(Crouch))]
public class CrouchPatches
{
	private static Vector3 beforeCamPos;
	
	[HarmonyPatch(nameof(Crouch.Update))]
	[HarmonyPrefix]
	public static void UpdatePrefix(ref Crouch __instance)
	{
		if (beforeCamPos.Equals(__instance.cam.position)) return;
		
		beforeCamPos = __instance.cam.position;
		MelonLogger.Warning("Crouch cam pos changed: " + beforeCamPos);
	}
}