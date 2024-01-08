using System;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using ShipExpander.Core;
using ShipExpander.Patch;
using UnityEngine;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class ShipExpanderModBase : BaseUnityPlugin
{

    private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        
    private static ShipExpanderModBase _instance;
        
    public static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        if (_instance == null)
        {
            _instance = this;
        }
            
        Core.SELogger.Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded by jbriggs.dev");
            
        //_harmony.PatchAll();
            
        _harmony.PatchAll(typeof(CopyShipPatch));
        _harmony.PatchAll(typeof(AutoParentToShipPatch));
    }
        
        
        

    /*private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }*/
}
    
/*[HarmonyDebug]
[HarmonyPatch(typeof(PlayerControllerB))] // I choose the PlayerControllerB script since it handles the movement of the player.
[HarmonyPatch("Update")] // I choose "Update" because it handles the movement for every frame
class infiniteSprint // my mod class
{
    private static ManualLogSource mls = new("infiniteSprint");
    [HarmonyPostfix] // I want the mod to run after the PlayerController Update void has executed
    static void Postfix(ref float ___sprintMeter) // the float sprintmeter handles the time left to sprint
    {

        ShipExpanderModBase.Log.LogDebug("Sprinting!");
        ___sprintMeter = 1f; // I set the sprintMeter to 1f (which if full) everytime the mod is run
    }
}*/