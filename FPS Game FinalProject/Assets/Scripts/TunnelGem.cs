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
        if (collected)
            return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Play collection sound
            if (collectSound != null && soundManager != null)
            {
                soundManager.PlayOneShot(
                    collectSound,
                    collectVolume
                );
            }

            // Update collection count
            TunnelGemManager.Instance.CollectGem();

            // Destroy watch
            Destroy(gameObject);
        }
    }
}