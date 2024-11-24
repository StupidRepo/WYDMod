using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon;
using UnityEngine;
using WYDClassicMod.Managers;

namespace WYDClassicMod.Networking;

public class BradNet: PunBehaviour
{
    public const byte SpawnEvent = 72;
    
    public void Start()
    {
        MelonLogger.Msg("BradNet has started!");
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
}