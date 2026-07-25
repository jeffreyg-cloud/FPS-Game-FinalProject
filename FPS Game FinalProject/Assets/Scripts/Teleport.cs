using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportDestination;
    public GameObject tunnelObjectiveCanvas;

    private bool hasTeleported = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTeleported)
            return;

        if (!other.CompareTag("Player"))
            return;

        CharacterController controller =
            other.GetComponentInParent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController not found!");
            return;
        }

        hasTeleported = true;

        // Turn off CharacterController
        controller.enabled = false;

        // Teleport the PLAYER
        controller.transform.position =
            teleportDestination.position;

        controller.transform.rotation =
            teleportDestination.rotation;

        // Turn CharacterController back ON
        controller.enabled = true;

        // Show UI
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(true);
        }

        Debug.Log("Player teleported into tunnel!");
    }
}