
using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.MonoBehaviour;

[RequireComponent(typeof(BoxCollider))]
public class TeleportComponent : UnityEngine.MonoBehaviour
{
    
    private bool _playerIsOverlapping = false;
    private Transform _player;
    private Transform _receiver;

    public void Initialize(Transform player, Transform receiver)
    {
        _player = player;
        _receiver = receiver;
    }

    // Update is called once per frame
    void Update () {
        if (_playerIsOverlapping)
        {
            Vector3 portalToPlayer = _player.position - transform.position;
            SELogger.Log(gameObject, $"Player is overlapping with collider component. portalToPlayer: {portalToPlayer}");
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            // If this is true: The player has moved across the portal
            if (dotProduct < 0f)
            {
                // TODO - Once I know the DOT product is sound, uncomment this.
                // Teleport him!
                float rotationDiff = -Quaternion.Angle(transform.rotation, _receiver.rotation);
                rotationDiff += 180;
                _player.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                _player.position = _receiver.position + positionOffset;

                _playerIsOverlapping = false;
            }
            SELogger.Log(gameObject, $"Player is overlapping with collider component. Dot product: {dotProduct}");
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            _playerIsOverlapping = true;
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "Player")
        {
            _playerIsOverlapping = false;
        }
    }
    
    
}