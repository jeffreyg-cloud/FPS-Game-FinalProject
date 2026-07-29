using UnityEngine;

public class ClockTeleport : MonoBehaviour
{
    public Transform teleportDestination;

private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CharacterController controller =
            other.GetComponentInParent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController not found!");
            return;
        }

        if (teleportDestination == null)
        {
            Debug.LogError("Teleport Destination is not assigned!");
            return;
        }

        // Disable Character Controller
        controller.enabled = false;

        // Teleport Player
        controller.transform.position =
            teleportDestination.position;

        controller.transform.rotation =
            teleportDestination.rotation;

        // Enable Character Controller
        controller.enabled = true;

        // Change to Clock Music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayClockMusic();
            Debug.Log("Clock Music Started!");
        }
        else
        {
            Debug.LogWarning("MusicManager Instance not found!");
        }

        Debug.Log("Player teleported into clock!");
    }

}
