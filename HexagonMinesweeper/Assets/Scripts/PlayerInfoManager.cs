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
    private const string KEY_VIBRATION_ON = "vibrationOn";
    private const string KEY_HAS_REMOVED_ADS = "hasRemovedAds";
    private const string KEY_HAS_PROVIDED_FEEDBACK = "hasProvidedFeedback";
    private const string KEY_RANDOM_LEVELS_COMPLETED = "randomLevelsCompleted";
    private const string KEY_HAS_SHOWN_RANDOM_POPUP = "hasShownRandomPopup";
    private const string KEY_HAS_DISABLED_ANALYTICS = "hasDisabledAnalytics";
    private const string KEY_TIMES_SHOWN_REVIEW_POPUP = "timesShownReviewPopup";

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

    public bool VibrationOn
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_VIBRATION_ON)); }
        set { PlayerPrefs.SetString(KEY_VIBRATION_ON, value.ToString()); }
    }

    public bool HasRemovedAds
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_REMOVED_ADS)); }
        set { PlayerPrefs.SetString(KEY_HAS_REMOVED_ADS, value.ToString()); }
    }

    public bool HasProvidedFeedback
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_PROVIDED_FEEDBACK)); }
        set { PlayerPrefs.SetString(KEY_HAS_PROVIDED_FEEDBACK, value.ToString()); }
    }

    public int AmountOfCompletedRandomLevels
    {
        get { return (PlayerPrefs.GetInt(KEY_RANDOM_LEVELS_COMPLETED)); }
        set { PlayerPrefs.SetInt(KEY_RANDOM_LEVELS_COMPLETED, value); }
    }

    public bool HasShownRandomPopup
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_SHOWN_RANDOM_POPUP)); }
        set { PlayerPrefs.SetString(KEY_HAS_SHOWN_RANDOM_POPUP, value.ToString()); }
    }

    public bool HasDisabledAnalytics
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_DISABLED_ANALYTICS)); }
        set { PlayerPrefs.SetString(KEY_HAS_DISABLED_ANALYTICS, value.ToString()); }
    }

    public int AmountOfTimesShownReviewPopup
    {
        get { return PlayerPrefs.GetInt(KEY_TIMES_SHOWN_REVIEW_POPUP); }
        set { PlayerPrefs.SetInt(KEY_TIMES_SHOWN_REVIEW_POPUP, value); }
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
        if (!PlayerPrefs.HasKey(KEY_HAS_PROVIDED_FEEDBACK))
            HasProvidedFeedback = false;
        if (!PlayerPrefs.HasKey(KEY_RANDOM_LEVELS_COMPLETED))
            AmountOfCompletedRandomLevels = 0;
        if (!PlayerPrefs.HasKey(KEY_HAS_SHOWN_RANDOM_POPUP))
            HasShownRandomPopup = false;
        if (!PlayerPrefs.HasKey(KEY_HAS_DISABLED_ANALYTICS))
            HasDisabledAnalytics = false;
        if (!PlayerPrefs.HasKey(KEY_VIBRATION_ON))
            VibrationOn = true;
        if (!PlayerPrefs.HasKey(KEY_TIMES_SHOWN_REVIEW_POPUP))
            AmountOfTimesShownReviewPopup = 0;
    }
}
