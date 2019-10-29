using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int BombsToDestroy { get; private set; } = 1;

    public int GridRadius { get; private set; } = 3;

    public int CurrentLevel { get; private set; } = 1;

    private List<int> levelInfo = new List<int>();

    private float timeTakenOnLevel;

    private int triesOnLevel = 1;

    private int bombsDisarmed = 0;

    private bool hasLoggedFailed;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        timeTakenOnLevel += Time.deltaTime;
    }

    public void DisarmBomb()
    {
        bombsDisarmed++;
        BombsToDestroy--;
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        if (BombsToDestroy <= 0)
        {
            if (GridGenerator.Instance.IsGeneratingGrid)
                return;

            AnalyticsManager.Instance.LogLevelCompleted(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);
            CurrentLevel++;
            if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
                PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
            //StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            StartLevel(CurrentLevel, true);
        }
    }

    public void SetGameOver()
    {
        if (!hasLoggedFailed)
            AnalyticsManager.Instance.LogLevelFailed(CurrentLevel, (int)timeTakenOnLevel, bombsDisarmed);
        hasLoggedFailed = true;
        triesOnLevel++;
        StartLevel(CurrentLevel);
        if (CurrentLevel >= PlayerInfoManager.Instance.LevelsUnlocked)
            UIController.Instance.ShowSkipButton();
    }

    public void SkipLevel()
    {
        AnalyticsManager.Instance.LogLevelSkipped(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);
        CurrentLevel++;
        if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
            PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
        StartLevel(CurrentLevel, false, true);
    }

    public void StartLevel(int level, bool levelFinished = false, bool levelSkipped = false)
    {
        triesOnLevel = 1;
        timeTakenOnLevel = 0;
        bombsDisarmed = 0;
        CurrentLevel = level;
        levelInfo = Levels.AllLevels[level];
        if (!levelSkipped)
            StartCoroutine(GridGenerator.Instance.GenerateGrid(levelInfo, levelFinished));
        else
        {
            StartCoroutine(GridGenerator.Instance.AnimateSkip(levelInfo));
        }
        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
        hasLoggedFailed = false;
    }

    public void QuitLevel()
    {
        AnalyticsManager.Instance.LogLevelQuit(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);
    }

    private IEnumerator WaitForLogReset()
    {
        yield return new WaitForSeconds(2f);
        hasLoggedFailed = true;
    }
}
