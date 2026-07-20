using System.Collections;
using UnityEngine;

public class ManaTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Mana Tutorial Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string manaMessage =
        "Mana is consumed when attacking.\n\n" +
        "Press H to consume a Mana Gem.";

    [Header("Tutorial Complete Message")]
    [TextArea(2, 4)]
    [SerializeField]
    private string completeMessage =
        "Tutorial Complete!\n\n" +
        "Good luck!";

    [Header("Settings")]
    [SerializeField] private float manaMessageDisplayTime = 6f;
    [SerializeField] private float completeMessageDisplayTime = 4f;

    private bool manaTutorialHasShown;
    private bool waitingForManaRecovery;
    private bool tutorialCompleteHasShown;

    private void Awake()
    {
        // 如果 Inspector 没有手动拖入，就自动寻找同一物体上的 TutorialUI
        if (tutorialUI == null)
        {
            tutorialUI = GetComponent<TutorialUI>();
        }
    }

    private void Update()
    {
        /*
         * ==================== 测试代码 ====================
         * M键：模拟“玩家已经打败敌人，战斗结束”。
         *
         * 正式整合时：
         * 删除下面整个 if 代码块。
         * 由负责敌人系统的同学在敌人死亡时调用：
         *
         * manaTutorial.ShowManaTutorial();
         * =================================================
         */
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowManaTutorial();
        }

        /*
         * H键本身恢复Mana的功能已经在PlayerManaUI中测试。
         * 这里不负责加Mana，只负责等待恢复动作发生。
         *
         * 目前为了测试，按H后等待一小段时间，再显示教程完成。
         * 正式整合时可以由PlayerManaUI恢复Mana后，
         * 直接调用NotifyManaRecovered()。
         */
        if (waitingForManaRecovery && Input.GetKeyDown(KeyCode.H))
        {
            NotifyManaRecovered();
        }
    }

    /// <summary>
    /// 在敌人死亡、战斗结束以后调用。
    /// 不要在第一次扣除Mana时调用，否则会打断战斗教学。
    /// </summary>
    public void ShowManaTutorial()
    {
        if (manaTutorialHasShown)
        {
            return;
        }

        if (tutorialUI == null)
        {
            Debug.LogWarning(
                "ManaTutorial: TutorialUI was not found."
            );
            return;
        }

        manaTutorialHasShown = true;
        waitingForManaRecovery = true;

        tutorialUI.ShowMessage(
            manaMessage,
            manaMessageDisplayTime
        );
    }

    /// <summary>
    /// 玩家使用Mana Gem并恢复Mana后调用。
    /// </summary>
    public void NotifyManaRecovered()
    {
        if (!waitingForManaRecovery || tutorialCompleteHasShown)
        {
            return;
        }

        waitingForManaRecovery = false;
        tutorialCompleteHasShown = true;

        StartCoroutine(ShowCompleteMessage());
    }

    private IEnumerator ShowCompleteMessage()
    {
        // 给Mana恢复和血条动画留一点时间
        yield return new WaitForSeconds(0.4f);

        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                completeMessage,
                completeMessageDisplayTime
            );
        }
    }
}
