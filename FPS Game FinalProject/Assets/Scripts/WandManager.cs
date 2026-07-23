using UnityEngine;
using UnityEngine.InputSystem;

public class WandManager : MonoBehaviour
{
    public WandStats[] wands;
    public int activeIndex = 0;

    public PlayerManaUI manaUI;
    public WeaponUI weaponUI;

    public WandStats ActiveWand => wands[activeIndex];

    void Start()
    {
        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].gameObject.SetActive(i == activeIndex);
        }

        if (weaponUI != null)
        {
            weaponUI.SetWeaponOwnership(true, true, activeIndex + 1);
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