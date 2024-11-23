using MelonLoader;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace WYDMod;

public class ConfigManager
{
	public MelonPreferences_Category GeneralCategory;
	public MelonPreferences_Category CheatsCategory;
	public MelonPreferences_Category KeybindingsCategory;
	
	public MelonPreferences_Entry<bool> SkipDisclaimer;
	
	public MelonPreferences_Entry<bool> UnlimitedEnergy;

	public MelonPreferences_Entry<bool> ShouldChangeRates;
	public MelonPreferences_Entry<float> EnergyRechargeRate;
	public MelonPreferences_Entry<float> EnergyDrainRate;
	
	public MelonPreferences_Entry<bool> InfinitePower;
	
	public MelonPreferences_Entry<KeyCode> RagdollKey;
	public MelonPreferences_Entry<KeyCode> SuicideKey;
	
	public static ConfigManager Instance { get; set; }
	
	public ConfigManager()
	{
		Load();
	}

	private void Load()
	{
		Melon<WYDMod>.Logger.Msg("Loading config!");
		
		#region General
			GeneralCategory = MelonPreferences.CreateCategory("WYDMod.General", "General Settings");
		
			SkipDisclaimer = GeneralCategory.CreateEntry("SkipDisclaimer", true, "Skip Disclaimer", "Whether or not to skip the disclaimer when opening the game");
		#endregion

		#region Cheats
			CheatsCategory = MelonPreferences.CreateCategory("WYDMod.Cheats", "Cheats Settings");
		
			#region Stamina
				UnlimitedEnergy = CheatsCategory.CreateEntry("UnlimitedEnergy", true, "Unlimited Stamina", "Whether or not to have unlimited stamina");
						
				ShouldChangeRates = CheatsCategory.CreateEntry("ShouldChangeRates", false, "Should Change Stamina Rates?", "Whether or not to change the energy rates (if unlimited isn't enabled) - default values below are from the base game with 0 perks enabled");
				EnergyRechargeRate = CheatsCategory.CreateEntry("StaminaRechargeRate", 2f, "Stamina Recharge Rate", "The rate at which energy recharges");
				EnergyDrainRate = CheatsCategory.CreateEntry("StaminaDrainRate", 1f, "Stamina Drain Rate", "The rate at which energy drains");
			#endregion
			
			#region Power/Battery
				InfinitePower = CheatsCategory.CreateEntry("InfinitePower", true, "Infinite Power/Battery", "Whether or not to have infinite power/battery (some items require plugging in once to get infinite power)");
			#endregion
		#endregion
		
		#region Keybindings
			KeybindingsCategory = MelonPreferences.CreateCategory("WYDMod.Keybindings", "Keybindings Settings");
		
			RagdollKey = KeybindingsCategory.CreateEntry("RagdollKey", KeyCode.L, "Ragdoll Key", "The key to ragdoll your character");
			SuicideKey = KeybindingsCategory.CreateEntry("SuicideKey", KeyCode.Slash,"Suicide Key", "The key to kill your character");
		#endregion
	}
}