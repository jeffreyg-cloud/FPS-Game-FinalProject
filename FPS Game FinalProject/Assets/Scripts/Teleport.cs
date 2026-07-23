using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Destination")]
    public Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = teleportDestination.position;
                controller.enabled = true;
            }
            else
            {
                other.transform.position = teleportDestination.position;
            }
        }
    }
}