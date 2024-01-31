using System;
using ShipExpander.Core;

namespace ShipExpander.MonoBehaviour;

public class InsideShipComponent : UnityEngine.MonoBehaviour
{
    private void Start()
    {
        this.transform.localPosition += ConstantVariables.InsideShipOffset;
    }
}