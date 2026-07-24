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

        // UI hidden when game starts
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
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

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

        // Hide tunnel UI after escaping
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