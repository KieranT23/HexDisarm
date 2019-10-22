using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelection Instance;

    [SerializeField] private Level levelPrefab;

    [SerializeField] private Transition transitionPrefab;

    private Dictionary<int, Level> levels = new Dictionary<int, Level>();

    private Dictionary<int, Transition> transitions = new Dictionary<int, Transition>();

    private int createdLevels = 0;

    [SerializeField] private RectTransform scrollContent;

    [SerializeField] private Color unlockedColor;

    [SerializeField] private Color lockedColor;

    [SerializeField] private Color lastUnlockedColor;

    [SerializeField] private RectTransform levelsContent;

    [SerializeField] private RectTransform transitionsContent;

    [SerializeField] private RectTransform testObject;

    private int unlockedLevels = 10;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        unlockedLevels = PlayerInfoManager.Instance.LevelsUnlocked;
        for (int i = 0; i < Levels.AllLevels.Count; i++)
        {
            CreateLevel();
        }

        scrollContent.anchoredPosition = new Vector2(0f, -testObject.anchoredPosition.y);
    }

    private void CreateLevel()
    {
        createdLevels++;
        bool isUnlocked = createdLevels <= unlockedLevels;
        bool isLastUnlockedLevel = createdLevels == unlockedLevels;

        int remainder = createdLevels % 4;
        if (remainder == 0)
            remainder += 4;
        float xMovementAmount = 369;
        float yMovementAmount = 211;

        float xPos = 0f;

        if (remainder % 2 != 0)
            xPos = (remainder - 2) * xMovementAmount;

        float yPos = (yMovementAmount * (createdLevels - 1)) + 200;
        Vector3 levelPos = new Vector3(xPos, yPos, 0f);
        Level instantiatedLevel = Instantiate(levelPrefab, levelsContent);
        ((RectTransform) instantiatedLevel.transform).anchoredPosition = levelPos;

        Color colorToSet;
        if (isLastUnlockedLevel)
            colorToSet = lastUnlockedColor;
        else if (isUnlocked)
            colorToSet = unlockedColor;
        else
            colorToSet = lockedColor;


        instantiatedLevel.Init(createdLevels, isUnlocked, isLastUnlockedLevel, colorToSet);
        levels.Add(createdLevels, instantiatedLevel);
        scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, (200 + (((RectTransform)levelPrefab.transform).sizeDelta.y / 2) + (yMovementAmount * (createdLevels - 1)) + 200));

        if (isLastUnlockedLevel)
            testObject.transform.position = instantiatedLevel.transform.position;

        if (createdLevels == 1)
            return;

        float transitionXAmount = 184.7f;

        int transitionRemainder = (createdLevels - 1) % 4;
        if (transitionRemainder == 0)
            transitionRemainder += 4;

        float transitionPosX = 0;
        float transitionPosY = (yMovementAmount * (createdLevels - 2)) + 304.9f;
        float rotation = 0f;

        float remainderMinusDivder = transitionRemainder - 2.5f;
        if (remainderMinusDivder == 0.5f || remainderMinusDivder == -0.5f)
            transitionPosX = transitionXAmount;
        else
            transitionPosX = -transitionXAmount;

        if (remainderMinusDivder > 0)
            rotation = 60f;
        else
            rotation = -60f;



        Vector3 transitionPos = new Vector3(transitionPosX, transitionPosY, 0f);
        Transition instantiatedTransition = Instantiate(transitionPrefab, transitionsContent);
        RectTransform transitionRect = (RectTransform) instantiatedTransition.transform;
        transitionRect.anchoredPosition = transitionPos;
        transitionRect.eulerAngles = new Vector3(0f, 0f, rotation);
        instantiatedTransition.Init(colorToSet);

        transitions.Add(createdLevels - 1, instantiatedTransition);
    }

    public void SetBlocksRaycast(bool blockRaycasts)
    {
        foreach (Level level in levels.Values)
            level.GetComponent<GraphicRaycaster>().enabled = false;
    }
}
