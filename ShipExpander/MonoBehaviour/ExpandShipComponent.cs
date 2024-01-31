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

    private List<string> _shipPartsToCopyOutside = new List<string>()
    {
        "WallInsulator",
        "CatwalkShip"
    };

    private List<string> _shipPartsForJustOutside = new List<string>()
    {
        "Cameras"
    };


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

        // Move inside ship up by 50
        TransformHelper.MoveObject(_insideShip, ConstantVariables.InsideShipOffset);

        var findObjectByName = GameObjectHelper.FindObjectByName("StartGameLever");
        findObjectByName.transform.localPosition += ConstantVariables.InsideShipOffset;

        StartCoroutine(CopyObjectsForOutside());
    }

    private void LateUpdate()
    {
        FixInsideShipHierarchy();
    }

    private void FixInsideShipHierarchy()
    {
        // Moving existing children of this gameobject to insideShip
        foreach (Transform child in gameObject.transform)
        {
            // Should ignore if child has physics prop component as props should not belong to ship
            if (child.GetComponent<PhysicsProp>() != null ||
                _shipPartsForJustOutside.Contains(child.gameObject.name)) continue;

            if (child.gameObject == _insideShip || child.gameObject == _outsideShip || child == null) continue;

            SELogger.Log(gameObject, $"1: Moving object into insideShip: {child.gameObject.name}");

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

            if (child.gameObject.GetComponent<InsideShipComponent>() != null) continue;

            child.gameObject.AddComponent<InsideShipComponent>();
        }

        // Some items are not found within the hangarShip gameObject. Get them now
        var specialGameObjectNames = new List<string>()
        {
            "ChangableSuit(Clone)"
        };
        foreach (var gameObjectToMove in specialGameObjectNames.Select(GameObjectHelper.FindObjectByName))
        {
            if (gameObjectToMove.transform.parent != _insideShip.transform)
            {
                gameObjectToMove.transform.parent = _insideShip.transform;
            }
        }
    }

    private IEnumerator CopyObjectsForOutside()
    {
        while (_shipPartsToCopyOutside.Count > 0)
        {
            foreach (var gameObjectName in _shipPartsToCopyOutside)
            {
                SELogger.Log(this.gameObject, $"Copying {gameObjectName} to outsideShip");
                var findObjectByName = GameObjectHelper.FindObjectByName(gameObjectName);
                var insideShipComponent = findObjectByName.GetComponent<InsideShipComponent>();
                if (insideShipComponent != null)
                {
                    Destroy(insideShipComponent);
                }

                Instantiate(findObjectByName, _outsideShip.transform);

                _shipPartsToCopyOutside.Remove(gameObjectName);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}