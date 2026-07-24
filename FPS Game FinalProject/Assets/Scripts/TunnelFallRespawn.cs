using UnityEngine;

public class TunnelFallRespawn : MonoBehaviour
{
    [Header("Respawn Point")]
    public Transform teleportDestinationTunnel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller =
                other.GetComponent<CharacterController>();

            if (controller != null)
            {
                // Disable CharacterController before teleporting
                controller.enabled = false;

                other.transform.position =
                    teleportDestinationTunnel.position;

                // Enable CharacterController again
                controller.enabled = true;
            }
            else
            {
                other.transform.position =
                    teleportDestinationTunnel.position;
            }
        }
    }
}