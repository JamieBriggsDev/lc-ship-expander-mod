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
            
        Core.SELogger.Log(gameObject, $"Plugin {PluginInfo.PLUGIN_GUID} is loaded by jbriggs.dev");
            
        //_harmony.PatchAll();
        _harmony.PatchAll(typeof(CopyShipPatch));
        _harmony.PatchAll(typeof(AutoParentToShipPatch));
        
    }

    // Keep this commented out, this for some reason disables the mod.
    /*private void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }*/
}
    
