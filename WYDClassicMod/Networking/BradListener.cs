using ExitGames.Client.Photon;
using MelonLoader;
using Photon;

namespace WYDClassicMod.Networking;

public class BradListener: PunBehaviour
{
    public void Start()
    {
        MelonLogger.Msg("BradListener has started!");
        PhotonNetwork.OnEventCall += OnEvent;
    }

    public override void OnConnectedToPhoton()
    {
        MelonLogger.Msg("Photon connection established!");
    }
    
    public override void OnCreatedRoom()
    {
        if(!PhotonNetwork.isMasterClient)
            return;
        
        MelonLogger.Msg("Room created, setting properties to say we are modded!");
        PhotonNetwork.room.SetCustomProperties(new Hashtable {{"isModded", true}});
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        
        MelonLogger.Msg("Master client switched to us! Setting properties to say we are modded.");
        PhotonNetwork.room.SetCustomProperties(new Hashtable {{"isModded", true}});
    }

    public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (!propertiesThatChanged.ContainsKey("isModded") || PhotonNetwork.room == null || PhotonNetwork.isMasterClient)
            return;
        
        var isModded = (bool) propertiesThatChanged["isModded"];
        MelonLogger.Msg($"Room properties changed! We are modded = {isModded}!");
        
        if (isModded)
            WYDClassicMod.BRPC.GetComponent<BradRPC>().RequestModList();
    }
    
    public void OnEvent(byte eventCode, object content, int senderId)
    {
        if (eventCode != 0x7B)
            return;
        
        MelonLogger.Msg("Mod list received!");
        
        var mods = (string[]) ((Hashtable) content)["mods"];
        MelonLogger.Msg($"Mods: {string.Join(", ", mods)}");
    }
}