﻿Write-Output "1:    Copying most up to date BepInEx.Debug tools"

$R2MODMAN_PROFILE_NAME = "ShipExpander"
$PROFILE_BEPINEX_DIR = "C:\Projects\LethalCompanyMods\r2modmanPlus-local\LethalCompany\profiles\$R2MODMAN_PROFILE_NAME\BepInEx"

Write-Output "Copying DebInEx.Debug.ScriptEngine"
Copy-Item "..\_downloads\expanded\ScriptEngine\BepInEx\plugins\" -Recurse -Destination "$PROFILE_BEPINEX_DIR\"

Write-Output "2:    Copying ShipExpander DLL to plugins/scripts folder"

Write-Output "Copying bin\Debug\netstandard2.1\JBriggs.ShipExpander.dll"
Copy-Item "tools\config\com.bepis.bepinex.scriptengine.cfg" -Destination "$PROFILE_BEPINEX_DIR\config"
Copy-Item "bin\Debug\netstandard2.1\JBriggs.ShipExpander.dll" -Destination "$PROFILE_BEPINEX_DIR\scripts"
Copy-Item "bin\Debug\netstandard2.1\JBriggs.ShipExpander.dll" -Destination "$PROFILE_BEPINEX_DIR\plugins"