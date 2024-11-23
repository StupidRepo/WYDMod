using System.Linq;
using JetBrains.Annotations;
using MelonLoader;
using UnityEngine;
using WYDClassicMod.API;
using WYDClassicMod.Networking;

[assembly: MelonInfo(typeof(WYDClassicMod.WYDClassicMod), "WYDClassicMod", "1.0.0", "StupidRepo")]
namespace WYDClassicMod;

public class WYDClassicMod : MelonMod
{
	[CanBeNull] public static BabyStats babyStats;
	public static DadPowerUps dadPowerUps;
	
	public static GameObject BRPC;
	public static GameObject BListener;

	private const float windowWidth = 280f;
	private Rect windowRect = new(20, 20, windowWidth, 75);
	
	private const float barHeight = 10f;
	
	private bool gameObjectsInitialized;

	// public override void OnInitializeMelon()
	// {
	// 	MelonLogger.Msg("Loading WYDClassicMod!");
	// 	ConfigManager.Instance = new ConfigManager();
	// 	
	// 	ItemAPI.RegisterNewItem("Test Item", new GameObject("Toaster"), Rarity.Low, 100);
	// }

	// TODO: We do initialisation in here because OnInitializeMelon doesn't exist in 0.4.x? Need to find a way around this.
	public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		if (gameObjectsInitialized) return;
		gameObjectsInitialized = true;
		
		MelonLogger.Msg("Loading WYDClassicMod!");
			
		BRPC = new GameObject("BradRPC");
		BRPC.AddComponent<BradRPC>();
		Object.DontDestroyOnLoad(BRPC);
		
		BListener = new GameObject("BradListener");
		BListener.AddComponent<BradListener>();
		Object.DontDestroyOnLoad(BListener);

		// MelonLogger.Msg(MelonMod.RegisteredMelons.Select(e => e.Info.Name));
		
		ConfigManager.Instance = new ConfigManager();
		
		ItemAPI.RegisterNewItem("Test Item", new GameObject("Toaster"), Rarity.Low, 100);
	}

	public override void OnGUI()
	{
		windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "WYDClassicMod");
	}
	
	void DoMyWindow(int windowID)
	{
		if (babyStats != null)
		{
			var clampedSickness = Mathf.Clamp(babyStats.sickness, 0f, 100f);
			var clampedDrowness = Mathf.Clamp(babyStats.drownness, 0f, 100f);
			var oneHundrededEnergy = babyStats.controlScript.energy * 100f;
			
			GUILayout.Label("<b>Baby Stats</b>");
			GUILayout.Label($"<color=#eb4242>HP: {100f - clampedSickness:F1}%</color>");
			GUILayout.Label($"<color=#4aeb42>Sickness: {clampedSickness:F1}%</color> <i>(Sickness Factor: {babyStats.sicknessFactor:F1})</i>");
			GUILayout.Label($"<color=#134ded>Oxygen: {100f - clampedDrowness:F1}%</color>");
			
			if (clampedSickness > 0f && babyStats.sicknessFactor > 1f)
			{
				var estimatedTimeToLive = (100f - clampedSickness) / babyStats.sicknessFactor;
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

		if (PhotonNetwork.inRoom && PhotonNetwork.room.CustomProperties.ContainsKey("isModded"))
			GUILayout.Label("<b>Lobby is modded!</b>");

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