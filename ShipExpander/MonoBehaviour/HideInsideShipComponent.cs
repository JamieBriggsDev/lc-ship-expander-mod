using System;
using System.Collections;
using System.Collections.Generic;
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.MonoBehaviour;

public class HideInsideShipComponent : UnityEngine.MonoBehaviour
{
    private bool _shouldHide;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        // Temporary
        StartCoroutine(Switch());
    }

    public IEnumerator Switch()
    {
        bool flip = false;
        while (true)
        {
            if (flip)
            {
                flip = false;
                SELogger.Log(gameObject, "ShowInsideShipLayer()");
                ShowInsideShipLayer();
            }
            else
            {
                flip = true;
                SELogger.Log(gameObject, "HideInsideShipLayer()");
                HideInsideShipLayer();
            }

            yield return new WaitForSeconds(3);
        }
    }
    
    /* Example

         // Render everything *except* layer 14
        camera.cullingMask = ~(1 << 14);

        // Switch off layer 14, leave others as-is
        camera.cullingMask &= ~(1 << 14);

        // Switch on layer 14, leave others as-is
        camera.cullingMask |= (1 << 14);

         */

    public void ShowInsideShipLayer()
    {
        _shouldHide = false;
        // Show inside ship
        SELogger.Log(gameObject, $"Showing layer {ConstantVariables.InsideShipLayer}");
        _camera.cullingMask |= (1 << ConstantVariables.InsideShipLayer);
    }
    
    public void HideInsideShipLayer()
    {
        _shouldHide = true;
        // Hide inside ship
        SELogger.Log(gameObject, $"Hiding layer {ConstantVariables.InsideShipLayer}");
        _camera.cullingMask &= ~(1 << ConstantVariables.InsideShipLayer);
    }
    
}