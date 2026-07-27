using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music")]
    public AudioClip castleMusic;
    public AudioClip clockMusic;
    public AudioClip tunnelMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is not assigned!");
            return;
        }

        if (castleMusic == null)
        {
            Debug.LogError("Castle Music is not assigned!");
            return;
        }

        if (clockMusic == null)
        {
            Debug.LogError("Clock Music is not assigned!");
            return;
        }

        if (tunnelMusic == null)
        {
            Debug.LogError("Tunnel Music is not assigned!");
            return;
        }

        // Start with castle music
        PlayCastleMusic();
    }

    // =================================
    // CASTLE MUSIC
    // =================================
    public void PlayCastleMusic()
    {
        Debug.Log("Playing Castle Music");

        musicSource.Stop();
        musicSource.clip = castleMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // =================================
    // CLOCK MUSIC
    // =================================
    public void PlayClockMusic()
    {
        Debug.Log("Playing Clock Music");

        musicSource.Stop();
        musicSource.clip = clockMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // =================================
    // TUNNEL MUSIC
    // =================================
    public void PlayTunnelMusic()
    {
        Debug.Log("Playing Tunnel Music");

        musicSource.Stop();
        musicSource.clip = tunnelMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}