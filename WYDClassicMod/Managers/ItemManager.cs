using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using UnityEngine;
using WYDClassicMod.Networking;

namespace WYDClassicMod.Managers;

[HarmonyPatch(typeof(ItemSpawner))]
public static class ItemManager
{
    private static readonly List<Item> ItemRegistry = [];
    
    public static void RegisterNewItem(
        string name,
        GameObject prefab,
        Rarity rarity,
        int spawnChance
    )
    {
        var item = new Item
        {
            name = name,
            prefab = prefab,
            rarity = rarity,
            spawnChance = spawnChance
        };
        
        ItemRegistry.Add(item);
    }
    
    [CanBeNull]
    public static Item GetItemByName(string itemName)
    {
        return ItemRegistry.Find(e => e.name == itemName);
    }
    
    [HarmonyPatch(nameof(ItemSpawner.NetworkSpawnObjects))]
    [HarmonyPostfix]
    public static void NetworkSpawnObjects(ItemSpawner __instance)
    {
        NetworkSpawnCustom(__instance);
    }

    private static void NetworkSpawnCustom(ItemSpawner itemSpawner)
    {
        foreach (var item in ItemRegistry)
        {
            Vector3 position;
            switch (item.rarity)
            {
                case Rarity.Low:
                    position = itemSpawner.lowTierPos[Random.Range(0, itemSpawner.lowTierPos.Length)].transform.position;
                    break;
                case Rarity.Mid:
                    if(Random.Range(0, 100) >= item.spawnChance) continue;
                    position = itemSpawner.midTierPos[Random.Range(0, itemSpawner.midTierPos.Length)].transform.position;
                    break;
                case Rarity.Special:
                    if (Random.Range(0, 100) >= item.spawnChance) continue;
                    position = itemSpawner.specialPos[Random.Range(0, itemSpawner.specialPos.Length)].transform.position;
                    break;
                case Rarity.UpstairsOnly:
                    position = itemSpawner.upstairsDrawers[Random.Range(0, itemSpawner.upstairsDrawers.Length)].transform.position;
                    break;
                default:
                    MelonLogger.Error($"Unknown rarity: {item.rarity}");
                    continue;
            }

            BradRPC.RPC(nameof(BradRPC.SpawnCustomItemRPC), PhotonTargets.All, item.name, position);
        }
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