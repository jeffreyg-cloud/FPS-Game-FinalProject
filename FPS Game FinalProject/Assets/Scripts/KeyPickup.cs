using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Tutorial Reference")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string collectMessage =
        "You collected the Gate Key!\n" +
        "You can now open the gate.";

    private bool playerNearby;

    // Store the player that entered the trigger
    private PlayerKey playerKey;

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Give the player the key
            if (playerKey != null)
            {
                playerKey.hasKey = true;
                Debug.Log("Player collected the key!");
            }

            tutorialUI.ShowMessage(collectMessage, 5f);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            playerNearby = true;

            // Get the PlayerKey component from the player
            playerKey = player.GetComponent<PlayerKey>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = false;
            playerKey = null;
        }
    }
}