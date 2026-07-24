using TMPro;
using UnityEngine;
public class ItemCounterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text mushroomText;
    [SerializeField] private TMP_Text gemText;

    [Header("Restore Targets")]
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private PlayerManaUI playerManaUI;

    [Header("Restore Amounts")]
    [SerializeField] private float mushroomHealAmount = 30f;
    [SerializeField] private float gemManaAmount = 40f;

    private int mushroomCount = 0;
    private int gemCount = 0;
    private int maxMushroomCount = 5;
    private int maxGemCount = 6;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            mushroomCount = GameManager.Instance.mushroomCount;
            gemCount = GameManager.Instance.gemCount;
            maxMushroomCount = GameManager.Instance.maxMushroomCount;
            maxGemCount = GameManager.Instance.maxGemCount;
        }
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            UseMushroom();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            UseGem();
        }
    }

    public bool IsMushroomFull()
    {
        return mushroomCount >= maxMushroomCount;
    }
    public bool IsGemFull()
    {
        return gemCount >= maxGemCount;
    }

    // Returns true if the mushroom was actually added (false if pouch was full)
    public bool AddMushroom()
    {
        if (IsMushroomFull())
        {
            return false;
        }
        mushroomCount++;
        SaveToGameManager();
        UpdateUI();
        return true;
    }

    // Returns true if the gem was actually added (false if pouch was full)
    public bool AddGem()
    {
        if (IsGemFull())
        {
            return false;
        }
        gemCount++;
        SaveToGameManager();
        UpdateUI();
        return true;
    }

    // Returns true if a mushroom was consumed and HP restored (false if pouch was empty)
    public bool UseMushroom()
    {
        if (mushroomCount <= 0) return false;

        mushroomCount--;
        SaveToGameManager();
        UpdateUI();

        if (playerHealthUI != null)
            playerHealthUI.Heal(mushroomHealAmount);

        return true;
    }

    // Returns true if a gem was consumed and mana restored (false if pouch was empty)
    public bool UseGem()
    {
        if (gemCount <= 0) return false;

        gemCount--;
        SaveToGameManager();
        UpdateUI();

        if (playerManaUI != null)
            playerManaUI.RecoverMana(gemManaAmount);

        return true;
    }

    private void SaveToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.mushroomCount = mushroomCount;
            GameManager.Instance.gemCount = gemCount;
        }
    }

    private void UpdateUI()
    {
        mushroomText.text = "Mushroom: " + mushroomCount + " / " + maxMushroomCount;
        gemText.text = "Gem: " + gemCount + " / " + maxGemCount;
    }
}