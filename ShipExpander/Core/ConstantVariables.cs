using UnityEngine;

namespace ShipExpander.Core;

public abstract class ConstantVariables
{
    public static readonly Vector3 InsideShipOffset = new Vector3(0f, 20f, 0);
    // Would be great if this value was 31 but need to understand why 31 does not work with post processing and shadows.
    public static readonly int InsideShipLayer = 7; // Can be from 4-31
    public static readonly int RenderLayer = 30; // Can be from 4-31
}