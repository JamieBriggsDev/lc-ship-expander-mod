using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ShipExpander.Builder;
using ShipExpander.Core;
using ShipExpander.Helper;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Rendering;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander.MonoBehaviour;

public class ExpandShipComponent : UnityEngine.MonoBehaviour
{
    private NetworkObject _thisNetworkObject;
    private GameObject _insideShip;
    private NetworkObject _insideShipNetworkObject;
    private GameObject _outsideShip;
    private NetworkObject _outsideShipNetworkObject;

    private GameObject _insideShipTeleporter;
    private GameObject _outsideShipTeleporter;




    private List<string> _toIgnore = new()
    {
        "Player"
    };

    private List<string> _toBeCopiedOutside = new()
    {
        "WallInsulator",
        // Catwalk stuff should really be moved to _toIgnoreInside but leave for debugging
        "CatwalkRailLining",
        "CatwalkShip",
        "CatwalkUnderneathSupports",
        "ClimbOntoCatwalkHelper",
        "CatwalkRailLiningB",
        // End of catwalk stuff
        "ShipRails",
        "ShipModels2b", // Outside ship parts
        "ShipInside", // More outside ship parts, despite naming
    };

    private List<string> _toIgnoreInside = new()
    {
        "Cameras"
    };

    


    private void Start()
    {
        SELogger.Log(gameObject, "###############      Updated scripts");
        _thisNetworkObject = GetComponent<NetworkObject>();
        // Create two new containers and set the parents to this.
        SELogger.Log(gameObject, "Creating container for insideShip");
        _insideShip = new GameObjectBuilder().WithName("insideShip").WithParent(this.gameObject)
            .WithNetworkObjectComponent(ref _insideShipNetworkObject)
            .WithNetworkTransformComponent()
            .GetGameObject();

        SELogger.Log(gameObject, "Creating container for outsideShip");
        _outsideShip = new GameObjectBuilder().WithName("outsideShip").WithParent(this.gameObject)
            .WithNetworkObjectComponent(ref _outsideShipNetworkObject)
            .WithNetworkTransformComponent()
            .GetGameObject();
        SELogger.Log(gameObject, $"outsideShip spawned with local position of {_outsideShip.transform.localPosition}");

        // Move inside ship up by 50
        TransformHelper.MoveObject(_insideShip, ConstantVariables.InsideShipOffset);
        SELogger.Log(gameObject, $"insideShip updated with local position of {_insideShip.transform.localPosition}");

        // Move start game lever. For some reason in the FixInsideShipHierarchy method, this specific
        //  GameObject does not want to move to the new insideShip location so doing it here specifically.
        List<string> objectOverrides = new()
        {
            "StartGameLever"
        };
        foreach (var objectOverride in objectOverrides)
        {
            var findObjectByName = GameObjectHelper.FindObjectByName(objectOverride);
            SELogger.Log(gameObject, $"Moving niche object {findObjectByName.name}");
            TransformHelper.MoveObject(findObjectByName, ConstantVariables.InsideShipOffset);
        }
        /*
            findObjectByName.transform.localPosition += ConstantVariables.InsideShipOffset;
        */


        SELogger.Log(gameObject, "Setting up teleport");
        StartCoroutine(SetupTeleport());
    }


    private void LateUpdate()
    {
        FixInsideShipHierarchy();
        CopyObjectsForOutside();
    }

