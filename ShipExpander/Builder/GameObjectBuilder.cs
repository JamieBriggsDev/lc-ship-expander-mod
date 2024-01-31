using ShipExpander.Core;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace ShipExpander.Builder;

public class GameObjectBuilder
{
    protected GameObject GameObject;
    protected GameObject Parent;
    protected bool IsNetworkObject;

    public GameObjectBuilder()
    {
        this.Reset();
    }

    private void Reset()
    {
        this.GameObject = new GameObject
        {
            transform =
            {
                localPosition = Vector3.zero
            }
        };
    }

    public GameObjectBuilder WithName(string name)
    {
        this.GameObject.name = name;
        return this;
    }

    public GameObjectBuilder WithNetworkObjectComponent()
    {
        this.IsNetworkObject = true;
        this.GameObject.AddComponent<NetworkObject>();
        return this;
    }
    
    public GameObjectBuilder WithNetworkObjectComponent(ref NetworkObject networkObject)
    {
        this.IsNetworkObject = true;
        networkObject = this.GameObject.AddComponent<NetworkObject>();
        return this;
    }
    

    public GameObjectBuilder WithNetworkTransformComponent()
    {
        this.IsNetworkObject = true;
        this.GameObject.AddComponent<NetworkTransform>();
        return this;
    }

    public GameObjectBuilder WithParent(GameObject parent)
    {
        Vector3 originalPosition = this.GameObject.transform.position;
        this.Parent = parent;
        this.GameObject.transform.position = originalPosition;
        return this;
    }

    public GameObjectBuilder WithPlane(Plane plane)
    {
        this.GameObject.AddComponent<MeshRenderer>();
        return this;
    }

    public GameObject GetGameObject()
    {
        if (Parent != null)
        {
            if (this.IsNetworkObject)
            {
                NetworkObject networkObject = this.GameObject.GetComponent<NetworkObject>();
                NetworkObject parentNetworkObject = this.Parent.GetComponent<NetworkObject>();
                if(networkObject != null && parentNetworkObject != null)
                {
                    networkObject.TrySetParent(parentNetworkObject);
                    networkObject.SpawnWithOwnership(parentNetworkObject.NetworkObjectId);
                }
                else
                {
                    throw new ShipExpanderException("Parent is not an Network Object");
                }
            }
            // Set parent
            this.GameObject.transform.parent = this.Parent.transform;
        }
        this.GameObject.transform.localPosition = Vector3.zero;
        return this.GameObject;
    }
}