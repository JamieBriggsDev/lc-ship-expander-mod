# Lethal Company - Ship Expander Mod

## Setup
In order to be able to download the correct NuGet dependencies,
a new source for BepInEx needs added to `NuGet.Config`.

![Add entry to NuGet.config](/Documents/NuGetConfig.png)

Following this, do a NuGet restore.

> This needs modifying to be more customizable per machine

Next open the file ./ShipExpander/copyDLL.bat and change the second
parameter to be where the Profile was setup for r2modman and LethalCompany
to allow the DDL to be automatically copied to the correct profile.