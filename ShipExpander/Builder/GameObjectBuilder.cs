using ShipExpander.Core;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace ShipExpander.Builder;

public class GameObjectBuilder
{
    private GameObject _gameObject;
    private GameObject _parent;
    private bool _isNetworkObject;

    public GameObjectBuilder()
    {
        this.Reset();
        this._gameObject.transform.localPosition = Vector3.zero;
    }

    private void Reset()
    {
        this._gameObject = new GameObject();
    }

    public GameObjectBuilder WithName(string name)
    {
        this._gameObject.name = name;
        return this;
    }

    public GameObjectBuilder WithNetworkObjectComponent()
    {
        this._isNetworkObject = true;
        this._gameObject.AddComponent<NetworkObject>();
        return this;
    }
    
    public GameObjectBuilder WithNetworkObjectComponent(ref NetworkObject networkObject)
    {
        this._isNetworkObject = true;
        networkObject = this._gameObject.AddComponent<NetworkObject>();
        return this;
    }
    

    public GameObjectBuilder WithNetworkTransformComponent()
    {
        this._isNetworkObject = true;
        this._gameObject.AddComponent<NetworkTransform>();
        return this;
    }

    public GameObjectBuilder WithParent(GameObject parent)
    {
        Vector3 originalPosition = this._gameObject.transform.position;
        this._parent = parent;
        this._gameObject.transform.position = originalPosition;
        return this;
    }

    public GameObject GetGameObject()
    {
        if (_parent != null)
        {
            if (this._isNetworkObject)
            {
                NetworkObject networkObject = this._gameObject.GetComponent<NetworkObject>();
                NetworkObject parentNetworkObject = this._parent.GetComponent<NetworkObject>();
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
            this._gameObject.transform.parent = this._parent.transform;
        }
        this._gameObject.transform.localPosition = Vector3.zero;
        return this._gameObject;
    }
}