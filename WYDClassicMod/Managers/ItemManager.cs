using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using UnityEngine;
using WYDClassicMod.Networking;
using Random = UnityEngine.Random;

namespace WYDClassicMod.Managers;

[HarmonyPatch(typeof(ItemSpawner))]
public static class ItemManager
{
    private static readonly List<Item> ItemRegistry = [];
    
    public static void RegisterNewItem(
        string name,
        GameObject prefab,
        Rarity rarity,
        int spawnChance,
        int layer,
        string tag,
        params Type[] components
    )
    {
        var item = new Item
        {
            name = name,
            prefab = prefab,
            rarity = rarity,
            spawnChance = spawnChance,
            layer = layer,
            tag = tag,
            components = components
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
    public int layer;
    public string tag;
    public Type[] components;
}

public enum Rarity
{
    Low,
    Mid,
    Special,
    UpstairsOnly
}

public abstract class InteractableItem : MonoBehaviour
{
    public PhotonView netView;
    
    public void Start() =>
        netView = GetComponent<PhotonView>();
    
    public void Interact(Transform input)
    {
        if (!PhotonNetwork.connected)
            return;
        netView.RPC("RPCInteract", PhotonTargets.All, input.gameObject.name);
    }

    [PunRPC]
    public void RPCInteract(string interactedBy)
    {
        MelonLogger.Msg($"Interacted by {interactedBy}!");
    }
}

public abstract class PickupableItem : MonoBehaviour
{
    public PhotonView netView;
    public Rigidbody rb;
    public PhotonNetworkManager netManager;
    public GameObject actionText;
    public int startLayer;
    
    public Transform curHoldPos;
    
    public bool held;
    public bool lastPickupWasDad;
    
    public bool leftHandPickup;
    
    public virtual void Start()
    {
        netView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        netManager = GameObject.Find("NetworkManager").GetComponent<PhotonNetworkManager>();
        actionText = GameObject.Find("ActionText");
        startLayer = gameObject.layer;
    }
    
    public void Update()
    {
        if (!held) return;
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void ButtonDown()
    {
        MelonLogger.Msg("Shoot/fire!");
    }

    public void Interact(Transform input)
    {
        if (!PhotonNetwork.connected)
            return;
        netView.RPC("RPCInteract", PhotonTargets.All, input.gameObject.name);
    }

    [PunRPC]
    public void RPCInteract(string interactedBy)
    {
        MelonLogger.Msg($"Interacted by {interactedBy}!");
        HandlePickup(interactedBy);
    }

    private void HandlePickup(string interactedBy)
    {
        var player = GameObject.Find(interactedBy);
        if (player == null)
            return;
        
        if (!held)
        {
            lastPickupWasDad = player.gameObject.name.Substring(0, 3) == "Dad";
            curHoldPos = player.transform.FindDeepChild(player.gameObject.name.Substring(0, 3) == "Bab"
                ? (leftHandPickup ? "LeftBabyHoldPos" : "BabyHoldPos")
                : (leftHandPickup ? "LeftDadHoldPos" : "DadHoldPos"));
            
            player.gameObject.SendMessage(leftHandPickup ? "SetLeftItem" : "SetItem", gameObject.name);
        } else if (player.gameObject.name.Substring(0, 3) == "Dad" && netManager.curGameMode != 4)
        {
            transform.root.SendMessage(leftHandPickup ? "DropLeftItem" : "DropItem");
            
            if (transform.root.GetComponent<PhotonView>().isMine)
                actionText.SendMessage("ActionDone", "Your Daddy took the " + gameObject.name.Substring(0, gameObject.name.Length - 5));
            
            curHoldPos = transform.FindDeepChild(leftHandPickup ? "LeftDadHoldPos" : "DadHoldPos");
        }
    }
    
    public void Grab()
    {
        if (!PhotonNetwork.connected)
            return;
        
        MelonLogger.Msg("Grabbed!");
        netView.RPC("RPCGrab", PhotonTargets.All);
    }
    
    [PunRPC]
    public void RPCGrab()
    {
        GetComponent<NetworkMovementRB>().enabled = false;
        
        held = true;
        
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        transform.SetParent(curHoldPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    
    public void Drop(Vector3 input)
    {
        if (!PhotonNetwork.connected) return;
        
        MelonLogger.Msg("Dropped!");
        netView.RPC("RPCDrop", PhotonTargets.All, input);
    }


    [PunRPC]
    public void RPCDrop(Vector3 dropPos)
    {
        held = false;

        gameObject.layer = startLayer;
        
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;

        transform.parent = null;

        // ignore this for now because items have a chance to go in ground
        // if (dropPos != Vector3.zero)
        //     transform.position = dropPos;

        GetComponent<NetworkMovementRB>().enabled = true;
    }
}