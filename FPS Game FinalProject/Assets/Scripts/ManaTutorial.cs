using System.Collections;
using UnityEngine;

public class ManaTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Mana Tutorial Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string manaMessage =
        "Mana is consumed when attacking.\n\n" +
        "Press H to consume a Mana Gem.";

    [Header("Settings")]
    [SerializeField] private float manaMessageDisplayTime = 6f;

    private bool manaTutorialHasShown;

    private void Awake()
    {
        if (tutorialUI == null)
        {
            tutorialUI = GetComponent<TutorialUI>();
        }
    }

    private void Update()
    {
        /*
         * ==================== TEST CODE ====================
         * M key: shows the mana tutorial message for testing.
         *
         * FINAL INTEGRATION:
         * Delete this if block once wired up to whatever event
         * should actually trigger the mana tutorial.
         * =================================================
         */
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowManaTutorial();
        }
    }

    /// <summary>
    /// Call this whenever you want to show the mana tutorial message
    /// (e.g. first time the player tries to attack/use mana).
    /// </summary>
    public void ShowManaTutorial()
    {
        if (manaTutorialHasShown)
        {
            return;
        }

        if (tutorialUI == null)
        {
            Debug.LogWarning("ManaTutorial: TutorialUI was not found.");
            return;
        }

        manaTutorialHasShown = true;
        tutorialUI.ShowMessage(manaMessage, manaMessageDisplayTime);
    }
}