using UnityEngine;

namespace ShipExpander.Core;

public abstract class ConstantVariables
{
#if DEBUG
    public static readonly Vector3 InsideShipOffset = new Vector3(0f, 50f, 0);
#else
    public static readonly Vector3 InsideShipOffset = new Vector3(0f, 500f, 0);
#endif
}