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
    [Header("Wand Identity")]
    public int wandID; // 0 = Arcane Whisper, 1 = Starfall Scepter, etc. Must match GameManager index.

    [Header("Idle Bob")]
    public bool enableIdleBob = true;
    public float bobHeight = 0.02f;   // how far up/down it moves
    public float bobSpeed = 1.5f;     // how fast it bobs

    private Vector3 baseLocalPos;
    private float bobTimer;

    void Awake()
    {
        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (!enableIdleBob) return;

        bobTimer += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(bobTimer) * bobHeight;
        transform.localPosition = baseLocalPos + new Vector3(0f, yOffset, 0f);
    }
}