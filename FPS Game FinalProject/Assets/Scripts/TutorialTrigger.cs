using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Reference")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 5)]
    [SerializeField] private string message;

    [SerializeField] private float displayTime = 5f;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        tutorialUI.ShowMessage(message, displayTime);
        hasTriggered = true;
    }
}
