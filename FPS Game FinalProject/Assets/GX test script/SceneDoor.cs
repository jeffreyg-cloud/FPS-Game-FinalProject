using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;

    [Header("Spawn Point In Next Scene")]
    public string destinationSpawnID;

    private bool loading = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.name);

        if (loading)
            return;

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Wrong tag: " + other.tag);
            return;
        }

        loading = true;

        Debug.Log("Loading scene: " + sceneToLoad);

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.spawnPointID = destinationSpawnID;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}