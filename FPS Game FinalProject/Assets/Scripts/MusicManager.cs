using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music")]
    public AudioClip castleMusic;
    public AudioClip tunnelMusic;

    private void Awake()
    {
        Instance = this;
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

        if (tunnelMusic == null)
        {
            Debug.LogError("Tunnel Music is not assigned!");
            return;
        }

        PlayCastleMusic();
    }

    public void PlayCastleMusic()
    {
        Debug.Log("Playing Castle Music");

        musicSource.Stop();
        musicSource.clip = castleMusic;
        musicSource.Play();
    }

    public void PlayTunnelMusic()
    {
        Debug.Log("Playing Tunnel Music");

        musicSource.Stop();
        musicSource.clip = tunnelMusic;
        musicSource.Play();
    }
}