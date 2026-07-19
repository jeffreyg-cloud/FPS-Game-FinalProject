using UnityEngine;

public class TutorialCollectItem : MonoBehaviour
{
    [Header("Tutorial Reference")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string collectMessage =
        "Mushrooms and gems collected.\n" +
        "They are now stored in your inventory.";

    private bool playerNearby;

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            tutorialUI.ShowMessage(collectMessage, 5f);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = false;
        }
    }
}