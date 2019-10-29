using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;

    private const string KEY_LEVELS_UNLOCKED = "levelsUnlocked";
    private const string KEY_TIMES_GENERATED_FEEDBACK = "timesGeneratedFeedback";
    private const string KEY_AUDIO_ON = "audioOn";
    private const string KEY_MUSIC_ON = "musicOn";
    private const string KEY_HAS_REMOVED_ADS = "hasRemovedAds";

    public int TimesGridGeneratedSinceFeedback
    {
        get { return PlayerPrefs.GetInt(KEY_TIMES_GENERATED_FEEDBACK);}
        set { PlayerPrefs.SetInt(KEY_TIMES_GENERATED_FEEDBACK, value); }
    }

    public int LevelsUnlocked
    {
        get { return PlayerPrefs.GetInt(KEY_LEVELS_UNLOCKED); }
        set { PlayerPrefs.SetInt(KEY_LEVELS_UNLOCKED, value);}
    }

    public bool AudioOn
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_AUDIO_ON)); }
        set { PlayerPrefs.SetString(KEY_AUDIO_ON, value.ToString());}
    }

    public bool MusicOn
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_MUSIC_ON)); }
        set { PlayerPrefs.SetString(KEY_MUSIC_ON, value.ToString()); }
    }

    public bool HasRemovedAds
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_REMOVED_ADS)); }
        set { PlayerPrefs.SetString(KEY_HAS_REMOVED_ADS, value.ToString()); }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }

        if (!PlayerPrefs.HasKey(KEY_LEVELS_UNLOCKED))
            LevelsUnlocked = 1;
        if (!PlayerPrefs.HasKey(KEY_TIMES_GENERATED_FEEDBACK))
            TimesGridGeneratedSinceFeedback = 0;
        if (!PlayerPrefs.HasKey(KEY_AUDIO_ON))
            AudioOn = true;
        if (!PlayerPrefs.HasKey(KEY_MUSIC_ON))
            MusicOn = true;
        if (!PlayerPrefs.HasKey(KEY_HAS_REMOVED_ADS))
            HasRemovedAds = false;
    }
}
