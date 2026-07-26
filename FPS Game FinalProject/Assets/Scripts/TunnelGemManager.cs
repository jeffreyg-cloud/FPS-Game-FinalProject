using UnityEngine;
using TMPro;

public class TunnelGemManager : MonoBehaviour
{
    public static TunnelGemManager Instance;

    [Header("Gem Settings")]
    public int requiredGems = 5;
    public int collectedGems = 0;

    [Header("Teleport Back")]
    public Transform originalDoorDestination;

    [Header("UI")]
    public GameObject tunnelObjectiveCanvas;
    public TMP_Text objectiveText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();

        // Hide UI at the beginning
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(false);
        }
    }

    public void CollectGem()
    {
        collectedGems++;

        UpdateUI();

        Debug.Log(
            "Watches collected: " +
            collectedGems + "/" +
            requiredGems
        );

        // If all watches are collected
        if (collectedGems >= requiredGems)
        {
            TeleportBackToDoor();
        }
    }

    private void UpdateUI()
    {
        if (objectiveText != null)
        {
            objectiveText.text =
                "Please collect " +
                requiredGems +
                " watches and escape!\n\n" +
                "Watches: " +
                collectedGems +
                "/" +
                requiredGems;
        }
    }

    private void TeleportBackToDoor()
    {
        // Find Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player with tag 'Player' not found!");
            return;
        }

        // Find CharacterController on Player or parent
        CharacterController controller =
            player.GetComponentInParent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError(
                "CharacterController not found on Player or its parent!"
            );
            return;
        }

        // Check destination
        if (originalDoorDestination == null)
        {
            Debug.LogError(
                "Original Door Destination is not assigned!"
            );
            return;
        }

        // Disable CharacterController
        controller.enabled = false;

        // Teleport player
        controller.transform.position =
            originalDoorDestination.position;

        // Keep player's current rotation
        // Do NOT copy destination rotation

        // Enable CharacterController
        controller.enabled = true;

        // Hide tunnel UI
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(false);
        }

        Debug.Log(
            "Collected 5 watches! " +
            "Teleported back to the door!"
        );
    }
}