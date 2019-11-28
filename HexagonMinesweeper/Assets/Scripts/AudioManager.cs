using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip[] musicFiles;

    [SerializeField] private AudioSource musicSource;

    private int currentlyPlayingMusic = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
            PlayMusic();
    }

    private void PlayMusic()
    {
        if (musicFiles.Length == 0 || !PlayerInfoManager.Instance.MusicOn)
            return;

        int musicToPlay = 0;

        do
        {
            musicToPlay = Random.Range(0, musicFiles.Length);
        } while (musicToPlay == currentlyPlayingMusic);

        currentlyPlayingMusic = musicToPlay;

        musicSource.PlayOneShot(musicFiles[musicToPlay]);
    }

    public void ToggleMusic(bool on)
    {
        musicSource.volume = on ? 1f : 0f;
    }
}
