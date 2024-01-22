using System.IO;
using System.Reflection;
using BepInEx.Logging;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.Helper;

public abstract class UnityBundleHelper
{
    private static AssetBundle _shaderBundle;

    private static readonly string ScreenCutoutShaderName = "assets/shader/screencutoutshader.shader";
    private static readonly string DebugShaderName = "assets/shader/debugshader.shader";
    
    private static AssetBundle LoadResource(string unityPackageName)
    {
        // Load assets
        var sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var output = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, unityPackageName));
        if (output == null) {
            SELogger.Log("UnityBundleHelper", "Failed to load custom assets.", LogLevel.Error); 
        }

        foreach (var obj in output.GetAllAssetNames())
        {
            SELogger.Log("UnityBundleHelper", $"Loaded asset: ${obj}"); 
        }

        return output;
    }

    private static Shader GetShader(string name)
    {
        if (_shaderBundle == null)
        {
            _shaderBundle = LoadResource("additionalshaders");
        }

        var screenCutoutShader = _shaderBundle.LoadAsset<Shader>(name);
        //var screenCutoutShader = _shaderBundle.LoadAsset<Shader>("ScreenCutoutShader");
        if (screenCutoutShader == null)
        {
            SELogger.Log("UnityBundleHelper", "Failed to load asset ScreenCutoutShader");
        }
        return screenCutoutShader;
    }

    public static Shader GetScreenCutoutShader()
    {
        return GetShader(ScreenCutoutShaderName);
    }
    
    public static Shader GetDebugShader()
    {
        return GetShader(DebugShaderName);
    }
}