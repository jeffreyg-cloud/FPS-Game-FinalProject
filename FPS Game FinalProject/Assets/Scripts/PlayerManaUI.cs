using TMPro;
using UnityEngine;

public class PlayerManaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform manaFill;
    [SerializeField] private TMP_Text manaText;

    [Header("Mana Settings")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana = 100f;

    [Header("Animation")]
    [SerializeField] private float changeSpeed = 500f;

    private float fullWidth;
    private float targetWidth;

    private void Start()
    {
        fullWidth = manaFill.sizeDelta.x;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);

        UpdateTargetWidth();
        UpdateManaText();
    }

    private void Update()
    {
        float currentWidth = manaFill.sizeDelta.x;

        float newWidth = Mathf.MoveTowards(
            currentWidth,
            targetWidth,
            changeSpeed * Time.deltaTime
        );

        manaFill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            newWidth
        );

        // 仅用于 UI 测试，J = 模拟武器 1 攻击，L = 模拟武器 2 攻击，H = 模拟使用宝石恢复 
        if (Input.GetKeyDown(KeyCode.J))
        {
            UseMana(5f);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            UseMana(20f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            RecoverMana(40f);
        }
    }

    public bool UseMana(float amount)
    {
        if (currentMana < amount)
        {
            return false;
        }

        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);

        UpdateTargetWidth();
        UpdateManaText();

        return true;
    }

    public void RecoverMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);

        UpdateTargetWidth();
        UpdateManaText();
    }

    public void SetMana(float current, float maximum)
    {
        maxMana = Mathf.Max(1f, maximum);
        currentMana = Mathf.Clamp(current, 0f, maxMana);

        UpdateTargetWidth();
        UpdateManaText();
    }

    private void UpdateTargetWidth()
    {
        float manaPercent = currentMana / maxMana;
        targetWidth = fullWidth * manaPercent;
    }

    private void UpdateManaText()
    {
        manaText.text =
            Mathf.CeilToInt(currentMana)
            + " / "
            + Mathf.CeilToInt(maxMana);
    }
}