using UnityEngine;

public class HealthTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string message =
        "HP decreased.\n\n" +
        "Press G to consume a Mushroom.";

    [Header("Settings")]
    [SerializeField] private float displayTime = 5f;

    private bool hasShown = false;

    public void ShowHealthTutorial()
    {
        if (hasShown)
            return;

        hasShown = true;

        tutorialUI.ShowMessage(message, displayTime);
    }
}