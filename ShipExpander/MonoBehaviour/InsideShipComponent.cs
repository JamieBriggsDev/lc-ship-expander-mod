using System;
using ShipExpander.Core;

namespace ShipExpander.MonoBehaviour;

public class InsideShipComponent : UnityEngine.MonoBehaviour
{
    private int initialLayerValue;
    private void Start()
    {
        this.transform.localPosition += ConstantVariables.InsideShipOffset;
        initialLayerValue = gameObject.layer;
        //Show();
    }

    private void OnDestroy()
    {
        this.transform.localPosition -= ConstantVariables.InsideShipOffset;
    }
    
    public void Hide()
    {
        this.gameObject.layer = ConstantVariables.InsideShipLayer;
    }

    public void Show()
    {
        this.gameObject.layer = initialLayerValue;
    }
}