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

        // Hide all wands
        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].gameObject.SetActive(false);
        }

        // Restore data from GameManager
        if (GameManager.Instance != null)
        {
            for (int i = 0; i < wands.Length; i++)
            {
                unlocked[i] = GameManager.Instance.unlockedWands[i];
            }

            activeIndex = GameManager.Instance.activeWand;

            // Show the equipped wand if unlocked
            if (activeIndex >= 0 &&
                activeIndex < wands.Length &&
                unlocked[activeIndex])
            {
                wands[activeIndex].gameObject.SetActive(true);
            }

            // Update Weapon UI
            if (weaponUI != null)
            {
                weaponUI.SetWeaponOwnership(
                    unlocked.Length > 0 ? unlocked[0] : false,
                    unlocked.Length > 1 ? unlocked[1] : false,
                    activeIndex + 1
                );
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
        if (index < 0 || index >= wands.Length)
            return;

        if (index == activeIndex)
            return;

        if (!unlocked[index])
            return;

        wands[activeIndex].gameObject.SetActive(false);

        activeIndex = index;

        wands[activeIndex].gameObject.SetActive(true);

        // Save equipped wand
        if (GameManager.Instance != null)
        {
            GameManager.Instance.activeWand = activeIndex;
        }

        // Update UI
        if (weaponUI != null)
        {
            weaponUI.SetWeaponOwnership(
                unlocked.Length > 0 ? unlocked[0] : false,
                unlocked.Length > 1 ? unlocked[1] : false,
                activeIndex + 1
            );
        }
    }

    public void UnlockWand(int index)
    {
        if (index < 0 || index >= wands.Length)
            return;

        unlocked[index] = true;

        // Save to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.unlockedWands[index] = true;
            GameManager.Instance.activeWand = index;
        }

        if (unlocked[activeIndex])
        {
            wands[activeIndex].gameObject.SetActive(false);
        }

        activeIndex = index;

        wands[activeIndex].gameObject.SetActive(true);

        // Update UI
        if (weaponUI != null)
        {
            weaponUI.SetWeaponOwnership(
                unlocked.Length > 0 ? unlocked[0] : false,
                unlocked.Length > 1 ? unlocked[1] : false,
                activeIndex + 1
            );
        }
    }

    public bool TrySpendMana(float amount)
    {
        if (manaUI == null)
            return true;

        return manaUI.UseMana(amount);
    }
}