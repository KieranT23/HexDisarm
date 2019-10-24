using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public static StartScreen Instance;

    [SerializeField] private TextMeshProUGUI txt_currentLevel;

    [SerializeField] private Button btn_continue;

    [SerializeField] private Button btn_settings;

    [SerializeField] private Button btn_feedback;

    [SerializeField] private Button btn_noAds;

    [SerializeField] private RectTransform currentLevelBackground;

    [SerializeField] private CanvasGroup txt_continue;

    [SerializeField] private RectTransform settingsIcon;

    [SerializeField] private Color colorToAnimateSettingsTo;

    [SerializeField] private Color colorToAnimateSettingsFrom;

    [SerializeField] private GameObject menuButtons;

    [SerializeField] private GameObject settingsButtons;

    [SerializeField] private Button btn_audio;

    [SerializeField] private Button btn_vibration;

    [SerializeField] private RectTransform returnIcon;

    [SerializeField] private Button btn_return;

    private bool settingsOpen;

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
        Init();
        btn_continue.onClick.AddListener(Continue);
        btn_feedback.onClick.AddListener(Feedback);
        btn_settings.onClick.AddListener(() => StartCoroutine(AnimateSettingsOpen()));
        btn_return.onClick.AddListener(() => StartCoroutine(AnimateSettingsClose()));
        btn_noAds.onClick.AddListener(() => Purchaser.Instance.BuyNoAds());
        settingsButtons.SetActive(false);
    }

    public void Init()
    {
        txt_currentLevel.text = string.Format("Level\n<size=200>{0}</size>", PlayerInfoManager.Instance.LevelsUnlocked);
    }

    private void Continue()
    {
        StartCoroutine(AnimateOut());
        //gameObject.SetActive(false);
    }

    private void Feedback()
    {
        PopupManager.Instance.ShowFeedbackPopup();
    }

    private IEnumerator AnimateOut()
    {
        
        LeanTween.value(currentLevelBackground.gameObject, 512, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();
        StartCoroutine(LevelSelection.Instance.AnimateIn());
        gameObject.SetActive(false);
    }

    public IEnumerator AnimateIn()
    {
        btn_vibration.GetComponent<VibrationButton>().Init();
        btn_audio.GetComponent<AudioButton>().Init();
        LeanTween.value(currentLevelBackground.gameObject, 4000, 512, 0.35f).setEase(LeanTweenType.easeOutSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;

        LeanTween.color((RectTransform)btn_settings.transform, colorToAnimateSettingsFrom, 0.15f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        LeanTween.value(btn_feedback.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.2f);
        LeanTween.scale(settingsIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.value(btn_noAds.gameObject, -200f, -350f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        
        
        
    }

    private IEnumerator AnimateSettingsOpen()
    {
        settingsOpen = true;
        //RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;

        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        //LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        settingsButtons.gameObject.SetActive(true);
        menuButtons.SetActive(false);
        returnIcon.localScale = Vector3.zero;
        LeanTween.scale(returnIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        RectTransform audioRect = (RectTransform)btn_audio.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;
        audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, -50f);
        vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, -50f);
        LeanTween.value(btn_audio.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_vibration.gameObject, -200f, -350f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
    }

    private IEnumerator AnimateSettingsClose()
    {
        settingsOpen = false;
        RectTransform audioRect = (RectTransform)btn_audio.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;
        LeanTween.value(btn_vibration.gameObject, -350f, -200f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_audio.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        settingsButtons.SetActive(false);
        menuButtons.SetActive(true);
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.value(btn_feedback.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_noAds.gameObject, -200f, -350f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        
    }

    private IEnumerator AnimateMenuContinue()
    {
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.value(btn_noAds.gameObject, -350f, -200f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        LeanTween.scale(settingsIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.color((RectTransform)btn_settings.transform, colorToAnimateSettingsTo, 0.15f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator AnimateSettingsContinue()
    {
        settingsOpen = false;
        RectTransform audioRect = (RectTransform)btn_audio.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;
        LeanTween.value(btn_vibration.gameObject, -350f, -200f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_audio.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.2f);
    }
}
