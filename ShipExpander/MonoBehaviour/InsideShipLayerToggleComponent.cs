using System;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.MonoBehaviour;

public class InsideShipLayerToggleComponent : UnityEngine.MonoBehaviour
{
    private int initialLayerValue;

    private void Start()
    {
        initialLayerValue = gameObject.layer;
        foreach (var child in this.transform)
        {
            if (child is not GameObject childGameObject) continue;
            
            if (childGameObject.GetComponent<InsideShipComponent>() == null)
            {
                childGameObject.AddComponent<InsideShipComponent>();
            }
        }
        //Show();
    }
    
    public void EnableLayer()
    {
        this.gameObject.layer = ConstantVariables.InsideShipLayer;
    }

    public void DisableLayer()
    {
        this.gameObject.layer = initialLayerValue;
    }
}