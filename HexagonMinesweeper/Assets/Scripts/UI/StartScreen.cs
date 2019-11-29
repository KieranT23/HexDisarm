using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class StartScreen : MonoBehaviour
{
    #region Variables
    #region Static
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static StartScreen Instance;
    #endregion
    /// <summary>
    /// The current level text
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI txt_currentLevel;
    /// <summary>
    /// The button to continue
    /// </summary>
    [SerializeField]
    private Button btn_continue;
    /// <summary>
    /// The button to open the settings
    /// </summary>
    [SerializeField]
    private Button btn_settings;
    /// <summary>
    /// The button to open the feedback popup
    /// </summary>
    [SerializeField]
    private Button btn_feedback;
    /// <summary>
    /// The button to open the no ads popup
    /// </summary>
    [SerializeField]
    private Button btn_noAds;
    /// <summary>
    /// The background of the current level object
    /// </summary>
    [SerializeField]
    private RectTransform currentLevelBackground;
    /// <summary>
    /// The canvas group component of the continue text
    /// </summary>
    [SerializeField]
    private CanvasGroup txt_continue;
    /// <summary>
    /// The settings icon
    /// </summary>
    [SerializeField]
    private RectTransform settingsIcon;
    /// <summary>
    /// The colour to animate the settings icon to
    /// </summary>
    [SerializeField]
    private Color colorToAnimateSettingsTo;
    /// <summary>
    /// The colour to animate the settings icon from
    /// </summary>
    [SerializeField]
    private Color colorToAnimateSettingsFrom;
    /// <summary>
    /// The parent of the menu buttons
    /// </summary>
    [SerializeField]
    private GameObject menuButtons;
    /// <summary>
    /// The parent of the settings buttons
    /// </summary>
    [SerializeField]
    private GameObject settingsButtons;
    /// <summary>
    /// The music button
    /// </summary>
    [SerializeField]
    private Button btn_music;
    /// <summary>
    /// The return icon that is used to close the settings
    /// </summary>
    [SerializeField]
    private RectTransform returnIcon;
    /// <summary>
    /// The return button
    /// </summary>
    [SerializeField]
    private Button btn_return;
    /// <summary>
    /// The background of the random level background
    /// </summary>
    [SerializeField]
    private RectTransform randomLevelBackground;
    /// <summary>
    /// The random level text
    /// </summary>
    [SerializeField]
    private CanvasGroup randomText;
    /// <summary>
    /// The random level icon
    /// </summary>
    [SerializeField]
    private CanvasGroup randomDice;
    /// <summary>
    /// Play a random level
    /// </summary>
    [SerializeField]
    private Button btn_playRandomLevel;
    /// <summary>
    /// The colour to animate the random background to
    /// </summary>
    [SerializeField]
    private Color colorToAnimateRandomBackgroundTo;
    /// <summary>
    /// The colour to animate the random background from
    /// </summary>
    [SerializeField]
    private Color colorToAnimateRandomBackgroundFrom;
    /// <summary>
    /// The button to restore purchases
    /// </summary>
    [SerializeField]
    private Button btn_restore;
    /// <summary>
    /// The button to open the data collection popup
    /// </summary>
    [SerializeField]
    private Button btn_dataCollection;
    /// <summary>
    /// The button to open the credits popup
    /// </summary>
    [SerializeField]
    private Button btn_credits;
    /// <summary>
    /// The vibration button
    /// </summary>
    [SerializeField]
    private Button btn_vibration;
    /// <summary>
    /// The colour to animate the current level background to
    /// </summary>
    [SerializeField]
    private Color colorToAnimateCurrentLevelTo;
    /// <summary>
    /// The colour to animate the current level background from
    /// </summary>
    [SerializeField]
    private Color colorToAnimateLevelSelectButtonTo;
    /// <summary>
    /// The select level background
    /// </summary>
    [SerializeField]
    private RectTransform selectLevelBackground;
    /// <summary>
    /// The select level text
    /// </summary>
    [SerializeField]
    private CanvasGroup selectLevelText;
    /// <summary>
    /// The select level button
    /// </summary>
    [SerializeField]
    private Button btn_selectLevel;

    #region Private
    /// <summary>
    /// Is the settings currently open?
    /// </summary>
    private bool settingsOpen;
    /// <summary>
    /// The canvas group that is attached to this object
    /// </summary>
    private CanvasGroup canvasGroup;
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
    }

    private void Start()
    {
        //Init screen and setup listeners
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
        btn_credits.onClick.AddListener(() => PopupManager.Instance.ShowCreditsPopup());
        settingsButtons.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
        btn_selectLevel.onClick.AddListener(() => StartCoroutine(AnimateToLevelSelect()));
    }

    private void OnEnable()
    {
        if (PlayerInfoManager.Instance != null)
            Init();
    }
    #endregion
    #region Public
    /// <summary>
    /// Initalise the current level text to make sure it is the correct level
    /// </summary>
    public void Init()
    {
        txt_currentLevel.text = string.Format("Level\n<size=120>{0}</size>", PlayerInfoManager.Instance.LevelsUnlocked);
    }

    /// <summary>
    /// Animate the settings into view
    /// </summary>
    public IEnumerator AnimateIn()
    {
        //Initalise elements
        gameObject.SetActive(true);
        btn_music.GetComponent<MusicButton>().Init();
        btn_vibration.GetComponent<VibrationButton>().Init();
        menuButtons.SetActive(true);
        settingsButtons.SetActive(false);
        selectLevelBackground.sizeDelta = new Vector2(312f, 312f);
        selectLevelText.alpha = 1f;
        currentLevelBackground.sizeDelta = new Vector2(4000, 4000);
        currentLevelBackground.localScale = Vector3.one;
        selectLevelBackground.localScale = Vector3.zero;
        LeanTween.color(selectLevelBackground, colorToAnimateRandomBackgroundFrom, 0f);

        //Animate in each element in the screen
        LeanTween.color(currentLevelBackground, colorToAnimateCurrentLevelTo, 0.35f);
        LeanTween.value(currentLevelBackground.gameObject, 4000, 386, 0.35f).setEase(LeanTweenType.easeOutSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(randomText, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(randomLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(selectLevelBackground, Vector3.one, 0.15f);

        //Animate in the menu icons
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

    /// <summary>
    /// Animate the settings screen into view from a random level
    /// </summary>
    public IEnumerator AnimateInRandomLevel()
    {
        gameObject.SetActive(true);

        //Animate the background
        LeanTween.color(randomLevelBackground, colorToAnimateRandomBackgroundFrom, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);

        LeanTween.value(randomLevelBackground.gameObject, 4000, 312, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { randomLevelBackground.sizeDelta = new Vector2(value, value); });

        yield return new WaitForSeconds(0.2f);

        //Animate in each element
        LeanTween.scale(currentLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(selectLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alphaCanvas(randomText.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomDice, 1, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeInSine);

        yield return AnimateMenuRandom();

        yield return new WaitForSeconds(0.15f);

        LevelSelection.Instance.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Animate from the level selection
    /// </summary>
    /// <returns></returns>
    public IEnumerator AnimateFromLevelSelect()
    {
        //Init
        gameObject.SetActive(true);
        btn_music.GetComponent<MusicButton>().Init();
        btn_vibration.GetComponent<VibrationButton>().Init();

        //Animate background
        LeanTween.color(selectLevelBackground, colorToAnimateRandomBackgroundFrom, 0.35f);
        LeanTween.value(selectLevelBackground.gameObject, 4000, 312, 0.35f).setEase(LeanTweenType.easeOutSine)
            .setOnUpdate(
                (float value) => { selectLevelBackground.sizeDelta = new Vector2(value, value); });

        yield return new WaitForSeconds(0.2f);

        //Animate in each element
        LeanTween.alphaCanvas(selectLevelText, 1f, 0.15f);
        LeanTween.scale(currentLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 1f, 0.15f)
            .setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(txt_continue, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.alphaCanvas(randomText, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(randomLevelBackground, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);

        //Animate in the menu buttons
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
    #endregion
    #region Private
    /// <summary>
    /// Continue to the game
    /// </summary>
    private void Continue()
    {
        AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Continue);
        StartCoroutine(AnimateOut());
    }

    /// <summary>
    /// Open the feedback popup
    /// </summary>
    private void Feedback()
    {
        AnalyticsManager.Instance.LogFirstAction(AnalyticsManager.FirstAction.Feedback);
        PopupManager.Instance.ShowFeedbackPopup();
    }

    /// <summary>
    /// Animate the settings screen out of view
    /// </summary>
    private IEnumerator AnimateOut()
    {
        LevelSelection.Instance.gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = false;

        Color colorToAnimateTo = GridGenerator3D.Instance.SetBackgroundColours(false, true);

        //Animate out elements
        LeanTween.value(currentLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { currentLevelBackground.sizeDelta = new Vector2(value, value); });

        LeanTween.scale(randomLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        LeanTween.scale(selectLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        LeanTween.alphaCanvas(txt_currentLevel.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);

        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);
        LeanTween.color(currentLevelBackground, colorToAnimateTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);
        
        //Start the level
        GameManager.Instance.StartLevel(PlayerInfoManager.Instance.LevelsUnlocked);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Animate the settings menu open
    /// </summary>
    private IEnumerator AnimateSettingsOpen()
    {
        settingsOpen = true;

        //Animate each of the menu buttons out of view
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        RectTransform restoreRect = (RectTransform)btn_restore.transform;

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

        yield return new WaitForSeconds(0.15f);

        settingsButtons.gameObject.SetActive(true);
        menuButtons.SetActive(false);
        returnIcon.localScale = Vector3.zero;

        //Animate the settings buttons into view
        LeanTween.scale(returnIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        RectTransform dataRect = (RectTransform)btn_dataCollection.transform;
        RectTransform creditsRect = (RectTransform) btn_credits.transform;
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;
        musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, -50f);
        dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, -50f);
        creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, -50f);
        vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, -50f);

        LeanTween.value(btn_dataCollection.gameObject, -50f, -200f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.2f);

        LeanTween.value(btn_credits.gameObject, -200, -350, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
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

        LeanTween.value(btn_vibration.gameObject, -500, -650, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
    }

    /// <summary>
    /// Animate the settings menu close
    /// </summary>
    private IEnumerator AnimateSettingsClose()
    {
        //Animate out the settings buttons
        settingsOpen = false;
        RectTransform dataRect = (RectTransform)btn_dataCollection.transform;
        RectTransform creditsRect = (RectTransform) btn_credits.transform;
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;

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

        LeanTween.value(btn_credits.gameObject, -350, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);

        LeanTween.value(btn_dataCollection.gameObject, -200, -50, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });

        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        yield return new WaitForSeconds(0.15f);
        settingsButtons.SetActive(false);
        menuButtons.SetActive(true);

        //Animate in the menu buttons
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform)btn_restore.transform;
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

    /// <summary>
    /// Animate the menu buttons disappearing when continue is pressed
    /// </summary>
    private IEnumerator AnimateMenuContinue()
    {
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform)btn_restore.transform;
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

    /// <summary>
    /// Animate the settings buttons disappearing when continue is pressed and settings is open
    /// </summary>
    private IEnumerator AnimateSettingsContinue()
    {
        settingsOpen = false;
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform dataRect = (RectTransform)btn_dataCollection.transform;
        RectTransform creditsRect = (RectTransform) btn_credits.transform;
        RectTransform vibrationRect = (RectTransform)btn_vibration.transform;

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

        LeanTween.value(btn_credits.gameObject, -350, -200, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);

        LeanTween.value(btn_dataCollection.gameObject, -200f, -50f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                dataRect.anchoredPosition = new Vector2(dataRect.anchoredPosition.x, value);
                creditsRect.anchoredPosition = new Vector2(creditsRect.anchoredPosition.x, value);
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
            });

        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        yield return new WaitForSeconds(0.2f);
    }

    /// <summary>
    /// Animate out the start screen to a random level
    /// </summary>
    /// <param name="randomLevel">Is a random level?</param>
    private IEnumerator AnimateOutRandomLevel(bool randomLevel)
    {
        canvasGroup.blocksRaycasts = false;

        //Animate all elements out of view
        LeanTween.value(randomLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { randomLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.scale(currentLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomText.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(randomDice, 0, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(selectLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);

        LeanTween.color(randomLevelBackground, colorToAnimateRandomBackgroundTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        //Start a random level
        if (randomLevel)
            GameManager.Instance.StartRandomLevel(false);
        else
        {
            GameManager.Instance.StartLevel(PlayerInfoManager.Instance.LevelsUnlocked);
        }

        LevelSelection.Instance.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Animate to a random level with the menu buttons visible
    /// </summary>
    private IEnumerator AnimateMenuRandom()
    {
        LeanTween.color((RectTransform)btn_settings.transform, colorToAnimateSettingsFrom, 0.15f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);
        LeanTween.scale(settingsIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);

        yield return new WaitForSeconds(0.15f);

        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform restoreRect = (RectTransform)btn_restore.transform;
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

    /// <summary>
    /// Animate to the level selection
    /// </summary>
    private IEnumerator AnimateToLevelSelect()
    {
        canvasGroup.blocksRaycasts = false;

        //Animate out each element
        LeanTween.value(selectLevelBackground.gameObject, 312, 4000, 0.35f).setEase(LeanTweenType.easeInSine)
            .setOnUpdate(
                (float value) => { selectLevelBackground.sizeDelta = new Vector2(value, value); });
        LeanTween.scale(randomLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(currentLevelBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(selectLevelText, 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(txt_continue, 0f, 0.15f).setEase(LeanTweenType.easeInSine);

        if (settingsOpen)
            yield return AnimateSettingsContinue();
        else
            yield return AnimateMenuContinue();

        yield return new WaitForSeconds(0.15f);

        LeanTween.color(selectLevelBackground, colorToAnimateLevelSelectButtonTo, 0.25f)
            .setEase(LeanTweenType.easeInOutSine).setRecursive(false);

        yield return new WaitForSeconds(0.25f);

        //Go to level selection
        LevelSelection.Instance.gameObject.SetActive(true);
        LevelSelection.Instance.SetupLevelSelect();
        gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = true;
    }
    #endregion
    #endregion
}
