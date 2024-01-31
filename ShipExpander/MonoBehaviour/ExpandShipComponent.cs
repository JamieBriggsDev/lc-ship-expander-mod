using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShipExpander.Builder;
using ShipExpander.Core;
using ShipExpander.Helper;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander.MonoBehaviour;

public class ExpandShipComponent : UnityEngine.MonoBehaviour
{
    private NetworkObject _thisNetworkObject;
    private GameObject _insideShip;
    private NetworkObject _insideShipNetworkObject;
    private GameObject _outsideShip;
    private NetworkObject _outsideShipNetworkObject;


    private void Start()
    {
        _thisNetworkObject = GetComponent<NetworkObject>();
        // Create two new containers and set the parents to this.
        SELogger.Log(gameObject, "Creating container for insideShip");
        _insideShip = new GameObjectBuilder().WithName("insideShip").WithParent(this.gameObject)
            .WithNetworkObjectComponent()
            .WithNetworkTransformComponent()
            .GetGameObject();
        _insideShipNetworkObject = _insideShip.GetComponent<NetworkObject>();
        SELogger.Log(gameObject, "Creating container for outsideShip");
        _outsideShip = new GameObjectBuilder().WithName("outsideShip").WithParent(this.gameObject)
            .WithNetworkObjectComponent()
            .WithNetworkTransformComponent()
            .GetGameObject();
        _outsideShipNetworkObject = _outsideShip.GetComponent<NetworkObject>();


        // Do remaining setup as a coroutine to allow objects to be spawned after a frame
        StartCoroutine(PostStartSetup());
    }

    IEnumerator PostStartSetup()
    {
        yield return new WaitForEndOfFrame();

        // Moving existing children of this gameobject to insideShip
        SELogger.Log(gameObject, "Moving existing children of this gameObject to insideShip");
        SELogger.Log(gameObject, $"There are {transform.childCount} in {gameObject.name}");

        while (gameObject.transform.childCount > 2)
        {
            foreach (Transform child in gameObject.transform)
            {
                SELogger.Log(gameObject, $"1: Moving object into insideShip: {child.gameObject.name}");
                // https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/networkobject-parenting/
                if (child.gameObject == _insideShip || child.gameObject == _outsideShip || child == null) continue;

                var childNetworkObject = child.gameObject.GetComponent<NetworkObject>();
                if (childNetworkObject != null)
                {
                    SELogger.Log(gameObject, $"2: (NE) Changing child.transform.parent: {child.gameObject.name}");
                    var trySetParent = childNetworkObject.TrySetParent(_insideShipNetworkObject.transform);

                    if (trySetParent) continue;

                    SELogger.Log(gameObject, $"Could not set parent for {child.gameObject.name}", LogLevel.Error);
                    child.transform.parent = _insideShipNetworkObject.transform;
                }
                else
                {
                    SELogger.Log(gameObject, $"2: (GO) Changing child.transform.parent: {child.gameObject.name}");
                    child.transform.parent = _insideShip.transform;
                }
            }

            yield return new WaitForSeconds(2);
        }

        // Some items are not found within the hangarShip gameObject. Get them now
        var specialGameObjectNames = new List<string>()
        {
            "ChangableSuit(Clone)"
        };
        foreach (var gameObjectToMove in specialGameObjectNames.Select(gameObjectName =>
                     GameObjectHelper.FindObjectByName(gameObjectName)))
        {
            gameObjectToMove.transform.parent = _insideShip.transform;
        }

        // Move inside ship up by 50
        TransformHelper.MoveObject(_insideShip, new Vector3(0f, ConstantVariables.InsideShipHeight, 0f));
        yield return new WaitForEndOfFrame();
    }
}