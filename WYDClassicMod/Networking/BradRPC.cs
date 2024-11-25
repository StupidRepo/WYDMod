using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon;
using UnityEngine;
using WYDClassicMod.Managers;
using MonoBehaviour = Photon.MonoBehaviour;

namespace WYDClassicMod.Networking;

public class BradRPC: MonoBehaviour
{
    private static PhotonView PV;
    
    public static BradRPC Me;
    
    public void Start()
    {
        MelonLogger.Msg("BradRPC has started!");
        
        PV = gameObject.AddComponent<PhotonView>();
        PV.viewID = PhotonNetwork.AllocateViewID();
        
        if(PV.isMine && Me == null)
            Me = this;
    }
    
    public static void RPC(string methodName, PhotonTargets targets, params object[] args)
    { PV.RPC(methodName, targets, args); }
    
    [PunRPC]
    public void SpawnCustomItemRPC(string itemName, Vector3 position)
    {
        var item = ItemManager.GetItemByName(itemName);
        if(item == null)
        {
            if (PhotonNetwork.isMasterClient) return;
            
            MelonLogger.Error($"Item \"{itemName}\" was not found in the item registry. Telling the host that there may be incompatibilities.");
            RPC("ItemNotFoundByClientRPC", PhotonTargets.MasterClient, itemName);

            return;
        }
        
        MelonLogger.Msg($"Spawning {itemName} at {position}!");
        
        var go = Instantiate(item.prefab, position, Quaternion.identity);
        go.name = item.name;
        go.layer = item.layer;
        go.tag = item.tag;
        
        var pv = go.AddComponent<PhotonView>();
        pv.viewID = PhotonNetwork.AllocateViewID();
        
        go.AddComponent<Rigidbody>();
        go.AddComponent<NetworkMovementRB>();
        go.AddComponent<PickUp>();

        item.components.ToList().ForEach(e => go.AddComponent(e));
    }
    
    [PunRPC]
    public void ItemNotFoundByClientRPC(string itemName, PhotonMessageInfo info)
    {
        MelonLogger.Error($"A player in your lobby tried to spawn an item that was not found in the item registry. (Item: \"{itemName}\")");
        MelonLogger.Error($"Player: {info.sender.CustomProperties}");
    }
}