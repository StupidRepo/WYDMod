using MelonLoader;

namespace WYDClassicMod;

public class ConfigManager
{
	public MelonPreferences_Category GeneralCategory;

	public static ConfigManager Instance { get; set; }

	public ConfigManager()
	{
		MelonLogger.Msg("Loading config!");

		#region General
			GeneralCategory = MelonPreferences.CreateCategory("WYDClassicMod.General", "General Settings");
		#endregion
	}
}