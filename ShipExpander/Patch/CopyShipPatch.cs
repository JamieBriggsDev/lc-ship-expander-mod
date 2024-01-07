using GameNetcodeStuff;
using HarmonyLib;

namespace ShipExpander.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class CopyShipPatch
    {
        
        [HarmonyPostfix]
        static void changeBaseFOV()
        {
            
            
            
        }
    }
}