using ShipExpander.Core;
using UnityEngine;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander.Helper;

public abstract class GameObjectHelper
{
    public static GameObject FindObjectByName(string name)
    {
        var gameObject = GameObject.Find(name);
        if (gameObject != null)
        {
            return gameObject;
        }

        SELogger.Log("GameObjectHelper", $"Could not find {name}", LogLevel.Warning);
        return null;
    }
    
}