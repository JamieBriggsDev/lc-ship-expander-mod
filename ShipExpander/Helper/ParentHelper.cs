using Unity.Netcode;
using UnityEngine;

namespace ShipExpander.Helper;

public abstract class ParentHelper
{
    public static void SetParent(GameObject child, GameObject parent)
    {
        var childNetworkObject = child.gameObject.GetComponent<NetworkObject>();
        if (childNetworkObject != null)
        {
            //SELogger.Log(gameObject, $"2: (NE) Changing child.transform.parent: {child.gameObject.name}");
            //Vector3 originalPos = child.transform.position;
            var trySetParent = childNetworkObject.TrySetParent(parent.transform);

            if (!trySetParent)
            {
                child.transform.parent = parent.transform;
            }

            //SELogger.Log(gameObject, $"Could not set parent for {child.gameObject.name}", LogLevel.Error);
            //child.transform.position = originalPos;
        }
        else
        {
            //SELogger.Log(gameObject, $"2: (GO) Changing child.transform.parent: {child.gameObject.name}");
            //Vector3 originalPos = child.transform.position;
            child.transform.parent = parent.transform;
            //child.transform.position = originalPos;
        }
    }
}