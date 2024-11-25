using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MelonLoader;
using Photon;
using UnityEngine;
using WYDClassicMod.Items;
using WYDClassicMod.Managers;
using WYDClassicMod.Networking;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(WYDClassicMod.WYDClassicMod), "WYDClassicMod", "1.0.0", "StupidRepo")]
namespace WYDClassicMod;

public class WYDClassicMod : MelonMod
{
	[CanBeNull] public static BabyStats BabyStats;
	public static DadPowerUps DadPowerUps;
	
	public static BradNet BradNet;

	private const float windowWidth = 280f;
	private Rect windowRect = new(20, 20, windowWidth, 75);
	
	private const float barHeight = 10f;

	private bool hasLoadedNetworking;

	public override void OnApplicationStart()
	{
		MelonLogger.Msg("Loading WYDClassicMod!");
		ConfigManager.Instance = new ConfigManager();
	}

	public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		if (hasLoadedNetworking || sceneName != "Game") return;
		hasLoadedNetworking = true;
		
		var net = new GameObject("BradNetworking");
		var rpc = new GameObject("BradRPC");
		
		Object.DontDestroyOnLoad(rpc);
		rpc.hideFlags |= HideFlags.HideAndDontSave;
		
		BradNet = net.AddComponent<BradNet>();
		Object.DontDestroyOnLoad(net);
		
		// var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		// cube.name = "WYDClassicModCube";
		// cube.transform.localScale = new Vector3(2f, 2f, 2f);
		// cube.layer = LayerMask.NameToLayer("Grabable");
		// cube.tag = "Grab";
		
		// Object.DontDestroyOnLoad(cube);
		
		AssetBundleManager.LoadABFromEmbeddedResources("gun", Assembly.GetExecutingAssembly());

		ItemManager.RegisterNewItem("Gun", AssetBundleManager.GetGameObject("BGPrefab"), Rarity.Mid, 100, 
			LayerMask.NameToLayer("Grabable"), "Grab", typeof(Gun));
	}

	public override void OnGUI()
	{
		windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "WYDClassicMod");
	}

	private void DoMyWindow(int windowID)
	{
		if (BabyStats != null)
		{
			var clampedSickness = Mathf.Clamp(BabyStats.sickness, 0f, 100f);
			var clampedDrowness = Mathf.Clamp(BabyStats.drownness, 0f, 100f);
			var oneHundrededEnergy = BabyStats.controlScript.energy * 100f;
			
			GUILayout.Label("<b>Baby Stats</b>");
			GUILayout.Label($"<color=#eb4242>HP: {100f - clampedSickness:F1}%</color>");
			GUILayout.Label($"<color=#4aeb42>Sickness: {clampedSickness:F1}%</color> <i>(Sickness Factor: {BabyStats.sicknessFactor:F1})</i>");
			GUILayout.Label($"<color=#134ded>Oxygen: {100f - clampedDrowness:F1}%</color>");
			
			if (clampedSickness > 0f && BabyStats.sicknessFactor > 1f)
			{
				var estimatedTimeToLive = (100f - clampedSickness) / BabyStats.sicknessFactor;
				GUILayout.Label($"<b><color=#eb4242>You will die to sickness in {estimatedTimeToLive:F1}sec</color></b>");
			}
			
			var fullRect = GUILayoutUtility.GetRect(windowWidth, barHeight);
			
			var greenWidth = clampedSickness / 100f * windowWidth;
			var blueWidth = clampedDrowness / 100f * windowWidth;
			var grayWidth = oneHundrededEnergy / 100f * windowWidth;
			
			var healthRect = new Rect(fullRect.x, fullRect.y, fullRect.width, barHeight);
			var sicknessRect = new Rect(fullRect.x, fullRect.y, greenWidth, barHeight);
			var oxygenRect = new Rect(fullRect.x, fullRect.y, blueWidth, barHeight);

			GUI.DrawTexture(healthRect, MakeTex(2, 2, new Color(0.92f, 0.26f, 0.26f)));
			GUI.DrawTexture(sicknessRect, MakeTex(2, 2, new Color(0.29f, 0.92f, 0.26f)));
			GUI.DrawTexture(oxygenRect, MakeTex(2, 2, new Color(0.07f, 0.3f, 0.93f)));
			
			GUILayout.Label($"<color=gray>Stamina: {oneHundrededEnergy:F1}%</color>");
			
			var staminaFullRect = GUILayoutUtility.GetRect(windowWidth, barHeight);
			var staminaRect = new Rect(staminaFullRect.x, staminaFullRect.y, grayWidth, barHeight);

			GUI.DrawTexture(staminaRect, MakeTex(2, 2, new Color(.7f, .7f, .7f)));
		}
		else
			GUILayout.Label("<b><color=red>Your health and other stats will show up here when you're in-game!</color></b>");

		if (PhotonNetwork.inRoom)
		{
			GUILayout.Space(16f);
			
			GUILayout.Label("<b>Lobby</b>");
			GUILayout.Label($"Name: {PhotonNetwork.room.Name}");
			GUILayout.Label($"Players: {PhotonNetwork.playerList.Length}/{PhotonNetwork.room.MaxPlayers}");
			GUILayout.Label($"Ping: {PhotonNetwork.GetPing()}");
			
			if (BradNet.IsModded())
			{
				GUILayout.Space(8f);
				
				GUILayout.Label("<b><color=green>Host is Modded</color></b>");
				GUILayout.Label($"Mods: {string.Join(", ", BradNet.GetCurrentModList())}");
			}
			else
				GUILayout.Label("<color=red>Host is not Modded</color>");
			
			GUILayout.Space(8f);
				
			GUILayout.Label("<b>Players connected with mods</b>");
			foreach (var player in PhotonNetwork.playerList.Where(e => e.CustomProperties.ContainsKey("isModded")))
			{
				GUILayout.Label($"{player.NickName}");
			}
		}

		GUI.DragWindow();
	}
	
	private Texture2D MakeTex(int width, int height, Color col)
	{
		var pix = new Color[width * height];
		for (var i = 0; i < pix.Length; i++)
		{
			pix[i] = col;
		}
		var result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}
}