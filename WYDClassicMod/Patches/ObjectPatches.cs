using System;
using HarmonyLib;
using MelonLoader;
using Object = UnityEngine.Object;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(Object))]
public class ObjectPatches
{
	// [HarmonyPatch(nameof(Object.Destroy), [typeof(Object), typeof(float)])]
	// [HarmonyPostfix]
	// public static void DestroyPostfix(Object obj, float t)
	// {
	// 	MelonLogger.Error($"WE JUST TRIED TO DESTROY: {obj.name}");
	// }
	//
	// [HarmonyPatch(nameof(Object.DestroyImmediate), [typeof(Object), typeof(bool)])]
	// [HarmonyPostfix]
	// public static void DestroyImmediatePostfix(Object obj, bool allowDestroyingAssets)
	// {
	// 	MelonLogger.Error($"WE JUST TRIED TO DESTROY: {obj.name}, {allowDestroyingAssets}");
	// }
}