using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace WYDMod.Patches;

[HarmonyPatch(typeof(Resources))]
public class TestPatches
{
    [HarmonyPatch(nameof(Resources.Load), typeof(string), typeof(Il2CppSystem.Type))]
    [HarmonyPostfix]
    public static void Resources_Load_Postfix(string path, Il2CppSystem.Type systemTypeInstance,
        ref UnityEngine.Object __result)
    {
        if(true)
            return;
#pragma warning disable CS0162 // Unreachable code detected
        Melon<WYDMod>.Logger.Warning($"=== {path} ===");
        Melon<WYDMod>.Logger.Msg($"Path: {path}");
        Melon<WYDMod>.Logger.Msg($"Type: {systemTypeInstance}");
        Melon<WYDMod>.Logger.Msg($"Result: {__result}");
#pragma warning restore CS0162 // Unreachable code detected
    }
}