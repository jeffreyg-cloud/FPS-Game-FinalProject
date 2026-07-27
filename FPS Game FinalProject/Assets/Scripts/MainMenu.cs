using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string storySceneName = "Story";
    public string levelSceneName = "Emily";

    public void StartGame()
    {
        SceneManager.LoadScene(storySceneName);
    }

    public void OnNextPressed()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}