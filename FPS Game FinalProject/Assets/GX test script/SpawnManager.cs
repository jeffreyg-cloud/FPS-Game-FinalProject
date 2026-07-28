using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public string spawnPointID = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("SpawnManager Ready");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        Debug.Log("Wanted Spawn: " + spawnPointID);

        SpawnPoint[] points = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        Debug.Log("Spawn Points Found: " + points.Length);

        foreach (SpawnPoint point in points)
        {
            Debug.Log("Checking: " + point.spawnID);

            if (point.spawnID == spawnPointID)
            {
                Debug.Log("MATCH FOUND!");

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player == null)
                {
                    Debug.LogError("PLAYER NOT FOUND!");
                    return;
                }

                Debug.Log("Moving Player");

                player.transform.position = point.transform.position;
                player.transform.rotation = point.transform.rotation;

                spawnPointID = "";

                return;
            }
        }

        Debug.LogError("Spawn Point NOT FOUND!");
    }
}