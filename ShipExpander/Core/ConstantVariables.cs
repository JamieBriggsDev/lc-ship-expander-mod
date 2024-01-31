using UnityEngine;

namespace ShipExpander.Core;

public abstract class ConstantVariables
{
    public static readonly Vector3 InsideShipOffset = new Vector3(0f, 20f, 0);
    public static readonly int InsideShipLayer = 31; // Can be from 4-31
}