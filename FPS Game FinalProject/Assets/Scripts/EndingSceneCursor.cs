using UnityEngine;

public class EndingSceneCursor : MonoBehaviour
{
    private void Start()
    {
        // Show and unlock cursor for the ending scene UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
