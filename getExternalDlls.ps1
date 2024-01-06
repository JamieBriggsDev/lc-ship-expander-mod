$LETHAL_COMPANY_DIRECTORY = "F:\SteamLibrary\steamapps\common\Lethal Company"

$ASSEMBLY_CSHARP_DLL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\Assembly-CSharp.dll"
$UNITY_ENGINE_DDL = "$LETHAL_COMPANY_DIRECTORY\Lethal Company_Data\Managed\UnityEngine.dll"

echo "Assembly-CSharp.dll location: $ASSEMBLY_CSHARP_DLL";
echo "UnityEngine.dll location: $ASSEMBLY_CSHARP_DLL";


Copy-Item $ASSEMBLY_CSHARP_DLL -Destination ".\externalDLLs"
Copy-Item $UNITY_ENGINE_DDL -Destination ".\externalDLLs"