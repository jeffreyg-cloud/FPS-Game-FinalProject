
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
        // Set Singleton
        Instance = this;
    }

    private void Start()
    {
        // Update UI text
        UpdateUI();

        // Hide UI at the beginning
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(false);
        }
    }

    // ==========================================
    // SHOW TUNNEL UI
    // ==========================================

    public void ShowTunnelUI()
    {
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(true);

            Debug.Log(
                "Tunnel Objective Canvas is now ACTIVE!"
            );
        }
        else
        {
            Debug.LogError(
                "Tunnel Objective Canvas is NOT assigned!"
            );
        }

        // Update text
        UpdateUI();
    }

    // ==========================================
    // COLLECT GEM / WATCH
    // ==========================================

    public void CollectGem()
    {
        collectedGems++;

        // Update UI
        UpdateUI();

        Debug.Log(
            "Watches collected: " +
            collectedGems +
            "/" +
            requiredGems
        );

        // Check if all watches are collected
        if (collectedGems >= requiredGems)
        {
            TeleportBackToDoor();
        }
    }

    // ==========================================
    // UPDATE UI TEXT
    // ==========================================

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
        else
        {
            Debug.LogWarning(
                "Objective Text is NOT assigned!"
            );
        }
    }

    // ==========================================
    // TELEPORT BACK TO DOOR
    // ==========================================

    private void TeleportBackToDoor()
    {
        // Find Player
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "Player with tag 'Player' not found!"
            );
            return;
        }

        // Find CharacterController
        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller == null)
        {
            controller =
                player.GetComponentInParent<CharacterController>();
        }

        if (controller == null)
        {
            Debug.LogError(
                "CharacterController not found!"
            );
            return;
        }

        // Check destination
        if (originalDoorDestination == null)
        {
            Debug.LogError(
                "Original Door Destination is NOT assigned!"
            );
            return;
        }

        // Disable CharacterController
        controller.enabled = false;

        // Teleport Player Back
        controller.transform.position =
            originalDoorDestination.position;

        // IMPORTANT:
        // Do NOT change player's rotation.
        // This keeps the camera rotation unchanged.

        // Enable CharacterController
        controller.enabled = true;

        // Hide Tunnel UI
        if (tunnelObjectiveCanvas != null)
        {
            tunnelObjectiveCanvas.SetActive(false);
        }

        Debug.Log(
            "Collected all watches!"
        );

        Debug.Log(
            "Player teleported back to the original door!"
        );
    }
}

