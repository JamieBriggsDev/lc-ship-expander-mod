using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShipExpander.Builder;
using ShipExpander.Core;
using ShipExpander.Helper;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using LogLevel = BepInEx.Logging.LogLevel;

namespace ShipExpander.MonoBehaviour;

public class ExpandShipComponent : UnityEngine.MonoBehaviour
{
    private NetworkObject _thisNetworkObject;
    private GameObject _insideShip;
    private NetworkObject _insideShipNetworkObject;
    private GameObject _outsideShip;
    private NetworkObject _outsideShipNetworkObject;


    private List<string> _toIgnore = new()
    {
        "Player",
        "InsideTeleporter",
        "OutsideTeleporter"
    };

    private List<string> _toBeCopiedOutside = new()
    {
        "WallInsulator",
        "ShipModels2b", // Outside ship parts
        "ShipInside", // More outside ship parts, despite naming
        // Lighting
        "ShipElectricLights"
    };

    private List<string> _toIgnoreInside = new()
    {
        "Cameras",
        // Ship stuff
        "Cube.004",
        "Cube.005",
        "Cube.006",
        "Cube.007",
        "Cube.008",
        "ThrusterBackRight",
        "ThrusterBackLeft",
        "ThrusterFrontRight",
        "ThrusterFrontLeft",
        "ShipSupportBeams",
        // Catwalk stuff should really be moved to _toIgnoreInside but leave for debugging
        "ShipRails",
        "ShipRailPosts",
        "CatwalkRailLining",
        "CatwalkShip",
        "CatwalkUnderneathSupports",
        "ClimbOntoCatwalkHelper",
        "CatwalkRailLiningB",
        "Ladder",
        "LadderShort",
        "LadderShort (1)",
        "LargePipe (1)"
        // End of catwalk stuff
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


        SELogger.Log(gameObject, "Setting up teleport");
        StartCoroutine(SetupTeleport());
    }


    private void LateUpdate()
    {
        FixInsideShipHierarchy();
        FixLightingForInsideShip();
        FixVolumetricEffectsForInsideShip();
        CopyObjectsForOutside();
    }

    private void FixVolumetricEffectsForInsideShip()
    {
        foreach (var volumetric in FindObjectsByType<LocalVolumetricFog>(FindObjectsSortMode.None))
        {
        }
    }

    private void FixLightingForInsideShip()
    {
        foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (!light.LayerCullingIncludes(1 << ConstantVariables.InsideShipLayer))
                //if (light.cullingMask != (light.cullingMask | (1 << ConstantVariables.InsideShipLayer)))
                //if ((light.cullingMask & (1 << ConstantVariables.InsideShipLayer)) != 0)
            {
                //SELogger.Log(gameObject,
                //    $"Adding layer {ConstantVariables.InsideShipLayer} to object {light.name}");
                //SELogger.Log(gameObject, $"Before: {light.cullingMask}");
                light.LayerCullingShow(1 << ConstantVariables.InsideShipLayer);
                //SELogger.Log(gameObject, $"After: {light.cullingMask}");
            }
        }
    }

    private void FixInsideShipHierarchy()
    {
        // Moving existing children of this gameobject to insideShip
        foreach (Transform child in gameObject.transform)
        {
            var childGameObject = child.gameObject;


            // Should ignore if child has physics prop component as props should not belong to ship


            if (_toIgnoreInside.Contains(childGameObject.name) || _toIgnore.Contains(childGameObject.name)) continue;
            // Should ignore if child is helper container object for this mod or null
            if (childGameObject == _insideShip || childGameObject == _outsideShip || child == null) continue;


            //SELogger.Log(gameObject, $"1: Moving object into insideShip: {child.gameObject.name}");


            var childNetworkObject = childGameObject.GetComponent<NetworkObject>();


            //ParentHelper.SetParent(childGameObject, _insideShip);

            // Adds component which toggles the game objects layer with it's original and a new one
            // StartGameLever does not like this component.
            
            _insideShip.AddComponent<InsideShipLayerToggleComponent>();
            /*if (childGameObject.gameObject.name != "StartGameLever" &&
                childGameObject.GetComponent<InsideShipLayerToggleComponent>() == null)
            {
                childGameObject.AddComponent<InsideShipLayerToggleComponent>();
            }*/
            
            if (child.GetComponent<PhysicsProp>() != null) continue;

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

            if (childGameObject.GetComponent<InsideShipComponent>() == null)
            {
                childGameObject.AddComponent<InsideShipComponent>();
            }
        }

        // TODO 11/02/2024: Should use object toIgnoreInside to move objects to _outsideShip
        
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
                //TransformHelper.MoveObject(instantiatedOutsideObject, ConstantVariables.InsideShipOffset);
                // Why do I need to do this twice for this object specifically?
                //if (gameObjectName.Equals("ShipInside"))
                //{
                //    TransformHelper.MoveObject(instantiatedOutsideObject, -ConstantVariables.InsideShipOffset);
                //}

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
        var foundMainCamera = false;
        Transform playerContainer = null;
        Transform cameraContainer = null;
        while (!foundMainCamera)
        {
            try
            {
                SELogger.Log(gameObject, "Finding Player", LogLevel.Debug);
                playerContainer = gameObject.transform.Find("Player");
                SELogger.Log(gameObject, $"Found Player: {playerContainer.name}", LogLevel.Debug);

                SELogger.Log(gameObject, "Finding ScavengerModel", LogLevel.Debug);
                var scavModel = playerContainer.Find("ScavengerModel");
                SELogger.Log(gameObject, $"Found ScavengerModel: {scavModel.name}", LogLevel.Debug);

                SELogger.Log(gameObject, "Finding metarig", LogLevel.Debug);
                var metaRig = scavModel.Find("metarig");
                SELogger.Log(gameObject, $"Found metarig: {metaRig.name}", LogLevel.Debug);

                SELogger.Log(gameObject, "Finding CameraContainer", LogLevel.Debug);
                cameraContainer = metaRig.Find("CameraContainer");
                SELogger.Log(gameObject, $"Found CameraContainer: {cameraContainer.name}", LogLevel.Debug);
                foundMainCamera = true;
            }
            catch (Exception e)
            {
                SELogger.Log(gameObject, $"Couldn't find camera, will re-attempt: {e.Message}");
            }

            yield return new WaitForFixedUpdate();
        }

        var teleportComponent = cameraContainer.gameObject.AddComponent<TeleportCreatorComponent>();
        teleportComponent.Initialize(_insideShip, _outsideShip, playerContainer);
    }


    private void OnDestroy()
    {
        SELogger.Log(gameObject, "Removing ExpandShipComponent");
    }
}