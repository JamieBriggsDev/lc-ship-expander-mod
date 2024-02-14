using System.IO;
using System.Reflection;
using BepInEx.Logging;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.Helper;

public abstract class UnityBundleHelper
{
    private static AssetBundle _shaderBundle;
    private static AssetBundle _prefabBundle;

    private static readonly string ScreenCutoutShaderName = "assets/shader/screencutoutshader.shader";
    private static readonly string DebugShaderName = "assets/shader/debugshader.shader";
    private static readonly string CameraContainerPrefabName    = "assets/prefabs/othercameracontainer.prefab";
    
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
            SELogger.Log("UnityBundleHelper", $"Loaded asset: {obj} of type {output.LoadAsset(obj).GetType()}"); 
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
        if (screenCutoutShader == null)
        {
            SELogger.Log("UnityBundleHelper", $"Failed to load asset {name} from additionalshaders");
        }
        return screenCutoutShader;
    }
    
    private static GameObject GetPrefab(string name)
    {
        if (_shaderBundle == null)
        {
            _prefabBundle = LoadResource("additionalprefabs");
        }

        var screenCutoutShader = _prefabBundle.LoadAsset<GameObject>(name);
        if (screenCutoutShader == null)
        {
            SELogger.Log("UnityBundleHelper", $"Failed to load asset {name} from additionalprefabs");
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
    
    public static GameObject GetCameraContainerPrefab()
    {
        return GetPrefab(CameraContainerPrefabName);
    }
}