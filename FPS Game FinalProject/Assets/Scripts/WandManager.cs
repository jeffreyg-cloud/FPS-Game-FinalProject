using UnityEngine;
using UnityEngine.InputSystem;
public class WandManager : MonoBehaviour
{
    public WandStats[] wands;
    public int activeIndex = 0;
    public PlayerManaUI manaUI;
    public WeaponUI weaponUI;
    public WandStats ActiveWand => wands[activeIndex];

    // Tracks which wands the player has actually picked up.
    // Sized to match `wands` automatically at Start.
    private bool[] unlocked;

    void Start()
    {
        unlocked = new bool[wands.Length];

        // Nothing is equippable until picked up, EXCEPT whichever wand(s)
        // you've manually marked unlocked in a save/inventory system later.
        // For now, everything starts locked and hidden.
        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].gameObject.SetActive(false);
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
        if (!unlocked[index]) return; // can't switch to a wand you haven't picked up yet

        wands[activeIndex].gameObject.SetActive(false);
        activeIndex = index;
        wands[activeIndex].gameObject.SetActive(true);

        if (weaponUI != null)
        {
            weaponUI.SelectWeapon(index + 1);
        }
    }

    // Call this from pickup scripts (e.g. TutorialWeaponPickup) when the
    // player picks up a new wand. Unlocks it and immediately equips it.
    public void UnlockWand(int index)
    {
        if (index < 0 || index >= wands.Length) return;

        unlocked[index] = true;

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