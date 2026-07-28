
using UnityEngine;

public class TunnelTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform tunnelDestination;

    private bool hasTeleported = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only detect the Player
        if (!other.CompareTag("Player"))
            return;

        // Prevent multiple teleporting
        if (hasTeleported)
            return;

        // Check Tunnel Destination
        if (tunnelDestination == null)
        {
            Debug.LogError(
                "Tunnel Destination is NOT assigned!"
            );
            return;
        }

        hasTeleported = true;

        // Find CharacterController
        CharacterController controller =
            other.GetComponent<CharacterController>();

        if (controller == null)
        {
            controller =
                other.GetComponentInParent<CharacterController>();
        }

        // Check CharacterController
        if (controller == null)
        {
            Debug.LogError(
                "CharacterController NOT found on Player!"
            );

            hasTeleported = false;
            return;
        }

        // Disable CharacterController
        controller.enabled = false;

        // Teleport Player
        controller.transform.position =
            tunnelDestination.position;

        // IMPORTANT:
        // Do NOT change the player's rotation.
        // This prevents the camera from becoming upside down.

        // Enable CharacterController
        controller.enabled = true;

        Debug.Log(
            "PLAYER SUCCESSFULLY TELEPORTED INTO TUNNEL!"
        );

        // Show Tunnel Objective UI
        if (TunnelGemManager.Instance != null)
        {
            TunnelGemManager.Instance.ShowTunnelUI();

            Debug.Log(
                "TUNNEL OBJECTIVE UI ACTIVATED!"
            );
        }
        else
        {
            Debug.LogError(
                "TunnelGemManager.Instance is NULL!"
            );
        }
    }
}

