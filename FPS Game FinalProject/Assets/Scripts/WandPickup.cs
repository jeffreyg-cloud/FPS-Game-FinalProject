using System.Collections;
using UnityEngine;

public class WandPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private WeaponUI weaponUI;

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

        // 按下 E 后，让魔法棒立刻消失
        if (staffRenderer != null)
        {
            staffRenderer.enabled = false;
        }

        // 关闭拾取范围，避免重复触发
        if (staffCollider != null)
        {
            staffCollider.enabled = false;
        }

        // 解锁第一把武器的 UI
        weaponUI.UnlockWeapon(1);

        // 继续播放获得武器和攻击教学
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