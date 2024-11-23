using System.Collections.Generic;
using Il2CppSystem;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppExitGames.Client.Photon.StructWrapping;
using Il2CppInterop.Runtime;
using MelonLoader;
using Il2CppWYD;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Type = System.Type;

[assembly: MelonInfo(typeof(WYDMod.WYDMod), "WYDMod", "1.0.0", "StupidRepo")]

namespace WYDMod;

// ReSharper disable once ClassNeverInstantiated.Global
public class WYDMod : MelonMod
{
    public Stats currentStats;

    private Rect windowRect = new(20, 20, 240, 100);
    private Vector2 scrollPosition;

    private List<Buff> buffs = new();
    private List<string> buffNames = new();

    private int selectedBuffIndex;
    private int buffTime = 3;

    public override void OnLateInitializeMelon()
    {
        SearchForBuffs();
    }

    public override void OnInitializeMelon()
    {
        ConfigManager.Instance = new ConfigManager();
    }

    public override void OnGUI()
    {
        base.OnGUI();
        if (buffs.Count == 0)
            return;

        windowRect = GUILayout.Window(0, windowRect, (GUI.WindowFunction)DrawWindow, "WYDMod");
    }

    private void DrawWindow(int windowID)
    {
        GUILayout.BeginVertical();

        if (currentStats == null)
        {
            GUILayout.Label("<b><color=red>No character stats found!</color></b>");
            // currentStats = Object.FindObjectOfType<Stats>();
        }
        else
        {
            GUILayout.Toggle(ConfigManager.Instance.UnlimitedEnergy.Value, "Unlimited Energy/Stamina");

            if (GUILayout.Button($"Kill Yourself ({ConfigManager.Instance.SuicideKey.Value})"))
                currentStats.myCharacter.Death();

            if (GUILayout.Button("Apply Selected Buff"))
                ApplySelectedBuff();

            // Object.FindObjectOfType<BuffManager>().gameObject.AddComponent<ChokingHazardBuff>().ApplyBuff();
        }

        // start vertical scroll
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition, GUILayout.Width(240), GUILayout.Height(70));

        selectedBuffIndex = GUILayout.Toolbar(selectedBuffIndex, buffNames.ToArray());

        GUILayout.EndScrollView();

        if (GUILayout.Button("Refresh"))
            SearchForBuffs();

        buffTime = Mathf.RoundToInt(GUILayout.HorizontalSlider(buffTime, 1f, 120f));
        GUILayout.Label($"<b><color=#68f2d4>Buff time: {buffTime}s!</color></b>");
        GUILayout.Label(
            $"<b><color=#6898f2>Selected buff: {buffNames[selectedBuffIndex]} ({buffs[selectedBuffIndex].GetType().Name})</color></b>");

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    public void SearchForBuffs()
    {
        LoggerInstance.Warning("Searching for Buffs...");
        
        buffs = Object.FindObjectsOfTypeIncludingAssets(Il2CppType.Of<Buff>()).Select(c => c.Cast<Buff>()).ToList();
        buffNames = buffs.Select(b => b.buffName).ToList();

        LoggerInstance.Msg($"Found {buffs.Count} Buffs!");
    }

    private void ApplySelectedBuff()
    {
        var buff = buffs[selectedBuffIndex];
        var buffManager = currentStats.myCharacter.buffManager;

        buffManager.AddBuff(buff, buffTime);
    }
}