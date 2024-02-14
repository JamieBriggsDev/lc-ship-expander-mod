using UnityEngine;

namespace ShipExpander.Helper;

public static class LightLayerHelper 
{
    public static void LayerCullingShow(this Light light, int layerMask) {
        light.cullingMask |= layerMask;
    }
    public static void LayerCullingHide(this Light light, int layerMask) {
        light.cullingMask &= ~layerMask;
    }
    public static void LayerCullingToggle(this Light light, int layerMask) {
        light.cullingMask ^= layerMask;
    }
    public static bool LayerCullingIncludes(this Light light, int layerMask) {
        return (light.cullingMask & layerMask) > 0;
    }
    public static void LayerCullingToggle(this Light light, int layerMask, bool isOn) {
        bool included = LayerCullingIncludes(light, layerMask);
        if (isOn && !included) {
            LayerCullingShow(light, layerMask);
        } else if (!isOn && included) {
            LayerCullingHide(light, layerMask);
        }
    }
}