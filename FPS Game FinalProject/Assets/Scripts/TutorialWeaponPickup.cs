using System.Collections;
using UnityEngine;
public class TutorialWeaponPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private WeaponUI weaponUI;
    [SerializeField] private WandManager wandManager;
    [SerializeField] private int wandIndex = 0;       // index into WandManager.wands for THIS weapon
    [SerializeField] private int weaponUISlot = 1;    // which WeaponUI slot number this unlocks (1, 2, ...)
    [Header("Messages")]
    [TextArea(2, 4)]
    [SerializeField]
    private string nearbyMessage =
        "You found your first magic staff.\n" +
        "Press E to pick it up.";
    [TextArea(2, 4)]
    [SerializeField]
    private string obtainedMessage =
        "Arcane Whisper acquired.";
    [TextArea(2, 4)]
    [SerializeField]
    private string attackMessage =
        "Left Click to cast your spell.";
    [Header("Timing")]
    [SerializeField] private float obtainedDisplayTime = 2.5f;
    [SerializeField] private float attackDisplayTime = 4f;
    private bool playerNearby;
    private bool collected;
    private MeshRenderer staffRenderer;
    private Collider staffCollider;
    private void Awake()
    {
        staffRenderer = GetComponent<MeshRenderer>();
        staffCollider = GetComponent<Collider>();
    }
    private void Update()
    {
        if (!playerNearby || collected)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CollectWeapon();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }
        playerNearby = true;
        tutorialUI.ShowMessage(
            nearbyMessage,
            0f
        );
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }
        playerNearby = false;
        if (!collected)
        {
            tutorialUI.HideMessage();
        }
    }
    private void CollectWeapon()
    {
        collected = true;
        playerNearby = false;
        // Hide everything visual under this wand: the staff mesh itself,
        // plus all child effects (particles, lights) like Flower Rain,
        // Toon Water Shield, Spot Lights, etc.
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        foreach (Light lightSource in GetComponentsInChildren<Light>())
        {
            lightSource.enabled = false;
        }
        // Turn off pickup range so it can't be triggered again
        if (staffCollider != null)
        {
            staffCollider.enabled = false;
        }
        // Unlock this weapon's UI slot
        weaponUI.UnlockWeapon(weaponUISlot);
        // Actually reveal + equip the wand in the player's hand
        if (wandManager != null)
        {
            wandManager.UnlockWand(wandIndex);
        }
        // Continue playing the pickup + attack tutorial sequence
        StartCoroutine(WeaponTutorialSequence());
    }
    private IEnumerator WeaponTutorialSequence()
    {
        tutorialUI.ShowMessage(
            obtainedMessage,
            obtainedDisplayTime
        );
        yield return new WaitForSeconds(
            obtainedDisplayTime + 0.4f
        );
        tutorialUI.ShowMessage(
            attackMessage,
            attackDisplayTime
        );
    }
}