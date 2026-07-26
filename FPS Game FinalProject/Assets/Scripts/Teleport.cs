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

        // Disable CharacterController
        controller.enabled = false;

        // Teleport player
        controller.transform.position =
            teleportDestination.position;

        // Enable CharacterController
        controller.enabled = true;

        // Show tunnel UI
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(true);
        }

        // Change music to tunnel music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayTunnelMusic();
        }

        Debug.Log("Player teleported into tunnel!");
    }
}