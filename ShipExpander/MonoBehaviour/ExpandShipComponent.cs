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


    public static AssetBundle ShaderAsset;


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

    private void Awake()
    {
        // Load assets
        ShaderAsset = UnityBundleHelper.LoadResource("screencutoutshader");
    }


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
        _insideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("InsideTeleporter").WithParent(_insideShip)
                .GetGameObject();
        _outsideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("InsideTeleporter").WithParent(_outsideShip)
                .GetGameObject();
        
        TransformHelper.MoveObject(_insideShipTeleporter.gameObject,new Vector3(-7.2f, 0, -14));
        TransformHelper.MoveObject(_outsideShipTeleporter.gameObject,new Vector3(-7.2f, 0, -14));
        
        bool secondCameraCreated = false;
        Camera outsideCameraGameObject = null;
        while (!secondCameraCreated)
        {
            try
            {
                // TODO: Player contains the camera, make new monobehaviour which is added to player which handles the two cameras then setup teleport properly

                
                // TODO: Why is this not finding the correct camera
                //var cam = gameObject.transform.Find("");
                //var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
                /*foreach (Camera camera in cameras)
                {
                 SELogger.Log(gameObject,  $"{camera}", LogLevel.Warning);
                }*/
                //var cameraContainer = GameObjectHelper.FindNestedObjectChild(this.transform, "CameraContainer");
                // GameObject container goes Player->ScavengerModel->metarig->CameraContainer
                SELogger.Log(gameObject, "Finding Player");
                var playerContainer = gameObject.transform.Find("Player");
                SELogger.Log(gameObject, $"Found Player: {playerContainer.name}");
                
                SELogger.Log(gameObject, "Finding ScavengerModel");
                var scavModel = playerContainer.Find("ScavengerModel");
                SELogger.Log(gameObject, $"Found ScavengerModel: {scavModel.name}");
                
                SELogger.Log(gameObject, "Finding metarig");
                var metaRig = scavModel.Find("metarig");
                SELogger.Log(gameObject, $"Found metarig: {metaRig.name}");
                
                SELogger.Log(gameObject, "Finding CameraContainer");
                var cameraContainer = metaRig.Find("CameraContainer");
                SELogger.Log(gameObject, $"Found CameraContainer: {cameraContainer.name}");


                if (cameraContainer is not null)
                {
                    var mainCameraGameObject = cameraContainer.gameObject.GetComponentInChildren<Camera>();
                    SELogger.Log(gameObject, "Changing inside camera name to MainCameraInside");
                    mainCameraGameObject.name = "MainCameraInside";

                    SELogger.Log(gameObject, "Creating outside camera");
                    outsideCameraGameObject = Instantiate(mainCameraGameObject, cameraContainer.transform, true);
                    outsideCameraGameObject.name = "MainCameraOutside";
                    outsideCameraGameObject.tag = "Untagged";
                    var tempFollowComponent = outsideCameraGameObject.gameObject.AddComponent<TempFollowComponent>();
                    tempFollowComponent.portal = _outsideShipTeleporter.transform;
                    tempFollowComponent.otherPortal = _insideShipTeleporter.transform;
                    tempFollowComponent.playerCamera = mainCameraGameObject.transform;

                    SELogger.Log(gameObject, "Disabling Audio Listener on outside camera");
                    //outsideCameraGameObject.GetComponent<AudioListener>().enabled = false;
                    //TransformHelper.MoveObject(outsideCameraGameObject.gameObject, -ConstantVariables.InsideShipOffset * 2);
                    secondCameraCreated = true;
                }
                else
                {
                    throw new ShipExpanderException();
                    //SELogger.Log(gameObject, "Could not find MainCamera yet", LogLevel.Warning);
                }

                SELogger.Log(gameObject, "Creating plane");
                CreatePlane(outsideCameraGameObject, _insideShipTeleporter.transform,
                    "Render Plane", 5f, 5f);
            }
            catch (Exception e)
            {
                SELogger.Log(gameObject, $"Couldn't find camera, will re-attempt: {e.Message}");
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private GameObject CreatePlane(Camera camera, Transform parentTransform, string planeName, float width,
        float height)
    {
        var shipDoorOffset = new Vector3(-6.4f, 0, -14);

        GameObject g = new GameObject
        {
            // Position = -8 0 -14
            // Rotation = 270 270 0
            name = planeName,
            transform =
            {
                //localPosition = new Vector3(0,0,0),
                parent = parentTransform,
                localEulerAngles = new Vector3(270, 270, 0)
            }
        };

        TransformHelper.MoveObject(g, ConstantVariables.InsideShipOffset + shipDoorOffset);
        g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>();
        g.GetComponent<MeshFilter>().mesh = CreatePlaneMesh(width, height);

        var shader =
            ShaderAsset.LoadAllAssets<Shader>()
                [0]; //ShaderAsset.LoadAsset<Shader>("assets/Shader/ScreenCutoutShader");
        //var find = Shader.Find("Unlit/ScreenCutoutShader");
        Material m = new Material(shader);
        m.name = planeName + "_material";
        //m.shader = Shader.Find("Transparent/Cutout/Diffuse");
        //m.shader = Shader.Find("Standard");

        Renderer renderer = g.GetComponent<Renderer>();
        renderer.material = m;
        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }

        camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        renderer.material.mainTexture = camera.targetTexture;
        renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
        renderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

        return g;
    }


    private Mesh CreatePlaneMesh(float Width, float Height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(Width, 0, Height), new Vector3(Width, 0, -Height), new Vector3(-Width, 0, Height),
            new Vector3(-Width, 0, -Height)
        };
        Vector2[] uv = new Vector2[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };
        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        mesh.vertices = vertices;
        mesh.uv = uv;


        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }


    private void OnDestroy()
    {
        SELogger.Log(gameObject, "Removing ExpandShipComponent");
    }
}