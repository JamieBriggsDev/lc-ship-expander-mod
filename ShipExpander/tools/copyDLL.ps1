Write-Output "1:    Getting external variables"
$R2MODMAN_PROFILE_DIR="$Env:R2MODMAN_DIR\LethalCompany\profiles"
$R2MODMAN_PROFILE_NAME="$Env:R2MODMAN_LETHALCOMPANY_PROFILE"

if (-not $env:R2MODMAN_DIR -or -not $env:R2MODMAN_LETHALCOMPANY_PROFILE ) { 
    exit
}

Write-Host "$R2MODMAN_PROFILE_DIR/$R2MODMAN_PROFILE_NAME"

$PROFILE_BEPINEX_DIR = "$R2MODMAN_PROFILE_DIR\$R2MODMAN_PROFILE_NAME\BepInEx"

Write-Output "2:    Copying most up to date BepInEx.Debug tools"

Write-Output "Copying DebInEx.Debug.ScriptEngine"
Copy-Item "..\_downloads\expanded\ScriptEngine\BepInEx\plugins\" -Recurse -Destination "$PROFILE_BEPINEX_DIR\" -Force

Write-Output "3:    Copying ShipExpander DLL to plugins/scripts folder"

$DLL_NAME = "JBriggs.ShipExpander.dll"

Write-Output "Copying bin\Debug\netstandard2.1\$DLL_NAME"
Copy-Item "tools\config\com.bepis.bepinex.scriptengine.cfg" -Destination "$PROFILE_BEPINEX_DIR\config"
Copy-Item "tools\config\BepInEx.cfg" -Destination "$PROFILE_BEPINEX_DIR\config"
Copy-Item "bin\Debug\netstandard2.1\$DLL_NAME" -Destination "$PROFILE_BEPINEX_DIR\scripts" -Force
Copy-Item "bin\Debug\netstandard2.1\$DLL_NAME" -Destination "$PROFILE_BEPINEX_DIR\plugins" -Force

Write-Output "3:    Copying UnityBundles to plugins/scripts folder"
Copy-Item "AssetBundles\additionalshaders" -Destination "$PROFILE_BEPINEX_DIR\plugins" -Force
Copy-Item "AssetBundles\additionalshaders" -Destination "$PROFILE_BEPINEX_DIR\scripts" -Force
Copy-Item "AssetBundles\additionalprefabs" -Destination "$PROFILE_BEPINEX_DIR\scripts" -Force
Copy-Item "AssetBundles\additionalprefabs" -Destination "$PROFILE_BEPINEX_DIR\plugins" -Force
