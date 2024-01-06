using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ShipExpander
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ShipExpanderModBase : BaseUnityPlugin
    {

        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        
        private static ShipExpanderModBase Instance;

        internal ManualLogSource mls;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            _harmony.PatchAll(typeof(ShipExpanderModBase));
        }
        
    }
}
