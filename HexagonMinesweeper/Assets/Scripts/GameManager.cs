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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void DisarmBomb()
    {
        BombsToDestroy--;
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        if (BombsToDestroy <= 0)
        {
            if (GridGenerator.Instance.IsGeneratingGrid)
                return;

            Debug.Log("You win!");
            CurrentLevel++;
            PlayerInfoManager.Instance.LevelsUnlocked = CurrentLevel;
            StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            StartLevel(CurrentLevel);
        }
    }

    public void SetGameOver()
    {
        Debug.Log("You lose!");
    }

    public void StartLevel(int level)
    {
        CurrentLevel = level;
        levelInfo = Levels.AllLevels[level];
        StartCoroutine(GridGenerator.Instance.GenerateGrid(levelInfo));
        GridRadius = levelInfo[0];
        BombsToDestroy = levelInfo[1];
        UIController.Instance.UpdateBombsRemaining(BombsToDestroy);
        UIController.Instance.UpdateLevel(CurrentLevel);
        UIController.Instance.Show(true);
    }
}
