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

    public bool IsRandomLevel { get; private set; }

    private int amountOfRandomLevelsCompleted = 0;

    private bool hasStartedCountingRandomLevels;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        Input.multiTouchEnabled = false;
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
            /*if (GridGenerator.Instance.IsGeneratingGrid)
                return;*/
            if (GridGenerator3D.Instance.IsGeneratingGrid)
                return;
            if (IsRandomLevel)
            {
                amountOfRandomLevelsCompleted++;
                AnalyticsManager.Instance.LogRandomLevelCompleted(CurrentLevel, (int)timeTakenOnLevel);
                PlayerInfoManager.Instance.AmountOfCompletedRandomLevels = CurrentLevel;
                StartRandomLevel(true);
                return;
            }
                

            if (CurrentLevel == 1 || CurrentLevel == 2 || CurrentLevel == 5)
                UIController.Instance.HideCurrentlyActiveTip();

            AnalyticsManager.Instance.LogLevelCompleted(CurrentLevel, (int) timeTakenOnLevel, triesOnLevel);
            CurrentLevel++;
            if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
                PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
            //StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            if (CurrentLevel == 201)
            {
                StartCoroutine(UIController.Instance.AnimateFinalLevelFinished());
                return;
            }

            StartLevel(CurrentLevel, true);
        }
    }

    public void SetGameOver()
    {
        if (!IsRandomLevel)
        {
            if (!hasLoggedFailed)
                AnalyticsManager.Instance.LogLevelFailed(CurrentLevel, (int)timeTakenOnLevel, bombsDisarmed);
            hasLoggedFailed = true;
            triesOnLevel++;
            StartLevel(CurrentLevel);
            if (CurrentLevel >= PlayerInfoManager.Instance.LevelsUnlocked)
                UIController.Instance.ShowSkipButton();
        }
        else
        {
            StartRandomLevel(false);
        }
        
    }

    public void SkipLevel()
    {
        AnalyticsManager.Instance.LogLevelSkipped(CurrentLevel, (int) timeTakenOnLevel, triesOnLevel);
        CurrentLevel++;
        if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
            PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
        StartLevel(CurrentLevel, false, true);
    }

    public void StartRandomLevel(bool levelFinished)
    {
        if (!hasStartedCountingRandomLevels)
            amountOfRandomLevelsCompleted = 0;
        hasStartedCountingRandomLevels = true;
        IsRandomLevel = true;
        triesOnLevel = 1;
        timeTakenOnLevel = 0;
        bombsDisarmed = 0;
        CurrentLevel = PlayerInfoManager.Instance.AmountOfCompletedRandomLevels + 1;
        Random.InitState(System.Environment.TickCount);
        //grid size, bombs, bombsToShow, seeds
        int gridSize = Random.Range(6, 9);
        int bombs = Random.Range(4, 8);
        List<int> levelInfo =  new List<int> {gridSize, bombs};
        if (!levelFinished)
            StartCoroutine(GridGenerator3D.Instance.AnimateGridIntoView());
        //StartCoroutine(GridGenerator.Instance.GenerateGrid(levelInfo, levelFinished));
        StartCoroutine(GridGenerator3D.Instance.GenerateGrid(levelInfo, levelFinished));
        //GridGenerator3D.Instance.SetBackgroundColours(false, true);
        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
        hasLoggedFailed = false;
    }

    public void StartLevel(int level, bool levelFinished = false, bool levelSkipped = false)
    {
        //LevelSelection.Instance.gameObject.SetActive(false);
        IsRandomLevel = false;
        triesOnLevel = 1;
        timeTakenOnLevel = 0;
        bombsDisarmed = 0;
        CurrentLevel = level;
        levelInfo = Levels.AllLevels[level];
        /*if (!levelSkipped)
            StartCoroutine(GridGenerator.Instance.GenerateGrid(levelInfo, levelFinished));
        else
        {
            StartCoroutine(GridGenerator.Instance.AnimateSkip(levelInfo));
        }*/
        if (!levelSkipped)
            StartCoroutine(GridGenerator3D.Instance.GenerateGrid(levelInfo, levelFinished));
        else
        {
            StartCoroutine(GridGenerator3D.Instance.AnimateSkip(levelInfo));
        }

        if (!levelFinished && !levelSkipped)
            StartCoroutine(GridGenerator3D.Instance.AnimateGridIntoView());
        //GridGenerator3D.Instance.SetBackgroundColours(false, true);


        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
        hasLoggedFailed = false;
    }

    public void QuitLevel()
    {
        if (IsRandomLevel)
            AnalyticsManager.Instance.LogRandomLevelQuit(CurrentLevel, (int)timeTakenOnLevel, amountOfRandomLevelsCompleted);
        else
            AnalyticsManager.Instance.LogLevelQuit(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);

        hasStartedCountingRandomLevels = false;
    }

    public void FinishTutorialLevel()
    {
        
        AnalyticsManager.Instance.LogLevelCompleted(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);
        CurrentLevel++;
        if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
            PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
        //StartCoroutine(UIController.Instance.ShowCompleteLevelText());
        StartLevel(CurrentLevel, true);
    }

    private IEnumerator WaitForLogReset()
    {
        yield return new WaitForSeconds(2f);
        hasLoggedFailed = true;
    }
}
