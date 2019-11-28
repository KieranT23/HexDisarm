using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;

public class AudioManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// A static instance of this script
    /// </summary>
    public static AudioManager Instance;

    /// <summary>
    /// The music files to use in this game
    /// </summary>
    [SerializeField]
    private AudioClip[] musicFiles;
    /// <summary>
    /// The music source to play the music from
    /// </summary>
    [SerializeField]
    private AudioSource musicSource;

    /// <summary>
    /// The music file that is currently being played
    /// </summary>
    private int currentlyPlayingMusic = 0;
    #endregion
    #region Methods

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

    /// <summary>
    /// Play a music file
    /// </summary>
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
    /// <summary>
    /// Toggle whether the music is on or off
    /// </summary>
    /// <param name="on">Is the music on?</param>
    public void ToggleMusic(bool on)
    {
        musicSource.volume = on ? 1f : 0f;
    }
    #endregion
}
