using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private GameObject weaponSlot1Object;
    [SerializeField] private GameObject weaponSlot2Object;

    [SerializeField] private Image weaponSlot1Image;
    [SerializeField] private Image weaponSlot2Image;

    [Header("Highlight Settings")]
    [SerializeField]
    private Color selectedColor =
        new Color(0.95f, 0.45f, 0.95f, 1f);

    [SerializeField]
    private Color unselectedColor =
        new Color(0.35f, 0.20f, 0.40f, 0.75f);

    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float normalScale = 1f;

    [Header("Test Settings")]
    [SerializeField] private bool hasWeapon1 = false;
    [SerializeField] private bool hasWeapon2 = false;

    private int currentWeaponIndex = 0;

    private void Start()
    {
        RefreshWeaponUI();
    }

    private void Update()
    {
        // 以下按键只用于在 UITestScene 测试

        // P：模拟拾取第一根魔法棒
        if (Input.GetKeyDown(KeyCode.P))
        {
            UnlockWeapon(1);
        }

        // O：模拟拾取第二根魔法棒
        if (Input.GetKeyDown(KeyCode.O))
        {
            UnlockWeapon(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(2);
        }
    }

    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex == 1)
        {
            hasWeapon1 = true;

            // 第一把武器获得后，自动装备
            if (currentWeaponIndex == 0)
            {
                currentWeaponIndex = 1;
            }
        }
        else if (weaponIndex == 2)
        {
            hasWeapon2 = true;

            // 拾取第二把后是否自动装备，可按策划调整
            currentWeaponIndex = 2;
        }

        RefreshWeaponUI();
    }

    public void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex == 1 && !hasWeapon1)
        {
            return;
        }

        if (weaponIndex == 2 && !hasWeapon2)
        {
            return;
        }

        currentWeaponIndex = weaponIndex;
        RefreshWeaponUI();
    }

    public void SetWeaponOwnership(
        bool ownsWeapon1,
        bool ownsWeapon2,
        int selectedWeapon
    )
    {
        hasWeapon1 = ownsWeapon1;
        hasWeapon2 = ownsWeapon2;

        if (selectedWeapon == 1 && hasWeapon1)
        {
            currentWeaponIndex = 1;
        }
        else if (selectedWeapon == 2 && hasWeapon2)
        {
            currentWeaponIndex = 2;
        }
        else if (hasWeapon1)
        {
            currentWeaponIndex = 1;
        }
        else
        {
            currentWeaponIndex = 0;
        }

        RefreshWeaponUI();
    }

    private void RefreshWeaponUI()
    {
        weaponSlot1Object.SetActive(hasWeapon1);
        weaponSlot2Object.SetActive(hasWeapon2);

        if (hasWeapon1)
        {
            bool selected = currentWeaponIndex == 1;

            weaponSlot1Image.color =
                selected ? selectedColor : unselectedColor;

            weaponSlot1Image.rectTransform.localScale =
                Vector3.one * (selected ? selectedScale : normalScale);
        }

        if (hasWeapon2)
        {
            bool selected = currentWeaponIndex == 2;

            weaponSlot2Image.color =
                selected ? selectedColor : unselectedColor;

            weaponSlot2Image.rectTransform.localScale =
                Vector3.one * (selected ? selectedScale : normalScale);
        }
    }
}