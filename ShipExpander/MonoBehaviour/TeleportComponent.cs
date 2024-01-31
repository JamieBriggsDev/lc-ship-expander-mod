using ShipExpander.Builder;
using ShipExpander.Core;
using ShipExpander.Helper;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShipExpander.MonoBehaviour;

public class TeleportComponent : UnityEngine.MonoBehaviour
{
    private Camera cameraInside;
    private GameObject _outsideShipTeleporter;
    private GameObject _insideRenderPlane;
    private Camera cameraOutside;
    private GameObject _insideShipTeleporter;

    public static AssetBundle ShaderAsset;
    
    private void Awake()
    {
        // Load assets
        ShaderAsset = UnityBundleHelper.LoadResource("screencutoutshader");
    }

    public void Initialize(GameObject insideShip, GameObject outsideShip)
    {
        _insideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("InsideTeleporter").WithParent(insideShip)
                .GetGameObject();
        _outsideShipTeleporter =
            new GameObjectBuilder().WithNetworkObjectComponent().WithName("InsideTeleporter").WithParent(outsideShip)
                .GetGameObject();
        
        TransformHelper.MoveObject(_insideShipTeleporter.gameObject,new Vector3(-7.2f, 0, -14));
        TransformHelper.MoveObject(_outsideShipTeleporter.gameObject,new Vector3(-7.2f, 0, -14));
        
        cameraInside = GetComponentInChildren<Camera>();
        SELogger.Log(gameObject, "Changing inside camera name to MainCameraInside");
        cameraInside.name = "MainCameraInside";

        SELogger.Log(gameObject, "Creating outside camera");
        cameraOutside = Instantiate(cameraInside, transform, true);
        cameraOutside.name = "MainCameraOutside";
        cameraOutside.tag = "Untagged";
        var tempFollowComponent = cameraOutside.gameObject.AddComponent<TempFollowComponent>();
        tempFollowComponent.portal = _outsideShipTeleporter.transform;
        tempFollowComponent.otherPortal = _insideShipTeleporter.transform;
        tempFollowComponent.playerCamera = cameraInside.transform;

        SELogger.Log(gameObject, "Disabling Audio Listener on outside camera");
        //outsideCameraGameObject.GetComponent<AudioListener>().enabled = false;
        //TransformHelper.MoveObject(outsideCameraGameObject.gameObject, -ConstantVariables.InsideShipOffset * 2);
        
        _insideRenderPlane = CreatePlane(cameraOutside, _insideShipTeleporter.transform,
            "Render Plane", 5f, 5f);
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


    private Mesh CreatePlaneMesh(float width, float height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = {
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
    
    
}