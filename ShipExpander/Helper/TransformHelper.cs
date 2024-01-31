using Unity.Netcode;
using UnityEngine;

namespace ShipExpander.Helper;

public abstract class TransformHelper
{
    public static void MoveObject(GameObject gameObject, Vector3 distance)
    {
        // Check if network object
        var networkObject = gameObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            //networkObject.transform.position += distance;
            gameObject.transform.position += distance;
        }
        else
        {
            gameObject.transform.position += distance;
        }
    }
}