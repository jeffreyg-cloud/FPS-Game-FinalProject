using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class KeyPickup : MonoBehaviour
{
    [Header("Tutorial Reference")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string collectMessage =
        "You collected the Gate Key!\n" +
        "You can now open the gate.";

    [Header("Sound")]
    [SerializeField] private AudioClip pickupSound;
    [Range(0f, 3f)][SerializeField] private float pickupSoundVolume = 1f; // can boost above normal

    private bool playerNearby;
    private PlayerKey playerKey;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound, plays at full volume regardless of distance
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Give the player the key
            if (playerKey != null)
            {
                playerKey.hasKey = true;
                Debug.Log("Player collected the key!");
            }

            tutorialUI.ShowMessage(collectMessage, 5f);

            // Hide the key immediately (visual + collider), but don't destroy
            // the object yet so the pickup sound can finish playing
            GetComponent<Collider>().enabled = false;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            if (pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound, pickupSoundVolume);
                Destroy(gameObject, pickupSound.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerNearby = true;
            playerKey = player.GetComponent<PlayerKey>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = false;
            playerKey = null;
        }
    }
}