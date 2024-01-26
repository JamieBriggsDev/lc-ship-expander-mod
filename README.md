# Lethal Company - Ship Expander Mod

## Status
[![.NET](https://github.com/JamieBriggsDev/lc-ship-expander-mod/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JamieBriggsDev/lc-ship-expander-mod/actions/workflows/dotnet.yml)

## Setup
### Add BepInEx source to NuGet.Config
In order to be able to download the correct NuGet dependencies,
a new source for BepInEx needs added to `NuGet.Config`.

![Add entry to NuGet.config](/Documents/NuGetConfig.png)

Following this, do a NuGet restore.

### R2Modman setup for automatic DLL copying
As part of the dotnet build, if built in Visual Studio or Jetbrain's Rider, dotnet will run a 
script which copies the required dll and any supporting files to the users R2Modman profile. To do
this, two environment variables need to be set:
- $R2MODMAN_DIR (e.g. `C:\Projects\LethalCompanyMods\r2modmanPlus-local`)
- $R2MODMAN_LETHALCOMPANY_PROFILE (e.g. `MyTestProfile`. This is the profile which you use in R2Modman to test the dll is working as expected)

If developing on Windows, I recommend using Microsoft's [PowerToys](https://apps.microsoft.com/detail/XP89DCGQ3K6VLD?hl=en-US&gl=US) to do this. 
There is a tool to create profiles of environment variables which you can enable or disable at your own leisure.