using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using Photon;
using UnityEngine;
using WYDClassicMod.Managers;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace WYDClassicMod.Networking;

public class BradNet: PunBehaviour
{
    public void Start()
    {
        StartCoroutine(WaitUntilPlayerIsNotNull());
    }

    private IEnumerator WaitUntilPlayerIsNotNull()
    {
        while (PhotonNetwork.player == null)
            yield return null;
        
        MelonLogger.Msg($"BradNet has started! {PhotonNetwork.player}");
        
        var pv = gameObject.AddComponent<PhotonView>();
        pv.viewID = 202411;
    }
    
    public override void OnConnectedToPhoton()
    {
        MelonLogger.Msg("Photon connection established!");
    }

    public override void OnCreatedRoom()
    {
        UpdatePropertiesIfMaster();
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        UpdatePropertiesIfMaster();
    }

    // public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    // {
    //     if (!propertiesThatChanged.ContainsKey("isModded") || PhotonNetwork.room == null || PhotonNetwork.isMasterClient)
    //         return;
    //     
    //     var isModded = (bool) propertiesThatChanged["isModded"];
    //     var modList = (string[]) propertiesThatChanged["modList"];
    // }

    public void UpdatePropertiesIfMaster()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        
        PhotonNetwork.room.SetCustomProperties(new Hashtable {{"isModded", true}, {"modList", MelonHandler.Mods.Select(e => e.Info.Name).ToArray()}});
    }

    public string[] GetCurrentModList()
    {
        if(PhotonNetwork.room == null || !IsModded())
            return [];
        
        return (string[]) PhotonNetwork.room.CustomProperties["modList"];
    }
    
    public bool IsModded()
    {
        return PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey("isModded");
    }
    
    [PunRPC]
    public void SpawnCustomItemRPC(string itemName, Vector3 position)
    {
        var item = ItemManager.GetItemByName(itemName);
        if(item == null)
        {
            if (PhotonNetwork.isMasterClient) return;
            
            MelonLogger.Error($"Item \"{itemName}\" was not found in the item registry.");
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
}