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
> This needs modifying to be more customizable per machine

Next open the file ./ShipExpander/copyDLL.bat and change the second
parameter to be where the Profile was setup for r2modman and LethalCompany
to allow the DDL to be automatically copied to the correct profile.