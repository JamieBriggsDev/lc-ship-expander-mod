using GameNetcodeStuff;
using HarmonyLib;

namespace ShipExpander.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerFOVPatch
    {
        
        [HarmonyPostfix]
        static void changeBaseFOV()
        {
            
        }
    }
}