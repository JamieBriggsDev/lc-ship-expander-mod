using BepInEx.Logging;
using ShipExpander.Builder;
using ShipExpander.Core;
using ShipExpander.Helper;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShipExpander.MonoBehaviour;

public class TeleportCreatorComponent : UnityEngine.MonoBehaviour
{
#if DEBUG
    private readonly bool shouldRenderColliderInDebug = false;
#endif

    private static readonly Vector3 ShipDoorOffset = new Vector3(-5.75f, 2, -14); // + new Vector3(-1,2,0);
    private static readonly Vector3 RenderPlaneOffset = new(.4f, 0, 0);

    private Transform _player;
    private Camera _playerCamera;

    private Camera _cameraInside;
    private GameObject _insideShipTeleporter;
    private TeleportComponent _insideColliderBox;

    private Camera _cameraOutside;
    private GameObject _outsideShipTeleporter;
    private TeleportComponent _outsideColliderBox;

    //public static AssetBundle ShaderAsset;

    /*private void Awake()
    {
        // Load assets
        ShaderAsset = UnityBundleHelper.LoadResource("lethalcompanyshaders");
    }*/

    public void Initialize(GameObject insideShip, GameObject outsideShip, Transform player)
    {
        _player = player;

        // Creating initial game objects for inside and outside teleporter
        _insideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("InsideTeleporter").WithParent(insideShip)
                .GetGameObject();
        _outsideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("OutsideTeleporter").WithParent(outsideShip)
                .GetGameObject();

        // Moving teleporters to correct location
        TransformHelper.MoveObject(_insideShipTeleporter.gameObject, new Vector3(-7.2f, 0, -14));
        TransformHelper.MoveObject(_outsideShipTeleporter.gameObject, new Vector3(-7.2f, 0, -14));

        // Get player camera
        _playerCamera = GetComponentInChildren<Camera>();

        // Create inside camera
        SELogger.Log(gameObject, "Creating inside camera name");
        _cameraInside = Instantiate(_playerCamera, transform, true);
        _cameraInside.name = "MainCameraInside";
        _cameraInside.tag = "Untagged";
        //SELogger.Log(gameObject, "Disabling Audio Listener on inside camera");
        //_cameraInside.GetComponent<AudioListener>().enabled = false;
        SELogger.Log(gameObject, "Adding TempFollowComponent to insideCamera");
        var insideFollowCameraComponent = _cameraInside.gameObject.AddComponent<TempFollowComponent>();
        SELogger.Log(gameObject, "Setting variables to tempFollowComponent on inside camera");
        insideFollowCameraComponent.portal = _insideShipTeleporter.transform;
        insideFollowCameraComponent.otherPortal = _outsideShipTeleporter.transform;
        insideFollowCameraComponent.playerCamera = _playerCamera.transform;

        // Create outside camera
        SELogger.Log(gameObject, "Creating outside camera");
        _cameraOutside = Instantiate(_playerCamera, transform, true);
        _cameraOutside.name = "MainCameraOutside";
        _cameraOutside.tag = "Untagged";
        //SELogger.Log(gameObject, "Disabling Audio Listener on inside camera");
        //_cameraOutside.GetComponent<AudioListener>().enabled = false;
        SELogger.Log(gameObject, "Adding TempFollowComponent to outsideCamera");
        var outsideFollowCameraComponent = _cameraOutside.gameObject.AddComponent<TempFollowComponent>();
        SELogger.Log(gameObject, "Setting variables to tempFollowComponent on outside camera");
        outsideFollowCameraComponent.portal = _outsideShipTeleporter.transform;
        outsideFollowCameraComponent.otherPortal = _insideShipTeleporter.transform;
        outsideFollowCameraComponent.playerCamera = _playerCamera.transform;


        // Create inside render plane
        SELogger.Log(gameObject, "Creating insideRenderPlane");
        var insideRenderPlane = CreatePlane(_cameraOutside, _insideShipTeleporter.transform,
            "RenderPlane");
        SELogger.Log(gameObject, "Moving insideRenderPlane up");
        TransformHelper.MoveObject(insideRenderPlane.gameObject, ConstantVariables.InsideShipOffset);
        // Create inside collider box
        SELogger.Log(gameObject, "Creating insideColliderBox");
        _insideColliderBox = CreateColliderPlane(_insideShipTeleporter.transform, "ColliderPlane");
        SELogger.Log(gameObject, "Moving insideColliderBox up");
        TransformHelper.MoveObject(_insideColliderBox.gameObject, ConstantVariables.InsideShipOffset);


        CreatePlane(_cameraInside, _outsideShipTeleporter.transform,
            "RenderPlane", true);
        _outsideColliderBox = CreateColliderPlane(_outsideShipTeleporter.transform, "ColliderPlane", true);


        SELogger.Log(gameObject, "Initializing colliderBoxes up");
        _insideColliderBox.Initialize(_player, _outsideColliderBox.transform, false);
        _outsideColliderBox.Initialize(_player, _insideColliderBox.transform, true);
    }

