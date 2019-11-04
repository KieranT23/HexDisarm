using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
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

    [SerializeField] private Button btn_music;

    [SerializeField] private RectTransform returnIcon;

    [SerializeField] private Button btn_return;

    [SerializeField] private RectTransform randomLevelBackground;

    [SerializeField] private CanvasGroup randomText;

    [SerializeField] private CanvasGroup randomDice;

    [SerializeField] private Button btn_playRandomLevel;

    [SerializeField] private Color colorToAnimateRandomBackgroundTo;

    [SerializeField] private Color colorToAnimateRandomBackgroundFrom;

    [SerializeField] private Button btn_restore;

    [SerializeField] private Button btn_dataCollection;

    [SerializeField] private Button btn_vibration;

    [SerializeField] private Color colorToAnimateCurrentLevelTo;

    [SerializeField] private Color colorToAnimateLevelSelectButtonTo;

    [SerializeField] private RectTransform selectLevelBackground;

    [SerializeField] private CanvasGroup selectLevelText;

    [SerializeField] private Button btn_selectLevel;

    private bool settingsOpen;

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
        Init();
        btn_continue.onClick.AddListener(Continue);
        btn_feedback.onClick.AddListener(Feedback);
        btn_settings.onClick.AddListener(() =>
        {
            AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Settings);
            StartCoroutine(AnimateSettingsOpen());
        });
        btn_return.onClick.AddListener(() => StartCoroutine(AnimateSettingsClose()));
        btn_noAds.onClick.AddListener(() =>
        {
            AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.RemoveAds);
            PopupManager.Instance.ShowRemoveAds(() => Purchaser.Instance.BuyNoAds());
        });
        btn_playRandomLevel.onClick.AddListener(() =>
        {
            AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Random);
            if (!PlayerInfoManager.Instance.HasShownRandomPopup)
            {
                PlayerInfoManager.Instance.HasShownRandomPopup = true;
                PopupManager.Instance.ShowRandomPopup(() => StartCoroutine(AnimateOutRandomLevel(true)));
            }
                
            else
                StartCoroutine(AnimateOutRandomLevel(true));
        });
        btn_restore.onClick.AddListener(() => Purchaser.Instance.RestorePurchases(true));
        btn_dataCollection.onClick.AddListener(() => PopupManager.Instance.ShowDataCollectionPopup());
        settingsButtons.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
        btn_selectLevel.onClick.AddListener(() => StartCoroutine(AnimateToLevelSelect()));
    }

    private void OnEnable()
    {
        if (PlayerInfoManager.Instance != null)
            Init();
    }

    public void Init()
    {
        txt_currentLevel.text = string.Format("Level\n<size=120>{0}</size>", PlayerInfoManager.Instance.LevelsUnlocked);
    }

    private void Continue()
    {
        AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Continue);
        StartCoroutine(AnimateOut());
        //gameObject.SetActive(false);
        //StartCoroutine(AnimateOutRandomLevel(false));
    }

    private void Feedback()
    {
        AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Feedback);
        PopupManager.Instance.ShowFeedbackPopup();
    }

    private IEnumerator AnimateOut()
    {
        /*
        canvasGroup.blocksRaycasts = false;
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.SELECT);
        LeanTween.value(currentLevelBackground.gameObject, 386, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomText, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(randomLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();
        StartCoroutine(LevelSelection.Instance.AnimateIn());
        gameObject.SetActive(false);*/
        canvasGroup.blocksRaycasts = false;
        Color colorToAnimateTo = GridGenerator3D.Instance.SetBackgroundColours(false, true);
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.SELECT);

        LeanTween.value(currentLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.scale(randomLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        //LeanTween.alphaCanvas(randomDice, 0, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);
        LeanTween.color(currentLevelBackground, colorToAnimateTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);
        GameManager.Instance.StartLevel(PlayerInfoManager.Instance.LevelsUnlocked);
        //LevelSelection.Instance.gameObject.SetActive(false);
        //StartCoroutine(LevelSelection.Instance.AnimateIn());
        gameObject.SetActive(false);
    }

    public IEnumerator AnimateIn()
    {
        gameObject.SetActive(true);
        btn_music.GetComponent<MusicButton>().Init();
        btn_audio.GetComponent<AudioButton>().Init();
        btn_vibration.GetComponent<VibrationButton>().Init();
        LeanTween.color(currentLevelBackground, colorToAnimateCurrentLevelTo, 0.35f);
        LeanTween.value(currentLevelBackground.gameObject, 4000, 386, 0.35f).setEase(LeanTweenType.easeOutSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(randomText, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(randomLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);

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
        canvasGroup.blocksRaycasts = true;


    }

    private IEnumerator AnimateSettingsOpen()
    {
        settingsOpen = true;
        //RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        RectTransform restoreRect = (RectTransform) btn_restore.transform;

        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -350f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_restore.gameObject, -350f, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        //LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        settingsButtons.gameObject.SetActive(true);
        menuButtons.SetActive(false);
        returnIcon.localScale = Vector3.zero;
        LeanTween.scale(returnIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        RectTransform dataRect = (RectTransform) btn_dataCollection.transform;
        RectTransform audioRect = (RectTransform)btn_audio.transform;
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, -50f);
        musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, -50f);
        dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, -50f);
        vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, -50f);
        LeanTween.value(btn_dataCollection.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);

        LeanTween.value(btn_audio.gameObject, -200, -350, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_music.gameObject, -350, -500, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_vibration.gameObject, -500f, -650f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
    }

    private IEnumerator AnimateSettingsClose()
    {
        settingsOpen = false;
        RectTransform dataRect = (RectTransform) btn_dataCollection.transform;
        RectTransform audioRect = (RectTransform)btn_audio.transform;
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        LeanTween.value(btn_vibration.gameObject, -650, -500, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_music.gameObject, -500, -350, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_audio.gameObject, -350, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_dataCollection.gameObject, -200, -50, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        settingsButtons.SetActive(false);
        menuButtons.SetActive(true);
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform) btn_restore.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.value(btn_feedback.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_restore.gameObject, -200f, -350f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_noAds.gameObject, -350f, -500f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        
    }

    private IEnumerator AnimateMenuContinue()
    {
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform) btn_restore.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.value(btn_noAds.gameObject, -500, -350, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_restore.gameObject, -350, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
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
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform dataRect = (RectTransform) btn_dataCollection.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        LeanTween.value(btn_vibration.gameObject, -650, -500, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_music.gameObject, -500, -350, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_audio.gameObject, -350f, -200f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_dataCollection.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                audioRect.anchoredPosition = new Vector2(audioRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator AnimateOutRandomLevel(bool randomLevel)
    {
        canvasGroup.blocksRaycasts = false;
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.SELECT);

        LeanTween.value(randomLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { randomLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.scale(currentLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomText.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomDice, 0, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);
        LeanTween.color(randomLevelBackground, colorToAnimateRandomBackgroundTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        if (randomLevel)
            GameManager.Instance.StartRandomLevel(false);
        else
        {
            GameManager.Instance.StartLevel(PlayerInfoManager.Instance.LevelsUnlocked);
        }
        LevelSelection.Instance.gameObject.SetActive(false);
        //StartCoroutine(LevelSelection.Instance.AnimateIn());
        gameObject.SetActive(false);
    }

    public IEnumerator AnimateInRandomLevel()
    {
        gameObject.SetActive(true);
        LeanTween.color(randomLevelBackground, colorToAnimateRandomBackgroundFrom, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        yield return new WaitForSeconds(0.25f);
        LeanTween.value(randomLevelBackground.gameObject, 4000, 312, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { randomLevelBackground.sizeDelta = new Vector2(value, value); });
        yield return new WaitForSeconds(0.2f);
        LeanTween.scale(currentLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alphaCanvas(randomText.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomDice, 1, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return AnimateMenuRandom();

        yield return new WaitForSeconds(0.15f);
        
        LevelSelection.Instance.gameObject.SetActive(true);
        
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator AnimateMenuRandom()
    {
        LeanTween.color((RectTransform)btn_settings.transform, colorToAnimateSettingsFrom, 0.15f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        LeanTween.scale(settingsIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.15f);
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform) btn_restore.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.value(btn_feedback.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(restoreRect.gameObject, -200, -350, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                restoreRect.anchoredPosition = new Vector2(restoreRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);

        LeanTween.value(btn_noAds.gameObject, -350, -500, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator AnimateToLevelSelect()
    {
        canvasGroup.blocksRaycasts = false;
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.SELECT);

        LeanTween.value(selectLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { selectLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.scale(randomLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(currentLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(selectLevelText, 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        //LeanTween.alphaCanvas(randomDice, 0, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);
        LeanTween.color(selectLevelBackground, colorToAnimateLevelSelectButtonTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);
        LevelSelection.Instance.gameObject.SetActive(true);
        LevelSelection.Instance.SetupLevelSelect();
        //GameManager.Instance.StartLevel(PlayerInfoManager.Instance.LevelsUnlocked);
        //LevelSelection.Instance.gameObject.SetActive(false);
        //StartCoroutine(LevelSelection.Instance.AnimateIn());
        gameObject.SetActive(false);
    }
}
