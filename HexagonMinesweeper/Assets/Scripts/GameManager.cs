using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    #region Static
    /// <summary>
    /// The static instance of this class
    /// </summary>
    public static GameManager Instance;
    #endregion
    #region Public
    /// <summary>
    /// The amount of bombs to destroy
    /// </summary>
    public int BombsToDestroy { get; private set; } = 1;
    /// <summary>
    /// The grid radius
    /// </summary>
    public int GridRadius { get; private set; } = 3;
    /// <summary>
    /// The current level
    /// </summary>
    public int CurrentLevel { get; private set; } = 1;
    /// <summary>
    /// Check if it is currently a random level
    /// </summary>
    public bool IsRandomLevel { get; private set; }
    #endregion
    #region Private
    /// <summary>
    /// The current level info
    /// </summary>
    private List<int> levelInfo = new List<int>();
    /// <summary>
    /// The time that has been taken on the current level
    /// </summary>
    private float timeTakenOnLevel;
    /// <summary>
    /// The amount of tries the user has taken to complete the level
    /// </summary>
    private int triesOnLevel = 1;
    /// <summary>
    /// The amount of bombs that have been disarmed on this level
    /// </summary>
    private int bombsDisarmed = 0;
    /// <summary>
    /// Check if the analytics event for failing has completed
    /// </summary>
    private bool hasLoggedFailed;
    /// <summary>
    /// The amount of random levels the user has completed this session
    /// </summary>
    private int amountOfRandomLevelsCompleted = 0;
    /// <summary>
    /// Check if it is now counting random levels
    /// </summary>
    private bool hasStartedCountingRandomLevels;
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

        Input.multiTouchEnabled = false;
    }

    private void Update()
    {
        timeTakenOnLevel += Time.deltaTime;
    }
    #endregion
    #region Public
    /// <summary>
    /// Disarm a bomb
    /// </summary>
    public void DisarmBomb()
    {
        bombsDisarmed++;
        BombsToDestroy--;
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);

        //Check if the level has been completed
        if (BombsToDestroy <= 0)
        {
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

            AnalyticsManager.Instance.LogLevelCompleted(CurrentLevel, (int) timeTakenOnLevel, triesOnLevel);
            CurrentLevel++;

            if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
                PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
            if (CurrentLevel == 201)
            {
                StartCoroutine(UIController.Instance.AnimateFinalLevelFinished());
                return;
            }

            StartLevel(CurrentLevel, true);
        }
    }

    /// <summary>
    /// Trigger game over - when a user clicks on a bomb
    /// </summary>
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
            StartRandomLevel(false);
        
    }

    /// <summary>
    /// Skip the current levle
    /// </summary>
    public void SkipLevel()
    {
        AnalyticsManager.Instance.LogLevelSkipped(CurrentLevel, (int) timeTakenOnLevel, triesOnLevel);
        CurrentLevel++;

        if (CurrentLevel > PlayerInfoManager.Instance.LevelsUnlocked)
            PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;

        StartLevel(CurrentLevel, false, true);
    }

    /// <summary>
    /// Start a random level
    /// </summary>
    /// <param name="levelFinished">Has a level been finished?</param>
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

        //Randomly generate the level
        int gridSize = Random.Range(6, 9);
        int bombs = Random.Range(4, 8);
        List<int> levelInfo =  new List<int> {gridSize, bombs};

        StartCoroutine(GridGenerator3D.Instance.GenerateGrid(levelInfo, levelFinished));
        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
        hasLoggedFailed = false;
    }

    /// <summary>
    /// Start a level
    /// </summary>
    /// <param name="level">The level to start</param>
    /// <param name="levelFinished">Has a level been finished?</param>
    /// <param name="levelSkipped">Has a level been skipped</param>
    public void StartLevel(int level, bool levelFinished = false, bool levelSkipped = false)
    {
        IsRandomLevel = false;
        triesOnLevel = 1;
        timeTakenOnLevel = 0;
        bombsDisarmed = 0;
        CurrentLevel = level;
        levelInfo = Levels.AllLevels[level];

        if (!levelSkipped)
            StartCoroutine(GridGenerator3D.Instance.GenerateGrid(levelInfo, levelFinished));
        else
            StartCoroutine(GridGenerator3D.Instance.AnimateSkip(levelInfo));

        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
        hasLoggedFailed = false;
    }

    /// <summary>
    /// Quit the current level
    /// </summary>
    public void QuitLevel()
    {
        if (IsRandomLevel)
            AnalyticsManager.Instance.LogRandomLevelQuit(CurrentLevel, (int)timeTakenOnLevel, amountOfRandomLevelsCompleted);
        else
            AnalyticsManager.Instance.LogLevelQuit(CurrentLevel, (int)timeTakenOnLevel, triesOnLevel);

        hasStartedCountingRandomLevels = false;
    }
    #endregion
    #region Private
    /// <summary>
    /// Wait before resetting the fail logging occurs again - prevents events being fired multiple times
    /// </summary>
    private IEnumerator WaitForLogReset()
    {
        yield return new WaitForSeconds(2f);
        hasLoggedFailed = true;
    }
    #endregion
    #endregion
}
