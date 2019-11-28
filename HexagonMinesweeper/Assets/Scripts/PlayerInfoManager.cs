using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    #region Variables
    #region Static
    /// <summary>
    /// The static instance of this class
    /// </summary>
    public static PlayerInfoManager Instance;
    #endregion

    #region Consts
    /// <summary>
    /// The player pref key for levels unlocked
    /// </summary>
    private const string KEY_LEVELS_UNLOCKED = "levelsUnlocked";
    /// <summary>
    /// The player pref key for times generated
    /// </summary>
    private const string KEY_TIMES_GENERATED_FEEDBACK = "timesGeneratedFeedback";
    /// <summary>
    /// The player pref key for music
    /// </summary>
    private const string KEY_MUSIC_ON = "musicOn";
    /// <summary>
    /// The player pref key for vibration
    /// </summary>
    private const string KEY_VIBRATION_ON = "vibrationOn";
    /// <summary>
    /// The player pref key for whether the user has removed ads
    /// </summary>
    private const string KEY_HAS_REMOVED_ADS = "hasRemovedAds";
    /// <summary>
    /// The player pref key for whether the user has provided feedback
    /// </summary>
    private const string KEY_HAS_PROVIDED_FEEDBACK = "hasProvidedFeedback";
    /// <summary>
    /// The player pref key for random levels completed
    /// </summary>
    private const string KEY_RANDOM_LEVELS_COMPLETED = "randomLevelsCompleted";
    /// <summary>
    /// The player pref key for whether the user has been shown the random levels popup
    /// </summary>
    private const string KEY_HAS_SHOWN_RANDOM_POPUP = "hasShownRandomPopup";
    /// <summary>
    /// The player pref key for whether the user has disabled analytics
    /// </summary>
    private const string KEY_HAS_DISABLED_ANALYTICS = "hasDisabledAnalytics";
    /// <summary>
    /// The player pref key for how many times the user has been shown the review popup
    /// </summary>
    private const string KEY_TIMES_SHOWN_REVIEW_POPUP = "timesShownReviewPopup";
    /// <summary>
    /// The player pref key for the current colour theme
    /// </summary>
    private const string KEY_CURRENT_COLOUR_THEME = "currentColourTheme;";
    #endregion

    #region Public
    /// <summary>
    /// How many times the grid has been generated since the user was last shown the feedback popup
    /// </summary>
    public int TimesGridGeneratedSinceFeedback
    {
        get { return PlayerPrefs.GetInt(KEY_TIMES_GENERATED_FEEDBACK);}
        set { PlayerPrefs.SetInt(KEY_TIMES_GENERATED_FEEDBACK, value); }
    }
    /// <summary>
    /// The amount of levels the user has unlocked
    /// </summary>
    public int LevelsUnlocked
    {
        get { return PlayerPrefs.GetInt(KEY_LEVELS_UNLOCKED); }
        set { PlayerPrefs.SetInt(KEY_LEVELS_UNLOCKED, value);}
    }
    /// <summary>
    /// Is the music on?
    /// </summary>
    public bool MusicOn
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_MUSIC_ON)); }
        set { PlayerPrefs.SetString(KEY_MUSIC_ON, value.ToString()); }
    }
    /// <summary>
    /// Is the vibration on?
    /// </summary>
    public bool VibrationOn
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_VIBRATION_ON)); }
        set { PlayerPrefs.SetString(KEY_VIBRATION_ON, value.ToString()); }
    }
    /// <summary>
    /// Has the user removed advertisements?
    /// </summary>
    public bool HasRemovedAds
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_REMOVED_ADS)); }
        set { PlayerPrefs.SetString(KEY_HAS_REMOVED_ADS, value.ToString()); }
    }
    /// <summary>
    /// Has the user provided feedback
    /// </summary>
    public bool HasProvidedFeedback
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_PROVIDED_FEEDBACK)); }
        set { PlayerPrefs.SetString(KEY_HAS_PROVIDED_FEEDBACK, value.ToString()); }
    }
    /// <summary>
    /// The amount of random levels the user has completed
    /// </summary>
    public int AmountOfCompletedRandomLevels
    {
        get { return (PlayerPrefs.GetInt(KEY_RANDOM_LEVELS_COMPLETED)); }
        set { PlayerPrefs.SetInt(KEY_RANDOM_LEVELS_COMPLETED, value); }
    }
    /// <summary>
    /// Has the user seen the random popup?
    /// </summary>
    public bool HasShownRandomPopup
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_SHOWN_RANDOM_POPUP)); }
        set { PlayerPrefs.SetString(KEY_HAS_SHOWN_RANDOM_POPUP, value.ToString()); }
    }
    /// <summary>
    /// Has the user disabled analytics?
    /// </summary>
    public bool HasDisabledAnalytics
    {
        get { return bool.Parse(PlayerPrefs.GetString(KEY_HAS_DISABLED_ANALYTICS)); }
        set { PlayerPrefs.SetString(KEY_HAS_DISABLED_ANALYTICS, value.ToString()); }
    }
    /// <summary>
    /// The amount of times the user has been shown the review popup
    /// </summary>
    public int AmountOfTimesShownReviewPopup
    {
        get { return PlayerPrefs.GetInt(KEY_TIMES_SHOWN_REVIEW_POPUP); }
        set { PlayerPrefs.SetInt(KEY_TIMES_SHOWN_REVIEW_POPUP, value); }
    }
    /// <summary>
    /// The current colour theme that the user is using
    /// </summary>
    public int CurrentColourTheme
    {
        get { return PlayerPrefs.GetInt(KEY_CURRENT_COLOUR_THEME); }
        set { PlayerPrefs.SetInt(KEY_CURRENT_COLOUR_THEME, value); }
    }
    #endregion
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

        if (!PlayerPrefs.HasKey(KEY_LEVELS_UNLOCKED))
            LevelsUnlocked = 1;
        if (!PlayerPrefs.HasKey(KEY_TIMES_GENERATED_FEEDBACK))
            TimesGridGeneratedSinceFeedback = 0;
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
        if (!PlayerPrefs.HasKey(KEY_CURRENT_COLOUR_THEME))
            CurrentColourTheme = 0;
    }
    #endregion
}
