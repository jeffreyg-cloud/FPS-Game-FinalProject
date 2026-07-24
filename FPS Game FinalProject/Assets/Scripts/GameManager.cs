using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Wand Data")]
    public bool[] unlockedWands;
    public int activeWand;

    [Header("Player Stats")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float currentMana = 100f;
    public float maxMana = 100f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            unlockedWands = new bool[2];
            unlockedWands[0] = false;
            unlockedWands[1] = false;
            activeWand = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}