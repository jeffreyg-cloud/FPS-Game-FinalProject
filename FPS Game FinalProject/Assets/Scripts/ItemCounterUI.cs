using TMPro;
using UnityEngine;

public class ItemCounterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text mushroomText;

    [SerializeField] private TMP_Text gemText;

    [Header("Item Count")]
    private int mushroomCount = 0;

    private int gemCount = 0;



    private void Start()
    {
        UpdateUI();
    }

    public void AddMushroom()
    {
        mushroomCount++;

        UpdateUI();
    }

    public void AddGem()
    {
        gemCount++;

        UpdateUI();
    }


    private void UpdateUI()
    {
        mushroomText.text =
            "Mushroom: " + mushroomCount;


        gemText.text =
            "Gem: " + gemCount;
    }



    // ==============================
    // 测试代码区域
    // 正式整合游戏后删除以下Update函数
    // 不要使用E测试，因为E已经是正式拾取键
    // ==============================

    private void Update()
    {

     
        // 按 B 模拟玩家拾取一个 Mushroom
    
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddMushroom();
        }

     
        // 按 V 模拟玩家拾取一个 Gem
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddGem();
        }

    }
}