using UnityEngine;

// Pure data/shared references. Each wand also has ONE of the weapon
// scripts below (ArcaneWhisperWeapon or StarfallScepterWeapon) which
// owns its own firing logic.
public class WandStats : MonoBehaviour
{
    [Header("Common")]
    public float damage = 10f;
    public float manaCost = 5f;
    public Transform firePoint;
    public GameObject effectPrefab; // muzzle/cast visual, optional
    public Animator wandAnimator;
    public WandManager wandManager;
    public LayerMask hittableLayers = ~0;

    [Header("Aiming")]
    public Transform camTrans; // drag your player camera here
    public float maxAimDistance = 100f;
}