    private GameObject CreatePlane(Camera camera, Transform parentTransform, string planeName, bool flipped = false)
    {
        float width = 2.25f;
        float height = 2.25f;
        GameObject g = new GameObject
        {
            // Position = -8 0 -14
            // Rotation = 270 270 0
            name = planeName,
            transform =
            {
                //localPosition = new Vector3(0,0,0),
                parent = parentTransform,
                // Rotate 180 if flipped
                localEulerAngles = new Vector3(270, 270, flipped ? 180f : 0)
            }
        };

        if (flipped)
        {
            TransformHelper.MoveObject(g, ShipDoorOffset + RenderPlaneOffset);
        }
        else
        {
            TransformHelper.MoveObject(g, ShipDoorOffset + -RenderPlaneOffset);
        }

        g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>();
        g.GetComponent<MeshFilter>().mesh = CreatePlaneMesh(width, height);

        var shader = UnityBundleHelper.GetScreenCutoutShader();
        //ShaderAsset.LoadAllAssets<Shader>()
        //    [0]; //ShaderAsset.LoadAsset<Shader>("assets/Shader/ScreenCutoutShader");
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


    private Mesh CreatePlaneMesh(float width, float height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices =
        {
            new(width, 0, height), new(width, 0, -height), new(-width, 0, height),
            new(-width, 0, -height)
        };
        Vector2[] uv = { new(1, 1), new(1, 0), new(0, 1), new(0, 0) };
        int[] triangles = { 0, 1, 2, 2, 1, 3 };
        mesh.vertices = vertices;
        mesh.uv = uv;


        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private TeleportComponent CreateColliderPlane(Transform parentTransform, string colliderName, bool flipped = false)
    {
        GameObject g = new GameObject();
        g.name = colliderName;
        g.transform.position = new Vector3();
        g.transform.parent = parentTransform;
        // Flipped to rotate correct way
        g.transform.localEulerAngles = flipped ? new Vector3(270, 0, 90) : new Vector3(270, 0, -90);
        var boxCollider = g.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(2, 0, 3);

#if DEBUG
        if (shouldRenderColliderInDebug)
        {
            SELogger.Log(gameObject, "Doing DEBUG step", LogLevel.Warning);

            // Add render component when debug to easily find when developing
            SELogger.Log(gameObject, "Adding MeshFilter");
            g.AddComponent<MeshFilter>();
            SELogger.Log(gameObject, "Adding MeshRenderer");
            g.AddComponent<MeshRenderer>();
            SELogger.Log(gameObject, "Adding MeshFilter");
            g.GetComponent<MeshFilter>().mesh = CreatePlaneMesh(5F, 5F);

            SELogger.Log(gameObject, "Creating shader");
            var shader = UnityBundleHelper.GetDebugShader();
            SELogger.Log(gameObject, "Creating material");
            Material m = new Material(shader);
            m.name = name + "_material";

            SELogger.Log(gameObject, "Getting renderer");
            Renderer renderer = g.GetComponent<Renderer>();
            SELogger.Log(gameObject, "Assigning material to renderer");
            renderer.material = m;
        }


#endif

        //var portalTeleporter = g.AddComponent<PortalTeleporter>();
        //portalTeleporter.player = player;
        //portalTeleporter.reciever = receiver;

        /*if (flipped)
        {
            TransformHelper.MoveObject(g, ShipDoorOffset + -RenderPlaneOffset);
        }
        else
        {
            TransformHelper.MoveObject(g, ShipDoorOffset + RenderPlaneOffset);
        }*/
        TransformHelper.MoveObject(g, ShipDoorOffset);


        var teleportComponent = g.AddComponent<TeleportComponent>();
        // Handle adding variables to TeleportComponent here

        return teleportComponent;
    }
}