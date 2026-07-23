using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Wand Data")]
    public bool[] unlockedWands;
    public int activeWand;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Number of wands in your game
            unlockedWands = new bool[2];

            // Player starts with no wand
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