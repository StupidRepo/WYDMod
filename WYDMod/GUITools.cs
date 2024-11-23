using System;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace WYDMod;

public class GUITools
{
    public static void DrawTextRow(string title, string text)
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label(title);
        GUILayout.FlexibleSpace();
        GUILayout.Label(text);
        
        GUILayout.EndHorizontal();
    }
}