using UnityEngine;

public class GateTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string gateMessage =
        "Use the Key to Open.";

    [Header("Settings")]
    [SerializeField] private float displayTime = 5f;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered)
        {
            return;
        }

        // 只允许玩家触发提示
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        if (tutorialUI == null)
        {
            Debug.LogWarning(
                "GateTutorial: TutorialUI has not been assigned."
            );
            return;
        }

        hasTriggered = true;

        tutorialUI.ShowMessage(
            gateMessage,
            displayTime
        );
    }
}