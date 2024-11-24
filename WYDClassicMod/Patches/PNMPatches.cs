using HarmonyLib;

namespace WYDClassicMod.Patches;

[HarmonyPatch(typeof(PhotonNetworkManager))]
public class PNMPatches
{
	[HarmonyPatch(nameof(PhotonNetworkManager.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(ref PhotonNetworkManager __instance)
	{
		PhotonNetwork.player.NickName = __instance.lobbyName;
		PhotonNetwork.player.CustomProperties["isModded"] = true;
	}
	
	[HarmonyPatch(nameof(PhotonNetworkManager.ChangeLobbyName))]
	[HarmonyPostfix]
	public static void ChangeLobbyNamePostfix(ref PhotonNetworkManager __instance)
	{
		PhotonNetwork.player.NickName = __instance.lobbyName;
	}
}