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

    [Header("Item Counts")]
    public int mushroomCount = 0;
    public int maxMushroomCount = 5;
    public int gemCount = 0;
    public int maxGemCount = 6;

    [Header("Checkpoint")]
    public Vector3 lastCheckpointPosition;
    public Quaternion lastCheckpointRotation;
    public bool hasCheckpoint = false;

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