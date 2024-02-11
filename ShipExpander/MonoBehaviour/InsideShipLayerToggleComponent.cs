using System;
using System.Collections.Generic;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.MonoBehaviour;

public class InsideShipLayerToggleComponent : UnityEngine.MonoBehaviour
{
    private int initialLayerValue;
    public bool neverDestroy;

    public List<int> layersToIgnore = new()
    {
        LayerMask.NameToLayer("Triggers"),
        LayerMask.NameToLayer("Colliders"),
        LayerMask.NameToLayer("ScanNode")
    };

    private void Start()
    {
        initialLayerValue = gameObject.layer;
        /*foreach (var child in this.transform)
        {
            if (child is not GameObject childGameObject) continue;

            if (childGameObject.GetComponent<InsideShipLayerToggleComponent>() == null)
            {
                childGameObject.AddComponent<InsideShipLayerToggleComponent>();
            }
        }*/
        //Show();
    }

    private void Update()
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.GetComponent<InsideShipLayerToggleComponent>() == null)
            {
                child.gameObject.AddComponent<InsideShipLayerToggleComponent>();
            }
        }

        // Check if parent does not have component anymore, otherwise destroy
        if (neverDestroy) return;
        if (this.transform.parent.gameObject.GetComponent<InsideShipLayerToggleComponent>() == null)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        this.gameObject.layer = initialLayerValue;
    }

    public void EnableLayer()
    {
        if (layersToIgnore.Contains(initialLayerValue)) return;

        this.gameObject.layer = ConstantVariables.InsideShipLayer;
    }

    public void DisableLayer()
    {
        if (layersToIgnore.Contains(initialLayerValue)) return;
        this.gameObject.layer = initialLayerValue;
    }
}