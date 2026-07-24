using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastCheckpointPosition = transform.position;
            GameManager.Instance.lastCheckpointRotation = transform.rotation;
            GameManager.Instance.hasCheckpoint = true;
        }
    }
}