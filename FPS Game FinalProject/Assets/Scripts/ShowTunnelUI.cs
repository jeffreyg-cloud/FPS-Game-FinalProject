using UnityEngine;

public class ShowTunnelUI : MonoBehaviour
{
    public GameObject tunnelObjectiveCanvas;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the teleport trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER DETECTED!");

            if (tunnelObjectiveCanvas != null)
            {
                tunnelObjectiveCanvas.SetActive(true);

                Debug.Log("TUNNEL UI ACTIVATED!");
            }
            else
            {
                Debug.LogError("Tunnel Objective Canvas is NOT assigned!");
            }
        }
    }
}