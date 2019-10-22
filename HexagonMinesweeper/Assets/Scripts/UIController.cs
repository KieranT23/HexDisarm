﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI bombsRemainingText;

    [SerializeField] private Button btn_menu;

    [SerializeField] private RectTransform img_menuIcon;

    [SerializeField] private RectTransform img_currentLevelBackground;

    [SerializeField] private RectTransform img_bombsRemainingBackground;

    private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI completeText;

    [SerializeField] private Button btn_dim;

    [SerializeField] private Button btn_quit;

    [SerializeField] private RectTransform quitIcon;

    [SerializeField] private Button btn_settings;

    [SerializeField] private Button btn_noAds;

    [SerializeField] private GameObject menuButtons;

    private string[] completeTexts =
    {
        "Well done!",
        "Congratulations!",
        "Amazing!",
        "Wow!",
        "OMG!",
        "Incredible!",
        "Unbelieable!",
        "Awesome!",
        "Superb!",
        "Smashing!",
        "Extraordinary!",
        "Astonishing!"
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        canvasGroup = GetComponent<CanvasGroup>();
        img_currentLevelBackground.anchoredPosition = new Vector2(275f, 152f);
        levelText.GetComponent<CanvasGroup>().alpha = 0f;
        LeanTween.alpha((RectTransform) btn_menu.transform, 0f, 0f);
        LeanTween.alpha(img_menuIcon, 0f, 0f);
        img_bombsRemainingBackground.localScale = Vector3.zero;
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 0f, 0f);
        completeText.transform.localScale = Vector3.zero;
        menuButtons.SetActive(false);
        Show(false);
        btn_dim.gameObject.SetActive(false);
        btn_menu.onClick.AddListener(() => StartCoroutine(AnimateMenuOpen()));
        btn_dim.onClick.AddListener(() => StartCoroutine(AnimateMenuClose()));
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateBombsRemaining(int bombsRemaining)
    {
        bombsRemainingText.text = bombsRemaining.ToString();
    }

    public void MenuClicked()
    {
        /*GridGenerator.Instance.DestroyGrid();
        LevelSelection.Instance.*/
    }

    public void Show(bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.blocksRaycasts = show;
        //StartCoroutine(AnimateInUI());
    }

    public IEnumerator AnimateInUI()
    {
        LeanTween.value(gameObject, img_currentLevelBackground.anchoredPosition, new Vector2(75f, -52f), 0.25f)
            .setEase(LeanTweenType.easeOutSine).setOnUpdate(
                (Vector2 value) => { img_currentLevelBackground.anchoredPosition = value; });
        LeanTween.alpha((RectTransform) btn_menu.transform, 1f, 0.25f).setEase(LeanTweenType.easeOutSine)
            .setRecursive(false);
        yield return new WaitForSeconds(0.5f);
        LeanTween.alphaCanvas(levelText.GetComponent<CanvasGroup>(), 1f, 0.25f).setEase(LeanTweenType.easeOutSine);
        LeanTween.alpha(img_menuIcon, 1f, 0.25f).setEase(LeanTweenType.easeOutSine);
        yield return null;
    }

    public IEnumerator AnimateInBombsRemaining()
    {
        LeanTween.scale(img_bombsRemainingBackground, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.5f);
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 1f, 0.25f)
            .setEase(LeanTweenType.easeInSine);
    }

    public IEnumerator ShowCompleteLevelText()
    {
        completeText.text = completeTexts[Random.Range(0, completeTexts.Length)];
        LeanTween.scale(completeText.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(3f);
        LeanTween.alphaCanvas(completeText.GetComponent<CanvasGroup>(), 0f, 0.35f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.35f);
        LeanTween.alphaCanvas(completeText.GetComponent<CanvasGroup>(), 1f, 0f).setEase(LeanTweenType.easeInSine);
        completeText.transform.localScale = Vector3.zero;
    }

    private IEnumerator AnimateMenuOpen()
    {
        yield return null;
        btn_dim.gameObject.SetActive(true);
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0f);
        RectTransform settingsRect = (RectTransform) btn_settings.transform;
        RectTransform noAdsRect = (RectTransform) btn_noAds.transform;
        settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, -50f);
        noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, -50f);
        menuButtons.gameObject.SetActive(true);
        quitIcon.localScale = Vector3.zero;
        yield return null;
        LeanTween.scale(quitIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.15f);
        LeanTween.alpha((RectTransform) btn_dim.transform, 0.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.value(btn_settings.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -350, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
    }

    private IEnumerator AnimateMenuClose()
    {
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_settings.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        btn_dim.gameObject.SetActive(false);
        menuButtons.gameObject.SetActive(false);
    }
}
