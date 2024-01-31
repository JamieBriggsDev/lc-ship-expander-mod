Write-Output "##########    Copying Game DLLs   ##########"

$LETHAL_COMPANY_DIRECTORY = "F:\SteamLibrary\steamapps\common\Lethal Company"

$ASSEMBLY_CSHARP_DLL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Assembly-CSharp.dll"
$UNITY_ENGINE_DDL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\UnityEngine.dll"


Write-Output "Assembly-CSharp.dll location: $ASSEMBLY_CSHARP_DLL";
Write-Output "UnityEngine.dll location: $ASSEMBLY_CSHARP_DLL";


Copy-Item $ASSEMBLY_CSHARP_DLL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_DDL -Destination ".\externalDLLs"



Write-Output "##########    Downloading BepInEx.Debug   ##########"

$DOWNLOADS_DIR = ".\_downloads"

$BEPINEX_DEBUG_RELEASE_VERSION = "r10";
$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL = "https://github.com/BepInEx/BepInEx.Debug/releases/download/$BEPINEX_DEBUG_RELEASE_VERSION"

$BEPINEX_CONSTRUCTOR_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/ConstructorProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_CTOR_SHOTGUN_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/CtorShotgun_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_DEMYSTIFY_EXCEPTIONS_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/DemystifyExceptions_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_MIRROR_INTERNAL_LOGS_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/MirrorInternalLogs_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_SCRIPT_ENGINE_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/ScriptEngine_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_SIMPLE_MONO_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/SimpleMonoProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_STARTUP_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/StartupProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"

Write-Output "Recreating downloads folder"
if(Test-Path -Path $DOWNLOADS_DIR){
    Remove-Item $DOWNLOADS_DIR -Force -Recurse
}
New-Item -Path $DOWNLOADS_DIR -ItemType "directory"


Write-Output "Downloading BepInEx.Debug.ConstructorProfiler $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_CONSTRUCTOR_PROFILER_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_CONSTRUCTOR_PROFILER_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\ConstructorProfiler.zip"

Write-Output "Downloading BepInEx.Debug.CtorShotgun $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_CTOR_SHOTGUN_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_CTOR_SHOTGUN_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\CtorShotgun.zip"

Write-Output "Downloading BepInEx.Debug.DemystifyExceptions $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_DEMYSTIFY_EXCEPTIONS_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_DEMYSTIFY_EXCEPTIONS_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\DemystifyExceptions.zip"

Write-Output "Downloading BepInEx.Debug.MirrorInternalLogs $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_MIRROR_INTERNAL_LOGS_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_MIRROR_INTERNAL_LOGS_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\MirrorInternalLogs.zip"

Write-Output "Downloading BepInEx.Debug.ScriptEngine $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_SCRIPT_ENGINE_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_SCRIPT_ENGINE_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\ScriptEngine.zip"

Write-Output "Downloading BepInEx.Debug.SimpleMonoProfiler $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_SIMPLE_MONO_PROFILER_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_SIMPLE_MONO_PROFILER_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\SimpleMonoProfiler.zip"

Write-Output "Downloading BepInEx.Debug.StartupProfiler $BEPINEX_DEBUG_RELEASE_VERSION from $BEPINEX_STARTUP_PROFILER_DOWNLOAD"
Invoke-WebRequest "$BEPINEX_STARTUP_PROFILER_DOWNLOAD" -OutFile "$DOWNLOADS_DIR\StartupProfiler.zip"

Write-Output "##########    EXPANDING ARCHIVES   ##########"

Write-Output "Extracting $DOWNLOADS_DIR\ConstructorProfiler.zip"
Expand-Archive "$DOWNLOADS_DIR\ConstructorProfiler.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\ConstructorProfiler"

Write-Output "Extracting $DOWNLOADS_DIR\CtorShotgun.zip"
Expand-Archive "$DOWNLOADS_DIR\CtorShotgun.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\CtorShotgun"

Write-Output "Extracting $DOWNLOADS_DIR\DemystifyExceptions.zip"
Expand-Archive "$DOWNLOADS_DIR\DemystifyExceptions.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\DemystifyExceptions"

Write-Output "Extracting $DOWNLOADS_DIR\MirrorInternalLogs.zip"
Expand-Archive "$DOWNLOADS_DIR\MirrorInternalLogs.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\MirrorInternalLogs"

Write-Output "Extracting $DOWNLOADS_DIR\ScriptEngine.zip"
Expand-Archive "$DOWNLOADS_DIR\ScriptEngine.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\ScriptEngine"

Write-Output "Extracting $DOWNLOADS_DIR\SimpleMonoProfiler.zip"
Expand-Archive "$DOWNLOADS_DIR\SimpleMonoProfiler.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\SimpleMonoProfiler"

Write-Output "Extracting $DOWNLOADS_DIR\StartupProfiler.zip"
Expand-Archive "$DOWNLOADS_DIR\StartupProfiler.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\StartupProfiler"