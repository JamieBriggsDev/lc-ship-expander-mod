using System.IO;
using System.Reflection;
using BepInEx.Logging;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.Helper;

public abstract class UnityBundleHelper
{
    public static AssetBundle LoadResource(string unityPackageName)
    {
        // Load assets
        var sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var output = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, unityPackageName));
        if (output == null) {
            SELogger.Log("UnityBundleHelper", "Failed to load custom assets.", LogLevel.Error); 
        }

        return output;
    }
}