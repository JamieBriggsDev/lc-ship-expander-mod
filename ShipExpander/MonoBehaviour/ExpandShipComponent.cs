using System;
using ShipExpander.Core;
using Unity.Netcode;
using UnityEngine;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander.MonoBehaviour;

public class ExpandShipComponent : UnityEngine.MonoBehaviour
{
    private GameObject _insideShip;
    private GameObject _outsideShip;
    
    private void Start()
    {
        // Create two new containers and set the parents to this.
        SELogger.Log(gameObject, "Creating container for insideShip");
        _insideShip = new GameObject("insideShip");
        SELogger.Log(gameObject, "Creating container for outsideShip");
        _outsideShip = new GameObject("outsideShip");
        var gameObjectTransform = this.gameObject.transform;
        SELogger.Log(gameObject, "Setting parent for insideShip and outsideShip to this");
        _insideShip.transform.parent = gameObjectTransform;
        _outsideShip.transform.parent = gameObjectTransform;

        SELogger.Log(gameObject, "Added NetworkObject component to this");
        _outsideShip.AddComponent<NetworkObject>();
        _insideShip.AddComponent<NetworkObject>();

        // Moving existing children of this gameobject to insideShip
        SELogger.Log(gameObject, "Moving existing children of this gameObject to insideShip");
        foreach (Transform child in transform)
        {
            // https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/networkobject-parenting/
            if (child != _insideShip.transform || child != _outsideShip.transform || child == null)
            {
                // TODO - Work out why this does not work
                //child.transform.parent = gameObject.transform;
                var networkObject = child.gameObject.GetComponent<NetworkObject>();
                var trySetParent = networkObject.TrySetParent(_insideShip.transform);
                if(!trySetParent)
                {
                    SELogger.Log(gameObject, $"Could not set parent for {child.gameObject.name}", LogLevel.Error);
                }
            }
        }
    }
    
}