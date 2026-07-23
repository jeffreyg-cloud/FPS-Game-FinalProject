using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCompleteUI : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene("Emily");
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