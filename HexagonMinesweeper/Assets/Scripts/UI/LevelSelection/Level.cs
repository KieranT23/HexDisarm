using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
public class Level : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Start the level
    /// </summary>
    [SerializeField]
    private Button btn_startLevel;
    /// <summary>
    /// The level text
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI txt_level;
    /// <summary>
    /// The background colour of the level
    /// </summary>
    [SerializeField]
    private Color backgroundColor;
    /// <summary>
    /// The objects to enable/disable when an object is locked
    /// </summary>
    [SerializeField]
    private GameObject pn_unlockPanel;
    /// <summary>
    /// The lock that is shown when a level is locked
    /// </summary>
    [SerializeField]
    private GameObject pn_lock;
    /// <summary>
    /// The colour to set the object when it is locked
    /// </summary>
    [SerializeField]
    private Color lockColor;

    /// <summary>
    /// The background image that is attched to this object
    /// </summary>
    private Image background;
    /// <summary>
    /// The canvas that is attached to this object
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// Used to check if a level is currently being animated
    /// </summary>
    public bool IsAnimating;
    #endregion

    #region Methods
    #region Public
    /// <summary>
    /// Initialise this level
    /// </summary>
    /// <param name="level">The level to set this object to</param>
    /// <param name="isUnlocked">Is the level unlocked?</param>
    /// <param name="isLastUnlockedLevel">Is the level the last unlocked level?</param>
    /// <param name="isNextLockedLevel">Is the level the next unlocked level>?</param>
    /// <param name="colorToSet">The colour to set this level to</param>
    /// <param name="textColor">The colour to set the text on this level to</param>
    public void Init(int level, bool isUnlocked, bool isLastUnlockedLevel, bool isNextLockedLevel, Color colorToSet, Color textColor)
    {

        background = GetComponent<Image>();
        canvas = GetComponent<Canvas>();
        txt_level.text = level.ToString();

        btn_startLevel.onClick.RemoveAllListeners();
        btn_startLevel.onClick.AddListener(() =>
        {
            if (isUnlocked)
                StartCoroutine(AnimateLevelSelect(level));
            else if (isNextLockedLevel)
                LevelSelection.Instance.UnlockLevel(level);

        });

        background.color = colorToSet;

        transform.localScale = isLastUnlockedLevel ? Vector3.one * 1.25f : Vector3.one;
        pn_lock.gameObject.SetActive(!isUnlocked);

        LeanTween.alpha((RectTransform)pn_lock.transform, 0.4f, 0f).setEase(LeanTweenType.easeInSine);
        LeanTween.color((RectTransform)pn_lock.transform, lockColor, 0f);

        pn_unlockPanel.gameObject.SetActive(isNextLockedLevel);
        txt_level.color = textColor;

    }

    /// <summary>
    /// Animate out the locked items
    /// </summary>
    /// <param name="textColor">The colour to set the text</param>
    public void AnimateOutLockedItems(Color textColor)
    {
        LeanTween.alpha((RectTransform) pn_lock.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(pn_unlockPanel, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);

        LeanTween.value(txt_level.gameObject, txt_level.color, textColor, 0.25f).setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((Color color) => { txt_level.color = color; });
    }

    /// <summary>
    /// Animate in the lock panel
    /// </summary>
    public void AnimateInLockPanel()
    {
        pn_unlockPanel.SetActive(true);
        pn_unlockPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(pn_unlockPanel, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }
    #endregion

    #region Private
    /// <summary>
    /// Animate into a level
    /// </summary>
    /// <param name="level">The level to animate into</param>
    private IEnumerator AnimateLevelSelect(int level)
    {
        if (IsAnimating)
            yield break;

        CanvasGroup levelSelectionCanvasGroup = LevelSelection.Instance.GetComponent<CanvasGroup>();
        Color originalColor = background.color;
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1;

        yield return new WaitForSeconds(0.15f);

        //Animate the level into full screen
        RectTransform rect = (RectTransform) transform;
        LeanTween.value(gameObject, rect.sizeDelta.x, 5000, 0.35f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) => { rect.sizeDelta = new Vector2(value, value); });

        Color colorToAnimateTo = GridGenerator3D.Instance.SetBackgroundColours(false, true);

        yield return new WaitForSeconds(0.35f);
        
        //Start the level
        GameManager.Instance.StartLevel(level);

        LeanTween.color(rect, colorToAnimateTo, 0.15f).setEase(LeanTweenType.easeInOutSine);

        yield return new WaitForSeconds(0.5f);

        levelSelectionCanvasGroup.alpha = 1f;

        //Reset
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 1f, 0f).setEase(LeanTweenType.easeInSine);
        canvas.overrideSorting = false;
        rect.sizeDelta = new Vector2(256, 256);
        background.color = originalColor;
        LevelSelection.Instance.gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
