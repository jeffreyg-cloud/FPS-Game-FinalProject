using UnityEngine;

public class TunnelGemManager : MonoBehaviour
{
    public static TunnelGemManager Instance;

    [Header("Gem Settings")]
    public int requiredGems = 5;
    public int collectedGems = 0;

    [Header("Teleport Back")]
    public Transform originalDoorDestination;

    private void Awake()
    {
        Instance = this;
    }

    public void CollectGem()
    {
        collectedGems++;

        Debug.Log("Gems collected: " + collectedGems + "/" + requiredGems);

        if (collectedGems >= requiredGems)
        {
            TeleportBackToDoor();
        }
    }

    private void TeleportBackToDoor()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;

            player.transform.position =
                originalDoorDestination.position;

            player.transform.rotation =
                originalDoorDestination.rotation;

            controller.enabled = true;
        }
        else
        {
            player.transform.position =
                originalDoorDestination.position;

            player.transform.rotation =
                originalDoorDestination.rotation;
        }

        Debug.Log("Collected 5 gems! Teleported back to the door!");
    }
}