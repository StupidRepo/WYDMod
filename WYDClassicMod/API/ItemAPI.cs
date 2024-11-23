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
        MelonLogger.Warning("Custom items are being registered now.");
        
        var spawner = __instance;
        
        var lowTier = spawner.lowTierObj.ToList();
        var midTier = spawner.midTierObj.ToList();
        var special = spawner.specialObj.ToList();
        var upstairs = spawner.upstairsObj.ToList();
        
        var midChance = spawner.chanceOfSpawnMid.ToList();
        var specialChance = spawner.specialSpawnChance.ToList();
        
        ItemsList.ForEach(item =>
        {
            switch (item.rarity)
            {
                case Rarity.Low:
                    lowTier.Add(item.prefab);
                    break;
                
                case Rarity.Mid:
                    midTier.Add(item.prefab);
                    midChance.Add(item.spawnChance);
                    break;
                
                case Rarity.Special:
                    special.Add(item.prefab);
                    specialChance.Add(item.spawnChance);
                    break;
                
                case Rarity.UpstairsOnly:
                    upstairs.Add(item.prefab);
                    break;
                
                default:
                    MelonLogger.Error($"Unknown rarity: {item.rarity}");
                    break;
            }
        });
        
        spawner.lowTierObj = lowTier.ToArray();
        spawner.midTierObj = midTier.ToArray();
        spawner.specialObj = special.ToArray();
        spawner.upstairsObj = upstairs.ToArray();
        
        spawner.chanceOfSpawnMid = midChance.ToArray();
        spawner.specialSpawnChance = specialChance.ToArray();
        
        MelonLogger.Warning("Custom items have been registered! Final list:");
        MelonLogger.Warning($"Low: {spawner.lowTierObj.Select(e => e.name).Join(delimiter: ",")}");
        MelonLogger.Warning($"Mid: {spawner.midTierObj.Select(e => e.name).Join(delimiter: ",")}");
        MelonLogger.Warning($"Special: {spawner.specialObj.Select(e => e.name).Join(delimiter: ",")}");
        MelonLogger.Warning($"UpstairsOnly: {spawner.upstairsObj.Select(e => e.name).Join(delimiter: ",")}");
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
    Special,
    UpstairsOnly
}