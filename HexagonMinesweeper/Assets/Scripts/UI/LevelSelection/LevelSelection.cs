﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
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

    [SerializeField] private Color lockedTextColor;

    [SerializeField] private Color unlockedTextColor;

    [SerializeField] private RectTransform levelsContent;

    [SerializeField] private RectTransform transitionsContent;

    [SerializeField] private RectTransform testObject;

    private int unlockedLevels = 10;

    [SerializeField] private RectTransform scrollRect;

    [SerializeField] private RectTransform returnIcon;

    [SerializeField] private Button btn_return;

    [SerializeField] private RectTransform mainCanvas;

    private CanvasGroup canvasGroup;

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

        btn_return.onClick.AddListener(() => StartCoroutine(AnimateLevelSelectToStartScreen()));

        canvasGroup = GetComponent<CanvasGroup>();
        //scrollContent.anchoredPosition = new Vector2(0f, -testObject.anchoredPosition.y);
    }

    private void CreateLevel()
    {
        createdLevels++;
        bool isUnlocked = createdLevels <= unlockedLevels;
        bool isLastUnlockedLevel = createdLevels == unlockedLevels;
        bool isNextLockedLevel = createdLevels == (unlockedLevels + 1);

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


        instantiatedLevel.Init(createdLevels, isUnlocked, isLastUnlockedLevel, isNextLockedLevel, colorToSet, !isUnlocked ? lockedTextColor : unlockedTextColor);
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

    public void Return()
    {
        scrollContent.anchoredPosition = new Vector2(0f, 0f);
        createdLevels = 0;
        unlockedLevels = PlayerInfoManager.Instance.LevelsUnlocked;

        Level lastPlayedLevel = null;
        for (int i = 0; i < levels.Count; i++)
        {
            createdLevels++;
            bool isUnlocked = createdLevels <= unlockedLevels;
            bool isLastUnlockedLevel = createdLevels == unlockedLevels;
            bool isLastPlayedLevel = GameManager.Instance.CurrentLevel == createdLevels;
            bool isNextLockedLevel = createdLevels == (unlockedLevels + 1);
            if (isLastPlayedLevel)
            {
                testObject.transform.position = levels[createdLevels].transform.position;
                lastPlayedLevel = levels[createdLevels];
            }
                

            Color colorToSet;
            if (isLastUnlockedLevel)
                colorToSet = lastUnlockedColor;
            else if (isUnlocked)
                colorToSet = unlockedColor;
            else
                colorToSet = lockedColor;

            levels[createdLevels].Init(createdLevels, isUnlocked, isLastUnlockedLevel, isNextLockedLevel, colorToSet, !isUnlocked ? lockedTextColor : unlockedTextColor);

            if (createdLevels == 1)
                continue;

            transitions[createdLevels - 1].Init(colorToSet);
        }

        scrollContent.anchoredPosition = new Vector2(0f, -testObject.anchoredPosition.y);
        gameObject.SetActive(true);
        StartCoroutine(lastPlayedLevel.Return());
    }

    public void SetBlocksRaycast(bool blockRaycasts)
    {
        foreach (Level level in levels.Values)
            level.GetComponent<GraphicRaycaster>().enabled = blockRaycasts;
    }

    public void UnlockLevel(int level)
    {
        AdManager.Instance.ShowRewardAd(true, () =>
        {
            AnalyticsManager.Instance.LogLevelSkippedLevelSelect(level);
            PlayerInfoManager.Instance.LevelsUnlocked = level;
            StartCoroutine(AnimateUnlockLevel(level));
        });
    }

    private IEnumerator AnimateUnlockLevel(int level)
    {
        LeanTween.color((RectTransform)transitions[level - 2].transform, unlockedColor, 0.25f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.25f);
        LeanTween.scale(levels[level - 1].gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.color((RectTransform) levels[level - 1].transform, unlockedColor, 0.25f)
            .setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.25f);
        LeanTween.color((RectTransform)transitions[level - 1].transform, lastUnlockedColor, 0.25f).setEase(LeanTweenType.easeInSine);
        levels[level].AnimateOutLockedItems(unlockedTextColor);
        yield return new WaitForSeconds(0.25f);
        LeanTween.scale(levels[level].gameObject, Vector3.one * 1.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.color((RectTransform)levels[level].transform, lastUnlockedColor, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        yield return new WaitForSeconds(0.25f);
        levels[level + 1].AnimateInLockPanel();
        yield return new WaitForSeconds(0.25f);
        levels[level - 1].Init(level - 1, true, false, false, unlockedColor, unlockedTextColor);
        transitions[level - 2].Init(unlockedColor);
        levels[level].Init(level, true, true, false, lastUnlockedColor, unlockedTextColor);
        transitions[level - 1].Init(lastUnlockedColor);
        levels[level + 1].Init(level + 1, false, false, true, lockedColor, lockedTextColor);
    }

    public IEnumerator AnimateIn()
    {
        canvasGroup.blocksRaycasts = true;
        float screenHeight = mainCanvas.sizeDelta.y;
        scrollRect.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, screenHeight);
        float testObjectPos = testObject.anchoredPosition.y;
        if (testObjectPos < 0)
            testObjectPos = 0;
        float movementAmount = screenHeight + testObjectPos;

        LeanTween.value(gameObject, 0f, movementAmount, 2f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate(
            (float value) =>
            {
                if (value <= screenHeight)
                {
                    scrollRect.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, screenHeight - value);
                }
                else
                {
                    scrollRect.anchoredPosition = Vector2.zero;
                    scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, screenHeight - value);
                }
            });

        returnIcon.localScale = Vector3.zero;
        LeanTween.scale(returnIcon, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return null;
    }

    private IEnumerator AnimateLevelSelectToStartScreen()
    {
        canvasGroup.blocksRaycasts = false;
        float screenHeight = mainCanvas.sizeDelta.y;
        float testObjectPos = -scrollContent.anchoredPosition.y;
        float movementAmount = screenHeight + testObjectPos;
        LeanTween.scale(returnIcon, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.value(gameObject, 0f, movementAmount, 2f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                float testValue = value;
                if (value <= testObjectPos)
                {
                    scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, -(testObjectPos - value));
                }
                else
                {
                    scrollContent.anchoredPosition = Vector2.zero;
                    scrollRect.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, value - testObjectPos);
                }
            });
        yield return new WaitForSeconds(3f);
        StartScreen.Instance.gameObject.SetActive(true);
        StartCoroutine(StartScreen.Instance.AnimateIn());

        //scrollRect.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, screenHeight);
    }
}
