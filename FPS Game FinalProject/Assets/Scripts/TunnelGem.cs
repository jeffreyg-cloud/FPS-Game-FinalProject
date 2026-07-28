
using UnityEngine;

public class TunnelGem : MonoBehaviour
{
    [Header("Collection Sound")]
    public AudioClip collectSound;
    public AudioSource soundManager;

    [Range(0f, 2f)]
    public float collectVolume = 1f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Prevent collecting twice
        if (collected)
            return;

        // Check Player
        if (other.CompareTag("Player"))
        {
            collected = true;

            // ==========================================
            // PLAY COLLECTION SOUND
            // ==========================================

            if (collectSound != null && soundManager != null)
            {
                soundManager.PlayOneShot(
                    collectSound,
                    collectVolume
                );
            }

            // ==========================================
            // UPDATE GEM / WATCH COUNT
            // ==========================================

            if (TunnelGemManager.Instance != null)
            {
                TunnelGemManager.Instance.CollectGem();
            }
            else
            {
                Debug.LogError(
                    "TunnelGemManager.Instance is NULL!"
                );
            }

            // ==========================================
            // DESTROY WATCH
            // ==========================================

            Destroy(gameObject);
        }
    }
}
