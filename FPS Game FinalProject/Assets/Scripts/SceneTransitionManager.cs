using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Next Scene")]
    public string sceneToLoad = "FinalScene";

    [Header("Delay")]
    public float delayBeforeNextScene = 2f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("SceneTransitionManager is ready!");
    }

    public void EnemyDefeated()
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        Debug.Log("FINAL ENEMY DEFEATED!");

        StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator TransitionToNextScene()
    {
        Debug.Log(
            "Final scene will load in "
            + delayBeforeNextScene
            + " seconds."
        );

        yield return new WaitForSeconds(delayBeforeNextScene);

        Debug.Log(
            "Loading final scene now!"
        );

        Debug.Log(
            "Current Scene: "
            + SceneManager.GetActiveScene().name
        );

        Debug.Log(
            "Target Scene: "
            + sceneToLoad
        );

        // Try loading scene
        SceneManager.LoadScene(sceneToLoad);

        Debug.Log(
            "LoadScene command executed!"
        );
    }
}