    private void FixInsideShipHierarchy()
    {
        // Moving existing children of this gameobject to insideShip
        foreach (Transform child in gameObject.transform)
        {
            var childGameObject = child.gameObject;

            // Should ignore if child has physics prop component as props should not belong to ship
            if (child.GetComponent<PhysicsProp>() != null ||
                _toIgnoreInside.Contains(childGameObject.name) || _toIgnore.Contains(childGameObject.name)) continue;
            // Should ignore if child is helper container object for this mod or null
            if (childGameObject == _insideShip || childGameObject == _outsideShip || child == null) continue;

            //SELogger.Log(gameObject, $"1: Moving object into insideShip: {child.gameObject.name}");

            var childNetworkObject = childGameObject.GetComponent<NetworkObject>();
            //ParentHelper.SetParent(childGameObject, _insideShip);
            if (childNetworkObject != null)
            {
                //SELogger.Log(gameObject, $"2: (NE) Changing child.transform.parent: {child.gameObject.name}");
                var trySetParent = childNetworkObject.TrySetParent(_insideShipNetworkObject.transform);

                if (trySetParent) continue;

                //SELogger.Log(gameObject, $"Could not set parent for {child.gameObject.name}", LogLevel.Error);
                child.transform.parent = _insideShipNetworkObject.transform;
            }
            else
            {
                //SELogger.Log(gameObject, $"2: (GO) Changing child.transform.parent: {child.gameObject.name}");
                child.transform.parent = _insideShip.transform;
            }

            if (childGameObject.GetComponent<InsideShipComponent>() != null) continue;

            childGameObject.AddComponent<InsideShipComponent>();
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

    private void CopyObjectsForOutside()
    {
        while (_toBeCopiedOutside.Count > 0)
        {
            List<string> partsCopied = new List<string>();
            SELogger.Log(gameObject, $"Copying {_toBeCopiedOutside.Count} object outside");
            foreach (var gameObjectName in _toBeCopiedOutside)
            {
                var findObjectByName = GameObjectHelper.FindObjectByName(gameObjectName);
                /*var insideShipComponent = findObjectByName.GetComponent<InsideShipComponent>();
                if (insideShipComponent != null)
                {
                    Destroy(insideShipComponent);
                }*/

                SELogger.Log(gameObject, $"Copying object to outsideShip {gameObjectName}", LogLevel.Debug);
                var instantiatedOutsideObject = Instantiate(findObjectByName, _outsideShip.transform, true);
                SELogger.Log(gameObject,
                    $"Copied object transforms: T({instantiatedOutsideObject.transform.position}) - LT({instantiatedOutsideObject.transform.localPosition})",
                    LogLevel.Debug);
                TransformHelper.MoveObject(instantiatedOutsideObject, ConstantVariables.InsideShipOffset);
                // Why do I need to do this twice for this object specifically?
                if (gameObjectName.Equals("ShipInside"))
                {
                    TransformHelper.MoveObject(instantiatedOutsideObject, -ConstantVariables.InsideShipOffset);
                }

                SELogger.Log(gameObject,
                    $"Copied object transforms: T({instantiatedOutsideObject.transform.position}) - LT({instantiatedOutsideObject.transform.localPosition})",
                    LogLevel.Debug);
                var insideShipComponent = instantiatedOutsideObject.GetComponent<InsideShipComponent>();
                if (insideShipComponent != null)
                {
                    Destroy(insideShipComponent);
                }

                partsCopied.Add(gameObjectName);
            }

            _toBeCopiedOutside.RemoveAll(x => partsCopied.Contains(x));
        }
    }

    private IEnumerator SetupTeleport()
    {
        var secondCameraCreated = false;
        while (!secondCameraCreated)
        {
            try
            {
                SELogger.Log(gameObject, "Finding Player", LogLevel.Debug);
                var playerContainer = gameObject.transform.Find("Player");
                SELogger.Log(gameObject, $"Found Player: {playerContainer.name}", LogLevel.Debug);
                
                SELogger.Log(gameObject, "Finding ScavengerModel", LogLevel.Debug);
                var scavModel = playerContainer.Find("ScavengerModel");
                SELogger.Log(gameObject, $"Found ScavengerModel: {scavModel.name}", LogLevel.Debug);
                
                SELogger.Log(gameObject, "Finding metarig", LogLevel.Debug);
                var metaRig = scavModel.Find("metarig");
                SELogger.Log(gameObject, $"Found metarig: {metaRig.name}", LogLevel.Debug);
                
                SELogger.Log(gameObject, "Finding CameraContainer", LogLevel.Debug);
                var cameraContainer = metaRig.Find("CameraContainer");
                SELogger.Log(gameObject, $"Found CameraContainer: {cameraContainer.name}", LogLevel.Debug);


                var teleportComponent = cameraContainer.gameObject.AddComponent<TeleportComponent>();
                teleportComponent.Initialize(_insideShip, _outsideShip);
                secondCameraCreated = true;

                SELogger.Log(gameObject, "Creating plane");
                
            }
            catch (Exception e)
            {
                SELogger.Log(gameObject, $"Couldn't find camera, will re-attempt: {e.Message}");
            }

            yield return new WaitForFixedUpdate();
        }
    }

    


    private void OnDestroy()
    {
        SELogger.Log(gameObject, "Removing ExpandShipComponent");
    }
}