using UnityEngine;

public class TeleportPoint1Music : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayClockMusic();   // Change to the music you want
        }
    }
}