using System.Linq;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon;

namespace WYDClassicMod.Networking;

public class BradRPC: MonoBehaviour
{
    public void Start()
    {
        MelonLogger.Msg("BradRPC has started!");
        PhotonNetwork.OnEventCall += OnEvent;
    }
    
    public void RequestModList()
    {
        MelonLogger.Msg("Requesting mod list from server...");
        PhotonNetwork.RaiseEvent(0x7A, null, true, new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient});
    }
    
    public void OnEvent(byte eventCode, object content, int senderId)
    {
        if (eventCode != 0x7A)
            return;
        
        MelonLogger.Msg("Mod list was requested, sending back...");
        PhotonNetwork.RaiseEvent(0x7B, 
            new Hashtable { { "mods", new[] { MelonMod.RegisteredMelons.Select(e => e.Info.Name) } } }, 
            true,
            new RaiseEventOptions { TargetActors = [senderId] });
    }
}