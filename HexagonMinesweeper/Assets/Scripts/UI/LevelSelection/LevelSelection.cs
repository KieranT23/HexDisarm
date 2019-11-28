using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LevelSelection : MonoBehaviour
{
    #region Variables
    #region Static
    public static LevelSelection Instance;
    #endregion

    #region Editor
    /// <summary>
    /// The level selection content
    /// </summary>
    [SerializeField]
    private RectTransform scrollContent;
    /// <summary>
    /// The colour to use when a level is unlocked
    /// </summary>
    [SerializeField]
    private Color unlockedColor;
    /// <summary>
    /// The colour to use when a level is locked
    /// </summary>
    [SerializeField]
    private Color lockedColor;
    /// <summary>
    /// The colour to use when a level is the last unlocked
    /// </summary>
    [SerializeField]
    private Color lastUnlockedColor;
    /// <summary>
    /// The colour to use for th text when a level is locked
    /// </summary>
    [SerializeField]
    private Color lockedTextColor;
    /// <summary>
    /// The colour to use for the text when a level is unlocked
    /// </summary>
    [SerializeField]
    private Color unlockedTextColor;
    /// <summary>
    /// The scroll rect content
    /// </summary>
    [SerializeField]
    private RectTransform scrollRect;
    /// <summary>
    /// The return icon to close the level selection
    /// </summary>
    [SerializeField]
    private RectTransform returnIcon;
    /// <summary>
    /// The button to return to the start screen
    /// </summary>
    [SerializeField]
    private Button btn_return;
    /// <summary>
    /// The main canvas in the scene
    /// </summary>
    [SerializeField]
    private RectTransform mainCanvas;
    /// <summary>
    /// All the level objects to use
    /// </summary>
    [SerializeField]
    private Level[] levelsToUse;
    /// <summary>
    /// All the transitions to use
    /// </summary>
    [SerializeField]
    private Transition[] transitionsToUse;
    /// <summary>
    /// The scroll bar
    /// </summary>
    [SerializeField]
    private Scrollbar scrollbar;
    #endregion
    #region Private
    /// <summary>
    /// The canvas group that is attached to this object
    /// </summary>
    private CanvasGroup canvasGroup;
    /// <summary>
    /// The level group that the level selection is currently on
    /// </summary>
    private int currentLevelGroup = 1;
    /// <summary>
    /// The position of all the transitions
    /// </summary>
    private Dictionary<Transition, Vector2> transitionPositions = new Dictionary<Transition, Vector2>();
    /// <summary>
    /// The positions of all the transitions when they are reversed
    /// </summary>
    private Dictionary<Transition, Vector2> transitionReversedPostions = new Dictionary<Transition, Vector2>();
    /// <summary>
    /// Flag for whether the level selection can be moved down
    /// </summary>
    private bool canMoveDown;
    /// <summary>
    /// Flag for whether the level selection can be moved up
    /// </summary>
    private bool canMoveUp;
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
        gameObject.SetActive(false);
        SetPositions();
    }

    private void Start()
    {
        btn_return.onClick.AddListener(() => StartCoroutine(AnimateLevelSelectToStartScreen()));

        canvasGroup = GetComponent<CanvasGroup>();
        SetupLevelSelect();
    }

    private void OnEnable()
    {
        //Set listeners
        SwipeManager.Instance.gameObject.SetActive(true);
        SwipeManager.Instance.OnSwipeDown.RemoveAllListeners();
        SwipeManager.Instance.OnSwipeUp.RemoveAllListeners();
        SwipeManager.Instance.OnSwipeDown.AddListener(NavigateUp);
        SwipeManager.Instance.OnSwipeUp.AddListener(NavigateDown);
    }

    private void OnDisable()
    {
        if (SwipeManager.Instance == null)
            return;

        //Remove listeners to avoid null references
        SwipeManager.Instance.OnSwipeDown.RemoveListener(NavigateUp);
        SwipeManager.Instance.OnSwipeUp.RemoveListener(NavigateDown);
        SwipeManager.Instance.gameObject.SetActive(false);
    }
    #endregion

    #region Private
    /// <summary>
    /// Navigate to the next area of the level selection
    /// </summary>
    private void NavigateUp()
    {
        if (!canMoveUp)
            return;

        currentLevelGroup++;
        if (currentLevelGroup > 25)
            currentLevelGroup = 25;

        StartCoroutine(AnimateToNextArea(true));
    }

    /// <summary>
    /// Navigate to the previous area of the level selection
    /// </summary>
    private void NavigateDown()
    {
        if (!canMoveDown)
            return;

        currentLevelGroup--;
        if (currentLevelGroup < 0)
            currentLevelGroup = 0;

        StartCoroutine(AnimateToNextArea(false));
    }

    /// <summary>
    /// Set the positions of all the transitions
    /// </summary>
    private void SetPositions()
    {
        foreach (Transition transition in transitionsToUse)
        {
            RectTransform rect = (RectTransform) transition.transform;
            transitionPositions.Add(transition, rect.anchoredPosition);

            Vector2 position = rect.anchoredPosition;
            if (rect.eulerAngles.z < 200)
            {
                if (rect.sizeDelta.y > 1000)
                {
                    position.x -= 1732f;
                    position.y += 1000;
                }
                else
                {
                    position.x -= 173.2f;
                    position.y += 100;
                }
            }
            else
            {
                if (rect.sizeDelta.y > 1000)
                {
                    position.x += 1732f;
                    position.y += 1000;
                }
                else
                {
                    position.x += 173.2f;
                    position.y += 100;
                }
            }
            transitionReversedPostions.Add(transition, position);
        }
    }

    /// <summary>
    /// Animate in the level selection after it has been initialised
    /// </summary>
    /// <param name="reversed">Is animating in reverse?</param>
    private IEnumerator AnimateInAfterInit(bool reversed = false)
    {
        float transitionTime = 0.05f;
        float levelTime = 0.075f;

        //Setup all the transitions to get the correct effect
        foreach (Transition transition in transitionsToUse)
        {
            RectTransform rect = (RectTransform)transition.transform;
            Vector2 position = rect.anchoredPosition;
            if ((reversed && transitionReversedPostions.ContainsKey(transition)) || (!reversed && transitionPositions.ContainsKey(transition)))
                position = reversed ? transitionReversedPostions[transition] : transitionPositions[transition];
            rect.pivot = reversed ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(5f, 0f);

        }

        foreach (Level level in levelsToUse)
            level.transform.localScale = Vector3.zero;

        yield return null;

        int transitionsUsed = reversed ? 8 : 0;
        int levelsUsed = reversed ? 7 : 0;

        //Animate in the level selection
        for (int i = 0; i < 17; i++)
        {
            //Do not animate the start and end in certain circumstances
            if (i == 0 && !reversed && currentLevelGroup == 0)
                continue;
            else if (i == 0 && reversed && currentLevelGroup >= 24)
                continue;
            else if (i == 16 && reversed && currentLevelGroup == 0)
                continue;
            else if (i == 16 && !reversed && currentLevelGroup >= 24)
                continue;

            int iValue = i;
            if (reversed)
                iValue = 16 - i;

            if (iValue % 2 == 0)
            {
                //Animate in the transition to create the effect of drawing lines between each level
                float valueToAnimateTo = 200;

                if (i == 0 || i == 16)
                    valueToAnimateTo = 2000;

                RectTransform transitionRect = (RectTransform)transitionsToUse[transitionsUsed].transform;
                LeanTween.value(transitionRect.gameObject, 0f, valueToAnimateTo, transitionTime).setEase(LeanTweenType.easeOutSine)
                    .setOnUpdate(
                        (float value) => { transitionRect.sizeDelta = new Vector2(5f, value); });

                if (reversed)
                    transitionsUsed--;
                else
                    transitionsUsed++;

                yield return new WaitForSeconds(transitionTime);

            }
            else
            {
                //Animate in the levels
                LeanTween.scale(levelsToUse[levelsUsed].gameObject, Vector3.one, levelTime).setEase(LeanTweenType.easeOutBack);

                if (reversed)
                    levelsUsed--;
                else
                    levelsUsed++;

                yield return new WaitForSeconds(levelTime);
            }
        }

        canMoveUp = currentLevelGroup < 24;
        canMoveDown = currentLevelGroup > 0;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Animate to the next area of the level selection
    /// </summary>
    /// <param name="nextGroup">Is going to the next group?</param>
    private IEnumerator AnimateToNextArea(bool nextGroup)
    {
        canMoveUp = false;
        canMoveDown = false;
        Vector2 pos = scrollContent.anchoredPosition;

        //Animate out of view
        LeanTween.value(scrollContent.gameObject, scrollContent.anchoredPosition.y,
                nextGroup ? -mainCanvas.sizeDelta.y : mainCanvas.sizeDelta.y, 0.15f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) =>
                {
                    scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, value);
                });

        yield return new WaitForSeconds(0.25f);

        //Animate the level selection into view
        SetupLevelSelect(0, true, false);
        scrollContent.anchoredPosition = pos;
        StartCoroutine(AnimateInAfterInit(!nextGroup));
    }

    /// <summary>
    /// Do the animation when a level is unlocked
    /// </summary>
    /// <param name="level">The level to unlock</param>
    private IEnumerator AnimateUnlockLevel(int level)
    {
        //The level to unlock
        int newLevel = (level - (currentLevelGroup * 8)) - 1;
        levelsToUse[newLevel].IsAnimating = true;

        //Animate the transition before the previous level
        if (newLevel >= 1)
        {
            LeanTween.color((RectTransform)transitionsToUse[newLevel - 1].transform, unlockedColor, 0.25f).setEase(LeanTweenType.easeInSine);

            yield return new WaitForSeconds(0.25f);
        }

        //Animate the previous level
        if (newLevel >= 1)
        {
            LeanTween.scale(levelsToUse[newLevel - 1].gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.color((RectTransform)levelsToUse[newLevel - 1].transform, unlockedColor, 0.25f)
                .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

            yield return new WaitForSeconds(0.25f);
        }

        //Animate the current level
        LeanTween.color((RectTransform)transitionsToUse[newLevel].transform, lastUnlockedColor, 0.25f).setEase(LeanTweenType.easeInSine).setRecursive(false);
        levelsToUse[newLevel].AnimateOutLockedItems(unlockedTextColor);

        yield return new WaitForSeconds(0.25f);
        
        //Animate the current level
        LeanTween.scale(levelsToUse[newLevel].gameObject, Vector3.one * 1.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.color((RectTransform)levelsToUse[newLevel].transform, lastUnlockedColor, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);

        //Animate the next level
        if (newLevel < 7)
        {
            levelsToUse[newLevel + 1].AnimateInLockPanel();

            yield return new WaitForSeconds(0.25f);

            levelsToUse[newLevel + 1].Init(level + 1, false, false, true, lockedColor, lockedTextColor);
        }
        else if (newLevel >= 1)
        {
            levelsToUse[newLevel - 1].Init(level - 1, true, false, false, unlockedColor, unlockedTextColor);
        }
        levelsToUse[newLevel].Init(level, true, true, false, lastUnlockedColor, unlockedTextColor);


        if (newLevel >= 2)
            transitionsToUse[newLevel - 1].Init(unlockedColor);

        transitionsToUse[newLevel].Init(lastUnlockedColor);

        yield return new WaitForSeconds(0.25f);

        levelsToUse[newLevel].IsAnimating = false;

    }

    /// <summary>
    /// Animate level selection to start screen
    /// </summary>
    private IEnumerator AnimateLevelSelectToStartScreen()
    {
        canvasGroup.blocksRaycasts = false;
        float screenHeight = mainCanvas.sizeDelta.y;
        float testObjectPos = -scrollContent.anchoredPosition.y;
        float movementAmount = -screenHeight;

        LeanTween.scale(returnIcon, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.value(gameObject, 0f, movementAmount, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                scrollContent.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);

        scrollContent.anchoredPosition = Vector2.zero;
        returnIcon.localScale = Vector3.one;
        StartScreen.Instance.gameObject.SetActive(true);
        StartCoroutine(StartScreen.Instance.AnimateFromLevelSelect());
    }
    #endregion

    #region Public
    /// <summary>
    /// Setup the level selection for the provided level group
    /// </summary>
    /// <param name="level">The level to show</param>
    /// <param name="useLevelGroup">Should the current level group be used?</param>
    /// <param name="animate">Animate the level selection into view</param>
    public void SetupLevelSelect(int level = 0, bool useLevelGroup = false, bool animate = true)
    {
        canMoveUp = false;
        canMoveDown = false;
        returnIcon.localScale = Vector3.one;

        int amountOfLevelsUnlocked = PlayerInfoManager.Instance.LevelsUnlocked;

        //Get the correct level group
        if (level == 0)
            level = amountOfLevelsUnlocked;

        float valueToRound = (level - 1) / 8;
        int levelGroup = (int)Math.Floor(valueToRound);
        if (useLevelGroup)
            levelGroup = currentLevelGroup;
        int startingValue = levelGroup * 8;
        currentLevelGroup = levelGroup;

        //Set the scrollbar to make it look like it is a scroll view
        scrollbar.value = (0.04f * currentLevelGroup);

        Color colorToSet;

        //Setup all levels and transitions
        for (int i = 0; i < 9; i++)
        {
            int value = startingValue + i + 1;
            bool isUnlocked = value <= amountOfLevelsUnlocked;
            bool isLastUnlockedLevel = value == amountOfLevelsUnlocked;
            bool isNextUnlockedLevel = value == amountOfLevelsUnlocked + 1;

            if (isLastUnlockedLevel)
                colorToSet = lastUnlockedColor;
            else if (isUnlocked)
                colorToSet = unlockedColor;
            else
                colorToSet = lockedColor;

            if (i != 8)
                levelsToUse[i].Init(value, isUnlocked, isLastUnlockedLevel, isNextUnlockedLevel, colorToSet, !isUnlocked ? lockedTextColor : unlockedTextColor);
            transitionsToUse[i].Init(colorToSet);
        }

        if (animate)
            StartCoroutine(AnimateInAfterInit());
    }

    /// <summary>
    /// Unlock a level
    /// </summary>
    /// <param name="level">The level to unlock</param>
    public void UnlockLevel(int level)
    {
        AdManager.Instance.ShowRewardAd(true, () =>
        {
            AnalyticsManager.Instance.LogLevelSkippedLevelSelect(level);
            PlayerInfoManager.Instance.LevelsUnlocked = level;
            StartCoroutine(AnimateUnlockLevel(level));
        });
    }
    #endregion
    #endregion





    
}
