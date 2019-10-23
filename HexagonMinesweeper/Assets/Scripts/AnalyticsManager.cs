using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private bool hasLoggedCompleted;
    private bool hasLoggedLevelSelect;
    private bool hasLoggedSkipped;
    private bool hasLoggedFailed;
    private bool hasLoggedQuit;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void LogLevelCompleted(int level, int timeTaken, int tries)
    {
        if (hasLoggedCompleted)
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

    public void LogLevelSkippedLevelSelect(int level)
    {
        if (hasLoggedLevelSelect)
            return;
        hasLoggedLevelSelect = true;
        Analytics.CustomEvent("levelSkippedLevelSelect", new Dictionary<string, object>
        {
            { "level", level }
        });
        StartCoroutine(WaitBeforeResetting(() => hasLoggedLevelSelect = false));
    }

    public void LogLevelSkipped(int level, int timeTaken, int tries)
    {
        if (hasLoggedSkipped)
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

    public void LogLevelFailed(int level, int timeTaken, int bombsDisarmed)
    {
        if (hasLoggedFailed)
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

    public void LogLevelQuit(int level, int timeTaken, int tries)
    {
        if (hasLoggedQuit)
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

    private IEnumerator WaitBeforeResetting(UnityAction actionToInvoke)
    {
        yield return new WaitForSeconds(2f);
        actionToInvoke.Invoke();
    }
}
