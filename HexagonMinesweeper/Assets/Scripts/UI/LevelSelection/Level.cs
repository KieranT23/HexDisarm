using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
public class Level : MonoBehaviour
{
    [SerializeField] private Button btn_startLevel;

    [SerializeField] private TextMeshProUGUI txt_level;

    private Image background;

    private Canvas canvas;

    [SerializeField] private Color backgroundColor;

    [SerializeField] private GameObject pn_unlockPanel;

    [SerializeField] private GameObject pn_lock;

    private void Start()
    {
        
    }

    public void Init(int level, bool isUnlocked, bool isLastUnlockedLevel, bool isNextLockedLevel, Color colorToSet, Color textColor)
    {
        background = GetComponent<Image>();
        canvas = GetComponent<Canvas>();
        txt_level.text = level.ToString();
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
        pn_unlockPanel.gameObject.SetActive(isNextLockedLevel);
        txt_level.color = textColor;

    }

    public void AnimateOutLockedItems(Color textColor)
    {
        LeanTween.alpha((RectTransform) pn_lock.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(pn_unlockPanel, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.value(txt_level.gameObject, txt_level.color, textColor, 0.25f).setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((Color color) => { txt_level.color = color; });
    }

    public void AnimateInLockPanel()
    {
        pn_unlockPanel.SetActive(true);
        pn_unlockPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(pn_unlockPanel, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }

    private IEnumerator AnimateLevelSelect(int level)
    {
        CanvasGroup levelSelectionCanvasGroup = LevelSelection.Instance.GetComponent<CanvasGroup>();
        levelSelectionCanvasGroup.blocksRaycasts = false;
        LevelSelection.Instance.SetBlocksRaycast(false);
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.SELECT);
        Color originalColor = background.color;
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1;
        yield return new WaitForSeconds(0.15f);
        RectTransform rect = (RectTransform) transform;
        LeanTween.value(gameObject, rect.sizeDelta.x, 5000, 0.35f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) => { rect.sizeDelta = new Vector2(value, value); });
        Color colorToAnimateTo = GridGenerator3D.Instance.SetBackgroundColours(false, true);
        yield return new WaitForSeconds(0.35f);
        
        GameManager.Instance.StartLevel(level);
        LeanTween.color(rect, colorToAnimateTo, 0.15f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.5f);
        levelSelectionCanvasGroup.alpha = 0f;
        //Reset
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 1f, 0f).setEase(LeanTweenType.easeInSine);
        canvas.overrideSorting = false;
        rect.sizeDelta = new Vector2(256, 256);
        background.color = originalColor;
        LevelSelection.Instance.gameObject.SetActive(false);
    }

    public IEnumerator Return()
    {
        Color originalColor = background.color;

        RectTransform rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(5000, 50000);
        background.color = backgroundColor;
        txt_level.GetComponent<CanvasGroup>().alpha = 0f;
        canvas.overrideSorting = true;

        CanvasGroup levelSelectionCanvasGroup = LevelSelection.Instance.GetComponent<CanvasGroup>();
        levelSelectionCanvasGroup.alpha = 1f;
        levelSelectionCanvasGroup.blocksRaycasts = true;
        LevelSelection.Instance.SetBlocksRaycast(true);
        yield return null;
        LeanTween.color(rect, originalColor, 0.15f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(gameObject, rect.sizeDelta.x, 256f, 0.35f).setEase(LeanTweenType.easeOutSine).setOnUpdate(
            (float value) => { rect.sizeDelta = new Vector2(value, value); });
        yield return new WaitForSeconds(0.35f);
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        canvas.overrideSorting = false;
    }
}
