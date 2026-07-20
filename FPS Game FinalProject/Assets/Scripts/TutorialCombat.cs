using UnityEngine;

public class TutorialCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string combatMessage =
        "An enemy is nearby.\n\n" +
        "Defeat the enemy.";

    [Header("Settings")]
    [SerializeField] private float displayTime = 5f;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool hideEnemyAtStart = true;

    private bool hasTriggered;
    private Renderer[] enemyRenderers;

    private void Awake()
    {
        // 获取小怪本体及其子物体上的所有 Renderer
        enemyRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        // 游戏开始时只隐藏模型，不关闭整个 GameObject
        if (hideEnemyAtStart)
        {
            SetEnemyVisible(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered)
        {
            return;
        }

        // 只有玩家进入时才触发
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        hasTriggered = true;

        // 玩家进入范围后，小怪出现
        SetEnemyVisible(true);

        // 显示攻击教学
        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                combatMessage,
                displayTime
            );
        }
        else
        {
            Debug.LogWarning(
                "TutorialCombat: TutorialUI has not been assigned."
            );
        }
    }

    private void SetEnemyVisible(bool visible)
    {
        foreach (Renderer enemyRenderer in enemyRenderers)
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.enabled = visible;
            }
        }
    }
}