using HarmonyLib;
using ShipExpander.Core;
using ShipExpander.Helper;
using ShipExpander.MonoBehaviour;

namespace ShipExpander.Patch;

#if DEBUG
[HarmonyDebug]
#endif
[HarmonyPatch(typeof(StartOfRound))]
public class CopyShipPatch
{
    private const string HangarShipGameObjectName = "HangarShip";

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    static void HarmonyPostFixAwake()
    {
        Core.SELogger.Log("CopyShipPatch",
            "Adding ExpandShipComponent to hangarShip");
        // Find ShipHangar object
        var shipHangar = GameObjectHelper.FindObjectByName(HangarShipGameObjectName);
        var expandShipComponent = shipHangar.AddComponent<ExpandShipComponent>();
        SELogger.Log(nameof(CopyShipPatch), "ExpandShipComponent added");
    }
        
}