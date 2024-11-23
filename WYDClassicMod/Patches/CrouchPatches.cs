using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(Crouch))]
public class CrouchPatches
{
	public static int btnLastPressed;
	public static int btnJustPressed;
	
	[HarmonyPatch(nameof(Crouch.Update))]
	[HarmonyPrefix]
	public static void UpdatePrefix(ref Crouch __instance)
	{
		// btnLastPressed = btnJustPressed;
		// if (__instance.btnDown)
		// {
		// 	if (btnLastPressed == 1 && __instance.goBack == false)
		// 	{
		// 		__instance.charCont.height = 2.8f;
		// 		__instance.goBack = true;
		// 	}
		// 	btnJustPressed = 1;
		// 	__instance.charCont.height = 1.4f;
		// 	__instance.charCont.center = Vector3.Lerp(__instance.charCont.center, new Vector3(0.0f, -0.6f, -0.28f), Time.deltaTime * 15f);
		// 	__instance.goBack = false;
		// }
		// else if (__instance.btn2Down)
		// {
		// 	if (btnLastPressed == 2 && __instance.goBack == false)
		// 	{
		// 		__instance.charCont.height = 2.8f;
		// 		__instance.goBack = true;
		// 	}
		// 	btnJustPressed = 2;
		// 	__instance.charCont.height = 1f;
		// 	__instance.charCont.center = Vector3.Lerp(__instance.charCont.center, new Vector3(0.0f, -0.8f, 0.9f), Time.deltaTime * 15f);
		// 	__instance.goBack = false;
		// }
		//
		// return false;
	}
}