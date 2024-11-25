using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using UnityEngine;
using WYDClassicMod.Networking;
using Random = UnityEngine.Random;

namespace WYDClassicMod.Managers;

public static class AssetBundleManager
{
	private static readonly List<GameObject> loadedGameObjects = new();

	public static void LoadABFromEmbeddedResources(string name, Assembly assembly)
	{
		var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
		if (resourceName == null)
		{
			MelonLogger.Error($"Resource {name} not found in assembly {assembly.FullName}");
			return;
		}

		using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream == null)
		{
			MelonLogger.Error($"Failed to get stream for resource {resourceName}");
			return;
		}

		var buffer = new byte[stream.Length];
		stream.Read(buffer, 0, buffer.Length);

		var assetBundle = AssetBundle.LoadFromMemory(buffer);
		if (assetBundle == null)
		{
			MelonLogger.Error($"Failed to load AssetBundle from resource {resourceName}");
			return;
		}

		var assets = assetBundle.LoadAllAssets<GameObject>();
		loadedGameObjects.AddRange(assets);
		
		MelonLogger.Msg($"Loaded {assets.Length} assets from AssetBundle {name}!");
		MelonLogger.Msg($"Available assets: {string.Join(", ", assets.Select(go => go.name).ToArray())}");
	}

	[CanBeNull]
	public static GameObject GetGameObject(string name)
	{
		return loadedGameObjects.FirstOrDefault(go => go.name == name);
	}
}