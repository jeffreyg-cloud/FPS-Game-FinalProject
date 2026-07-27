using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            other.transform.position = respawnPoint.position;
            other.transform.rotation = respawnPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }

            Debug.Log("Player respawned!");
        }
    }
}