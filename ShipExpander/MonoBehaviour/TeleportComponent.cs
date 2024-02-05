using ShipExpander.Core;
using UnityEngine;

namespace ShipExpander.MonoBehaviour;

[RequireComponent(typeof(BoxCollider))]
public class TeleportComponent : UnityEngine.MonoBehaviour
{
    private bool _playerIsOverlapping = false;
    private Transform _player;
    private Transform _receiver;
    private bool _isGoingOutside;

    public void Initialize(Transform player, Transform receiver, bool shouldHideInsideShip)
    {
        _player = player;
        _receiver = receiver;
        _isGoingOutside = shouldHideInsideShip;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerIsOverlapping)
        {
            Vector3 portalToPlayer = _player.position - transform.position;
            SELogger.Log(gameObject,
                $"Player is overlapping with collider component. portalToPlayer: {portalToPlayer}");
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            // If this is true: The player has moved across the portal
            if (dotProduct < 0f)
            {
                MovePlayer(portalToPlayer);
                HandleHidingInsideShip();
            }

            SELogger.Log(gameObject, $"Player is overlapping with collider component. Dot product: {dotProduct}");
        }
    }

    private void HandleHidingInsideShip()
    {
        var findAnyObjectByType = FindObjectsByType<InsideShipLayerToggleComponent>(FindObjectsSortMode.None);
        foreach (var insideShipComponent in findAnyObjectByType)
        {
            if (_isGoingOutside)
            {
                insideShipComponent.EnableLayer();
            }
            else
            {
                insideShipComponent.DisableLayer();
            }
        }
    }

    private void MovePlayer(Vector3 portalToPlayer)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerIsOverlapping = false;
        }
    }
}