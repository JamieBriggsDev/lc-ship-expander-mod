using System;
using ShipExpander.Core;

namespace ShipExpander.MonoBehaviour;

public class InsideShipComponent : UnityEngine.MonoBehaviour
{
    private int initialLayerValue;
    private void Start()
    {
        this.transform.localPosition += ConstantVariables.InsideShipOffset;
    }

    private void OnDestroy()
    {
        this.transform.localPosition -= ConstantVariables.InsideShipOffset;
    }
    
    
}