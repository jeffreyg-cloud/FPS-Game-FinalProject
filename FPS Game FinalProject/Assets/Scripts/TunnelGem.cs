using UnityEngine;

public class TunnelGem : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            TunnelGemManager.Instance.CollectGem();

            Destroy(gameObject);
        }
    }
}