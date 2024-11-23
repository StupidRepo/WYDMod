# WYDMod
MelonLoader mods for the game **[Who's Your Daddy?!][wyd]**.

## WYDMod
Cheats for the WYD?! Remake. Supports config files.

### Requirements
> [!IMPORTANT]
> Requires latest MelonLoader.

### Features
- Skip Disclaimer Warning
- Unlimited Energy/Stamina
  - Changing of the specific stamina refill and loss rates
- Infinite Battery
- Ragdoll and Death Keybinds
- **All of the above is configurable!**

## WYDClassicMod
Mods for the Classic WYD?!

### Requirements
> [!IMPORTANT]
> Requires MelonLoader 0.4.x

### Features
- GUI that displays percentages of your baby stats (health, sickness, oxygen)

## Building
You will need a `WYD*Mod.csproj.user` file with the following contents:
### WYDMod
```csharp
<Project>
    <PropertyGroup>
        <DLLDir>Path to the DLLs of the Game (contains Scripts.dll)</DLLDir>
        <NET6Dir>Path to the IL2CPP DLLs (contains IL2CppInterop*.dll)</NET6Dir>
        <OurOutDir>Path to the MelonLoader Mods folder</OurOutDir>
    </PropertyGroup>
</Project>
```
### WYDClassicMod
```csharp
<Project>
    <PropertyGroup>
        <MLManaged>MelonLoader Managed Path</MLManaged>
        <NET6Dir>Path to the DLLs of the Game (WhosYourDaddy_Data/Managed)</NET6Dir>
        <OurOutDir>Path to the MelonLoader Mods folder</OurOutDir>
    </PropertyGroup>
</Project>
```

[wyd]: https://store.steampowered.com/app/427730/