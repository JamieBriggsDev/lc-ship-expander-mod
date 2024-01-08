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
            SELogger.Log($"Found {name} {gameObject.transform}");
            return gameObject;
        }

        SELogger.Log($"Could not find {name}", LogLevel.Error);
        throw new ShipExpanderException($"Could not find GameObject with name: {name}");
    }
}