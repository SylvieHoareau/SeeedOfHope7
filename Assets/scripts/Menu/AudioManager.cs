using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------- Audio Source ---------")]
    [SerializeField] private AudioSource musicSource;

    [Header("--------- Playlist des musiques ---------")]
    [SerializeField] private AudioClip[] playlist;

    private int currentClipIndex = 0;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playlist.Length > 0)
        {
            musicSource.clip = playlist[currentClipIndex];
            musicSource.Play();
        }
    }

    private void Update()
    {
        if (!musicSource.isPlaying && playlist.Length > 0)
        {
            PlayNextClip();
        }
    }

    private void PlayNextClip()
    {
        currentClipIndex = (currentClipIndex + 1) % playlist.Length;
        musicSource.clip = playlist[currentClipIndex];
        musicSource.Play();
    }
}
