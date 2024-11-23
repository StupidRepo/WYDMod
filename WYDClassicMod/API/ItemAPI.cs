using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace WYDClassicMod.API;

[HarmonyPatch(typeof(ItemSpawner))]
public static class ItemAPI
{
    private static readonly List<Item> ItemsList = [];
    
    public static void RegisterNewItem(
        string name,
        GameObject prefab,
        Rarity rarity,
        int spawnChance
    )
    {
        MelonLogger.Msg($"Registering: {name} ({prefab.name})...");
        
        var item = new Item
        {
            name = name,
            prefab = prefab,
            rarity = rarity,
            spawnChance = spawnChance
        };
        
        ItemsList.Add(item);
    }
    
    [HarmonyPatch(nameof(ItemSpawner.Start))]
    [HarmonyPrefix]
    private static void StartPatch(ref ItemSpawner __instance)
    {
        MelonLogger.Warning("Custom items are being registered now!! Any items registered after this will be IGNORED.");
        
        var spawner = __instance;
        ItemsList.ForEach(item =>
        {
            switch (item.rarity)
            {
                case Rarity.Low:
                    MelonLogger.Msg($"Item {item.name} has been registered as a Low rarity.");
                    spawner.lowTierObj.AddToArray(item.prefab);
                    break;
                
                case Rarity.Mid:
                    MelonLogger.Msg($"Item {item.name} has been registered as a Mid rarity.");
                    
                    spawner.midTierObj.AddToArray(item.prefab);
                    spawner.chanceOfSpawnMid.AddToArray(item.spawnChance);
                    break;
                
                case Rarity.Special:
                    MelonLogger.Msg($"Item {item.name} has been registered as a Special rarity.");
                    spawner.specialObj.AddToArray(item.prefab);
                    spawner.specialSpawnChance.AddToArray(item.spawnChance);
                    break;
                
                default:
                    MelonLogger.Warning($"Item {item.name} has an invalid rarity. Defaulting to Low.");
                    spawner.lowTierObj.AddToArray(item.prefab);
                    break;
            }
        });
    }
}

public class Item
{
    public string name;
    public GameObject prefab;
    public Rarity rarity;
    [Range(0, 100)] public int spawnChance;
}

public enum Rarity
{
    Low,
    Mid,
    Special
}