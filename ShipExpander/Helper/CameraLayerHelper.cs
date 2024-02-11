using UnityEngine;

namespace ShipExpander.Helper;

public static class CameraLayerHelper 
{
    public static void LayerCullingShow(this Camera camera, int layerMask) {
        camera.cullingMask |= layerMask;
    }
    public static void LayerCullingHide(this Camera camera, int layerMask) {
        camera.cullingMask &= ~layerMask;
    }
    public static void LayerCullingToggle(this Camera camera, int layerMask) {
        camera.cullingMask ^= layerMask;
    }
    public static bool LayerCullingIncludes(this Camera camera, int layerMask) {
        return (camera.cullingMask & layerMask) > 0;
    }
    public static void LayerCullingToggle(this Camera camera, int layerMask, bool isOn) {
        bool included = LayerCullingIncludes(camera, layerMask);
        if (isOn && !included) {
            LayerCullingShow(camera, layerMask);
        } else if (!isOn && included) {
            LayerCullingHide(camera, layerMask);
        }
    }
}