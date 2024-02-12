Write-Output "##########    Copying Game DLLs   ##########"

$LETHAL_COMPANY_DIRECTORY = "F:\SteamLibrary\steamapps\common\Lethal Company"

$ASSEMBLY_CSHARP_DLL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Assembly-CSharp.dll"
$UNITY_ENGINE_DDL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\UnityEngine.dll"
$UNITY_ENGINE_NETWORK_RUNTIME_DDL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Unity.Netcode.Runtime.dll"
$UNITY_ENGINE_NETWORK_COMPONENTS_RUNTIME_DDL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Unity.Netcode.Components.dll"
$UNITY_ENGINE_HIGH_DEFINITION_RUNTIME_DLL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Unity.RenderPipelines.HighDefinition.Runtime.dll"

$DOWNLOADS_DIR = ".\_downloads"

Write-Output "Assembly-CSharp.dll location: $ASSEMBLY_CSHARP_DLL";
Write-Output "UnityEngine.dll location: $ASSEMBLY_CSHARP_DLL";
Write-Output "Unity.Netcode.Runtime.dll location: $UNITY_ENGINE_NETWORK_RUNTIME_DDL";
Write-Output "Unity.Netcode.Components.dll location: $UNITY_ENGINE_NETWORK_COMPONENTS_RUNTIME_DDL";
Write-Output "Unity.RenderPipelines.HighDefinition.Runtime.dll location: $UNITY_ENGINE_HIGH_DEFINITION_RUNTIME_DLL";


Copy-Item $ASSEMBLY_CSHARP_DLL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_DDL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_NETWORK_RUNTIME_DDL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_NETWORK_COMPONENTS_RUNTIME_DDL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_HIGH_DEFINITION_RUNTIME_DLL -Destination ".\externalDLLs"

Write-Output "##########    Initial setup               ##########"
Write-Output "Recreating downloads folder"
if(Test-Path -Path $DOWNLOADS_DIR){
    Remove-Item $DOWNLOADS_DIR -Force -Recurse
}
New-Item -Path $DOWNLOADS_DIR -ItemType "directory"

Write-Output "##########    Downloading tcli            ##########"

$THUNDERSTORE_CLI_VERSION = "0.2.3"
$THUNDERSTORE_CLI_DOWNLOAD_URL = "https://github.com/thunderstore-io/thunderstore-cli/releases/download/$THUNDERSTORE_CLI_VERSION/tcli-$THUNDERSTORE_CLI_VERSION-win-x64.zip";
$TCLI_DIR = "$DOWNLOADS_DIR\expanded\tcli"
$TCLI = "$TCLI_DIR\tcli"

# Remove directory if already existing
Write-Output "##########    Downloading Thunderstore CLI $THUNDERSTORE_CLI_VERSION  ###########"
Write-Output "Starting download..."
Invoke-WebRequest "$THUNDERSTORE_CLI_DOWNLOAD_URL" -OutFile "$DOWNLOADS_DIR\tcli.zip"
Write-Output "Finished downloading from $THUNDERSTORE_CLI_DOWNLOAD_URL"

Write-Output "##########    Extracting Thunderstore CLI $THUNDERSTORE_CLI_VERSION  ###########"
Write-Output "Starting extracting $DOWNLOADS_DIR\tcli.zip"
Expand-Archive "$DOWNLOADS_DIR\tcli.zip" -DestinationPath "$DOWNLOADS_DIR\expanded\tcli" -Force
Write-Output "Finished extracting to $DOWNLOADS_DIR\expanded\tcli"

Write-Output "##########    Downloading BepInEx.Debug   ##########"

$BEPINEX_DEBUG_RELEASE_VERSION = "r10";
$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL = "https://github.com/BepInEx/BepInEx.Debug/releases/download/$BEPINEX_DEBUG_RELEASE_VERSION"

$BEPINEX_CONSTRUCTOR_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/ConstructorProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_CTOR_SHOTGUN_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/CtorShotgun_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_DEMYSTIFY_EXCEPTIONS_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/DemystifyExceptions_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_MIRROR_INTERNAL_LOGS_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/MirrorInternalLogs_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_SCRIPT_ENGINE_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/ScriptEngine_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_SIMPLE_MONO_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/SimpleMonoProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"
$BEPINEX_STARTUP_PROFILER_DOWNLOAD = "$BEPINEX_DEBUG_TOOLS_DOWNLOAD_URL/StartupProfiler_$BEPINEX_DEBUG_RELEASE_VERSION.zip"


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