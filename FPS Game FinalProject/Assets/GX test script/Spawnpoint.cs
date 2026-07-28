using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn ID")]
    public string spawnID;

    private void Start()
    {
        if (SpawnManager.Instance == null)
            return;

        if (SpawnManager.Instance.spawnPointID != spawnID)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (cc != null)
                cc.enabled = true;
        }
    }
}