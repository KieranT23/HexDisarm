using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class AnalyticsManager : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// Used to find out what the first action the user completes is
    /// </summary>
    public enum FirstAction
    {
        Continue,
        Random,
        Settings,
        Feedback,
        RemoveAds
    }
    /// <summary>
    /// The action the user completes on the feedback popup
    /// </summary>
    public enum FeedbackAction
    {
        Reviewed,
        Emailed,
        Closed
    }
    /// <summary>
    /// The action the user completes on the random popup
    /// </summary>
    public enum RandomPopupAction
    {
        Yes,
        No
    }
    #endregion

    #region Variables
    #region Static
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static AnalyticsManager Instance;
    #endregion

    #region Private
    /// <summary>
    /// Check if the user has logged completed
    /// </summary>
    private bool hasLoggedCompleted;
    /// <summary>
    /// Check if the user has logged level select
    /// </summary>
    private bool hasLoggedLevelSelect;
    /// <summary>
    /// Check if the user has logged skipped level
    /// </summary>
    private bool hasLoggedSkipped;
    /// <summary>
    /// Check if the user has logged failed level
    /// </summary>
    private bool hasLoggedFailed;
    /// <summary>
    /// Check if the user has logged quit level
    /// </summary>
    private bool hasLoggedQuit;
    /// <summary>
    /// Check if the user has logged first action
    /// </summary>
    private bool hasLoggedFirstAction;
    /// <summary>
    /// Check if the user has logged feedback
    /// </summary>
    private bool hasLoggedFeedback;
    /// <summary>
    /// Check if the user has logged the random popup action
    /// </summary>
    private bool hasLoggedRandomPopup;
    /// <summary>
    /// Check if the user has logged random level completeds
    /// </summary>
    private bool hasLoggedRandomLevelCompleted;
    /// <summary>
    /// Check if the user has logged random level quit
    /// </summary>
    private bool hasLoggedRandomLevelQuit;
    /// <summary>
    /// Check if the user has disabled analytics
    /// </summary>
    private bool disableAnalytics;
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        SetDisableAnalytics(PlayerInfoManager.Instance.HasDisabledAnalytics);
    }
    #endregion
    #region Public
    /// <summary>
    /// Set the has disabled analytics value
    /// </summary>
    /// <param name="value">Has disabled analytics</param>
    public void SetDisableAnalytics(bool value)
    {
        disableAnalytics = value;
    }

    /// <summary>
    /// Log the level completed event
    /// </summary>
    /// <param name="level">The level completed</param>
    /// <param name="timeTaken">The time taken to complete the level</param>
    /// <param name="tries">The amount of tries taken to complete the level</param>
    public void LogLevelCompleted(int level, int timeTaken, int tries)
    {
        if (hasLoggedCompleted || disableAnalytics)
            return;

        hasLoggedCompleted = true;
        Analytics.CustomEvent("levelCompleted", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken },
            { "tries", tries }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedCompleted = false));
    }

    /// <summary>
    /// Log if a level has been skipped from the level selection
    /// </summary>
    /// <param name="level">The level skipped</param>
    public void LogLevelSkippedLevelSelect(int level)
    {
        if (hasLoggedLevelSelect || disableAnalytics)
            return;

        hasLoggedLevelSelect = true;
        Analytics.CustomEvent("levelSkippedLevelSelect", new Dictionary<string, object>
        {
            { "level", level }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedLevelSelect = false));
    }
    /// <summary>
    /// Log if a level has been skipped
    /// </summary>
    /// <param name="level">The level to skip</param>
    /// <param name="timeTaken">The time taken on that level</param>
    /// <param name="tries">The amount of tries taken on that level</param>
    public void LogLevelSkipped(int level, int timeTaken, int tries)
    {
        if (hasLoggedSkipped || disableAnalytics)
            return;

        hasLoggedSkipped = true;
        Analytics.CustomEvent("levelSkipped", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken },
            { "tries", tries }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedSkipped = false));
    }
    /// <summary>
    /// Log if a level has been failed
    /// </summary>
    /// <param name="level">The level that has been failed</param>
    /// <param name="timeTaken">The time taken to complete the level</param>
    /// <param name="bombsDisarmed">The amount of bombs disarmed in the level</param>
    public void LogLevelFailed(int level, int timeTaken, int bombsDisarmed)
    {
        if (hasLoggedFailed || disableAnalytics)
            return;
        hasLoggedFailed = true;
        Analytics.CustomEvent("levelFailed", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken },
            { "bombsDisarmed", bombsDisarmed }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedFailed = false));
    }
    /// <summary>
    /// Log if a level has been quit
    /// </summary>
    /// <param name="level">The level that is being quit</param>
    /// <param name="timeTaken">The time taken on that level</param>
    /// <param name="tries">The amount of tries taken on that level</param>
    public void LogLevelQuit(int level, int timeTaken, int tries)
    {
        if (hasLoggedQuit || disableAnalytics)
            return;

        hasLoggedQuit = true;
        Analytics.CustomEvent("levelQuit", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken },
            { "tries", tries }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedQuit = false));
    }
    /// <summary>
    /// The first action that the user takes when going on the app
    /// </summary>
    /// <param name="firstAction">The first action that the user completes</param>
    public void LogFirstAction(FirstAction firstAction)
    {
        if (hasLoggedFirstAction || disableAnalytics)
            return;

        hasLoggedFirstAction = true;
        Analytics.CustomEvent("firstAction", new Dictionary<string, object>
        {
            { "action", firstAction.ToString() },
        });
    }
    /// <summary>
    /// The action that the user does on the feedback popup
    /// </summary>
    /// <param name="action">The action to complete on the feedback popup</param>
    public void LogFeedbackAction(FeedbackAction action)
    {
        if (hasLoggedFeedback || disableAnalytics)
            return;

        hasLoggedFeedback = true;
        Analytics.CustomEvent("feedbackPopupAction", new Dictionary<string, object>
        {
            { "action", action.ToString() }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedFeedback = false));
    }
    /// <summary>
    /// The action that the user completes on the random level popup
    /// </summary>
    /// <param name="action">The action that the user completes</param>
    public void LogRandomPopupAction(RandomPopupAction action)
    {
        if (hasLoggedRandomPopup || disableAnalytics)
            return;
        hasLoggedRandomPopup = true;
        Analytics.CustomEvent("randomPopupAction", new Dictionary<string, object>
        {
            { "action", action.ToString() }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedRandomPopup = false));
    }
    /// <summary>
    /// Log a random level completed
    /// </summary>
    /// <param name="level">The level that has been completed</param>
    /// <param name="timeTaken">The time taken on that level</param>
    public void LogRandomLevelCompleted(int level, int timeTaken)
    {
        if (hasLoggedRandomLevelCompleted || disableAnalytics)
            return;

        hasLoggedRandomLevelCompleted = true;
        Analytics.CustomEvent("randomLevelCompleted", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedRandomLevelCompleted = false));
    }
    /// <summary>
    /// Log a random level quit action
    /// </summary>
    /// <param name="level">The level that they quit on</param>
    /// <param name="timeTaken">The time taken on that level</param>
    /// <param name="amountOfLevelsCompleted">The amount of levels they have completed this session</param>
    public void LogRandomLevelQuit(int level, int timeTaken, int amountOfLevelsCompleted)
    {
        if (hasLoggedRandomLevelQuit || disableAnalytics)
            return;

        hasLoggedRandomLevelQuit = true;
        Analytics.CustomEvent("randomLevelQuit", new Dictionary<string, object>
        {
            { "level", level },
            { "timeTaken", timeTaken },
            { "amountOfLevelsCompleted", amountOfLevelsCompleted }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedRandomLevelQuit = false));
    }
    #endregion
    #region Private
    /// <summary>
    /// Wait before resesting a bool to make sure the events are not being fired multiple times
    /// </summary>
    /// <param name="actionToInvoke">The action to invoke after 2 seconds</param>
    private IEnumerator WaitBeforeResetting(UnityAction actionToInvoke)
    {
        yield return new WaitForSeconds(2f);
        actionToInvoke.Invoke();
    }
    #endregion
    #endregion
}
