using UnityEngine;
using UnityEngine.InputSystem;

public class WandManager : MonoBehaviour
{
    public WandStats[] wands;
    public int activeIndex = 0;
    public PlayerManaUI manaUI;
    public WeaponUI weaponUI;

    public WandStats ActiveWand => wands[activeIndex];

    public bool[] unlocked;

    void Start()
    {
        unlocked = new bool[wands.Length];

        // Hide all wands first
        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].gameObject.SetActive(false);
        }

        // Restore saved data from GameManager
        if (GameManager.Instance != null)
        {
            for (int i = 0; i < wands.Length; i++)
            {
                unlocked[i] = GameManager.Instance.unlockedWands[i];
            }

            activeIndex = GameManager.Instance.activeWand;

            // Enable every unlocked wand
            for (int i = 0; i < wands.Length; i++)
            {
                if (unlocked[i])
                    wands[i].gameObject.SetActive(true);
            }

            // Only the active wand should stay visible
            for (int i = 0; i < wands.Length; i++)
            {
                if (i != activeIndex)
                    wands[i].gameObject.SetActive(false);
            }

            if (unlocked[activeIndex])
                wands[activeIndex].gameObject.SetActive(true);

            if (weaponUI != null && unlocked[activeIndex])
            {
                weaponUI.SelectWeapon(activeIndex + 1);
            }
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchWand(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchWand(1);
    }

    void SwitchWand(int index)
    {
        if (index < 0 || index >= wands.Length || index == activeIndex) return;
        if (!unlocked[index]) return;

        wands[activeIndex].gameObject.SetActive(false);

        activeIndex = index;
        wands[activeIndex].gameObject.SetActive(true);

        // Save current equipped wand
        if (GameManager.Instance != null)
        {
            GameManager.Instance.activeWand = activeIndex;
        }

        if (weaponUI != null)
        {
            weaponUI.SelectWeapon(index + 1);
        }
    }

    public void UnlockWand(int index)
    {
        if (index < 0 || index >= wands.Length) return;

        unlocked[index] = true;

        // Save to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.unlockedWands[index] = true;
            GameManager.Instance.activeWand = index;
        }

        if (unlocked[activeIndex])
            wands[activeIndex].gameObject.SetActive(false);

        activeIndex = index;
        wands[activeIndex].gameObject.SetActive(true);

        if (weaponUI != null)
        {
            weaponUI.SelectWeapon(index + 1);
        }
    }

    public bool TrySpendMana(float amount)
    {
        if (manaUI == null) return true;
        return manaUI.UseMana(amount);
    }
}