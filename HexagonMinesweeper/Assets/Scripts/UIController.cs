using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIController : MonoBehaviour
{
    #region Varaibles
    #region Static
    public static UIController Instance;
    #endregion
    #region Editor
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI bombsRemainingText;

    /*[SerializeField] private Button btn_menu;

    [SerializeField] private RectTransform img_menuIcon;*/

    [SerializeField] private RectTransform img_currentLevelBackground;

    [SerializeField] private RectTransform img_bombsRemainingBackground;

    [SerializeField] private TextMeshProUGUI completeText;

    [SerializeField] private Button btn_dim;

    [SerializeField] private Button btn_quit;

    [SerializeField] private RectTransform quitIcon;

    [SerializeField] private Button btn_settings;

    [SerializeField] private Button btn_noAds;

    [SerializeField] private GameObject menuButtons;

    [SerializeField] private Button btn_skip;

    [SerializeField] private Button btn_feedback;

    [SerializeField] private Color whiteColour;
    [SerializeField] private Color greyColour;

    [Header("Settings")]
    [SerializeField] private GameObject settings;

    [SerializeField] private Button btn_settingsReturn;

    [SerializeField] private RectTransform returnIcon;

    [SerializeField] private Button btn_music;

    [SerializeField] private Button btn_vibration;

    [SerializeField] private Button btn_colours;

    [SerializeField] private RectTransform mainCanvas;

    [Header("Tutorial")]
    [SerializeField] private RectTransform hand;

    [Header("End screen")] [SerializeField]
    private GameObject txt_incredible;

    [SerializeField] private GameObject txt_thanks;

    [SerializeField] private GameObject txt_neverExpected;

    [SerializeField] private GameObject txt_tweetMe;

    [SerializeField] private Button btn_playRandomLevel;

    [SerializeField] private CanvasGroup endGame;

    private bool isShowingHand = false;

    #endregion
    #region Private
    private CanvasGroup canvasGroup;

    private string[] completeTexts =
    {
        "Well done!",
        "Amazing!",
        "Wow!",
        "OMG!",
        "Incredible!",
        "Unbelievable!",
        "Awesome!",
        "Superb!",
        "Smashing!",
        "Extraordinary!",
        "Astonishing!"
    };

    private bool isOnSettings;

    private int currentlyActiveTip = 0;

    private bool isShown;

    private int currentlyShowingTutorialStep = 0;

    private Coroutine currentlyPlayingHandTutorial;
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        canvasGroup = GetComponent<CanvasGroup>();
        img_currentLevelBackground.anchoredPosition = new Vector2(250f, 250f);
        levelText.GetComponent<CanvasGroup>().alpha = 0f;
        btn_quit.transform.localScale = Vector3.zero;
        btn_settings.transform.localScale = Vector3.zero;
        btn_feedback.transform.localScale = Vector3.zero;
        btn_noAds.transform.localScale = Vector3.zero;
        //LeanTween.alpha((RectTransform) btn_menu.transform, 0f, 0f);
        //LeanTween.alpha(img_menuIcon, 0f, 0f);
        img_bombsRemainingBackground.localScale = Vector3.zero;
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 0f, 0f);
        completeText.transform.localScale = Vector3.zero;
        menuButtons.SetActive(false);
        Show(false);
        btn_dim.gameObject.SetActive(false);
        //btn_menu.onClick.AddListener(() => StartCoroutine(AnimateMenuOpen()));
        btn_dim.onClick.AddListener(() =>
        {
            StartCoroutine(AnimateSettingsReturn());
            /*if (isOnSettings)
                StartCoroutine(AnimateSettingsQuit());
            else
                StartCoroutine(AnimateMenuClose());*/
        });
            
        btn_quit.onClick.AddListener(Quit);
        btn_skip.onClick.AddListener(Skip);
        btn_skip.gameObject.SetActive(false);
        btn_feedback.onClick.AddListener(Feedback);
        btn_settings.onClick.AddListener(() => StartCoroutine(AnimateSettingsOpen()));
        btn_settingsReturn.onClick.AddListener(() => StartCoroutine(AnimateSettingsReturn()));
        btn_noAds.onClick.AddListener(() =>
        {
            PopupManager.Instance.ShowRemoveAds(() => Purchaser.Instance.BuyNoAds());
        });
        btn_playRandomLevel.onClick.AddListener(() =>
        {
            ResetUI();
            isShown = false;
            LeanTween.alphaCanvas(endGame, 0f, 0.15f).setEase(LeanTweenType.easeInSine)
                .setOnComplete(() =>
                {
                    endGame.gameObject.SetActive(false);
                    txt_tweetMe.SetActive(false);
                    btn_playRandomLevel.gameObject.SetActive(false);
                    txt_incredible.SetActive(true);
                });
            canvasGroup.blocksRaycasts = false;
            GameManager.Instance.StartRandomLevel(true);
        });
        settings.gameObject.SetActive(false);
        //InitSettingsButtons();
    }

    private void Update()
    {
        if (isShowingHand)
            if (Input.GetMouseButtonDown(0))
            {
                isShowingHand = false;
               StopCoroutine(currentlyPlayingHandTutorial);
                LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 0f, 0.25f).setEase(LeanTweenType.easeInSine)
                    .setOnComplete(
                        () => { hand.gameObject.SetActive(false); });
            }
                
    }
    #endregion
    #region Public

    public void InitSettingsButtons()
    {
        btn_music.GetComponent<MusicButton>().Init();
        btn_vibration.GetComponent<VibrationButton>().Init();
        btn_colours.GetComponent<ColourButton>().Init();
    }

    public void UpdateLevel(int level)
    {
        levelText.text = string.Format("Level\n<size=60>{0}</size>", level.ToString());
    }

    public void UpdateBombsRemaining(int bombsRemaining)
    {
        if (bombsRemaining == 1)
        {
            bombsRemainingText.text = string.Format("<size=80>{0}</size>\nBomb", bombsRemaining.ToString());
        }
        else
            bombsRemainingText.text = string.Format("<size=80>{0}</size>\nBombs", bombsRemaining.ToString());
    }

    public void Show(bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.blocksRaycasts = show;
    }

    public IEnumerator AnimateInUI()
    {
        if (isShown)
            yield break;
        isShown = true;
        ResetUI();
        canvasGroup.blocksRaycasts = true;
        LeanTween.value(gameObject, img_currentLevelBackground.anchoredPosition, new Vector2(-50f, -50f), 0.25f)
            .setEase(LeanTweenType.easeOutSine).setOnUpdate(
                (Vector2 value) => { img_currentLevelBackground.anchoredPosition = value; });
        LeanTween.scale(btn_quit.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.1f);
        LeanTween.scale(btn_settings.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.1f);
        LeanTween.scale(btn_feedback.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.1f);
        LeanTween.scale(btn_noAds.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        /*LeanTween.alpha((RectTransform)btn_menu.transform, 1f, 0.25f).setEase(LeanTweenType.easeOutSine)
            .setRecursive(false);*/
        yield return new WaitForSeconds(0.2f);
        LeanTween.alphaCanvas(levelText.GetComponent<CanvasGroup>(), 1f, 0.25f).setEase(LeanTweenType.easeOutSine);
        //LeanTween.alpha(img_menuIcon, 1f, 0.25f).setEase(LeanTweenType.easeOutSine);
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
        LeanTween.alphaCanvas(completeText.GetComponent<CanvasGroup>(), 1f, 0f).setEase(LeanTweenType.easeInSine);
        HideSkipButton();
        completeText.text = completeTexts[Random.Range(0, completeTexts.Length)];
        int random = Random.Range(0, 3);
        RectTransform textRect = (RectTransform)completeText.transform;
        completeText.transform.localScale = Vector3.zero;
        switch (random)
        {
            case 0:
                
                yield return null;
                LeanTween.scale(completeText.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
                yield return new WaitForSeconds(0.5f);
                LeanTween.scale(completeText.gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong(3);
                yield return new WaitForSeconds(3f);
                LeanTween.alphaCanvas(completeText.GetComponent<CanvasGroup>(), 0f, 0.35f).setEase(LeanTweenType.easeInSine);
                yield return new WaitForSeconds(0.35f);
                LeanTween.alphaCanvas(completeText.GetComponent<CanvasGroup>(), 1f, 0f).setEase(LeanTweenType.easeInSine);
                break;
            case 1:
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, 100f);
                LeanTween.value(textRect.gameObject, textRect.anchoredPosition.y, -450f, 0.35f)
                    .setEase(LeanTweenType.easeOutSine).setOnUpdate(
                        (float value) =>
                        {
                            textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, value);
                        });
                LeanTween.scale(completeText.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
                yield return new WaitForSeconds(3f);
                LeanTween.value(textRect.gameObject, textRect.anchoredPosition.y, 100f, 0.25f)
                    .setEase(LeanTweenType.easeInSine).setOnUpdate(
                        (float value) =>
                        {
                            textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, value);
                        });
                yield return new WaitForSeconds(0.25f);
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, -450f);
                break;
            case 2:
                textRect = (RectTransform)completeText.transform;
                textRect.anchoredPosition = new Vector2(-mainCanvas.sizeDelta.x, textRect.anchoredPosition.y);
                LeanTween.scale(completeText.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
                LeanTween.value(textRect.gameObject, textRect.anchoredPosition.x, 0f, 0.35f)
                    .setEase(LeanTweenType.easeOutSine).setOnUpdate(
                        (float value) =>
                        {
                            textRect.anchoredPosition = new Vector2(value, textRect.anchoredPosition.y);
                        });
                yield return new WaitForSeconds(3f);
                LeanTween.value(textRect.gameObject, textRect.anchoredPosition.x, mainCanvas.sizeDelta.x, 0.25f)
                    .setEase(LeanTweenType.easeInSine).setOnUpdate(
                        (float value) =>
                        {
                            textRect.anchoredPosition = new Vector2(value, textRect.anchoredPosition.y);
                        });
                yield return new WaitForSeconds(0.25f);
                textRect.anchoredPosition = new Vector2(0f, textRect.anchoredPosition.y);
                break;
        }


        
        completeText.transform.localScale = Vector3.zero;
    }

    public void ShowTutorialTip(int step)
    {
        LeanTween.cancel(hand);
        if (currentlyPlayingHandTutorial != null)
            StopCoroutine(currentlyPlayingHandTutorial);
        currentlyShowingTutorialStep = step;
        if (step == 3)
        {
            StartCoroutine(AnimateHandLevel3Part2());
            return;
        }
        currentlyActiveTip = step;
        hand.gameObject.SetActive(true);
        hand.GetComponent<CanvasGroup>().alpha = 0f;
        LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        
        if (step == 0)
            currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel1());
        else if (step == 1)
        {
            currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel2());
        }
        else if (step == 2)
        {
            currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel3Part1());
        }
    }

    private IEnumerator AnimateHandLevel1()
    {
        float movementTime = 0.5f;
        hand.anchoredPosition = new Vector2(23f, -738f);
        yield return new WaitForSeconds(1f);
        isShowingHand = true;
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(345f, -555f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(345f, -200f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23f, -50f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-284f, -200f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-284f, -555f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -738f), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);

        currentlyPlayingHandTutorial = StartCoroutine(CheckHand());
    }

    private IEnumerator AnimateHandLevel2()
    {
        float movementTime = 0.5f;
        hand.anchoredPosition = new Vector2(23f, -1100);
        yield return new WaitForSeconds(1f);
        isShowingHand = true;
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -738), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -370), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -10), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 0f, 0.5f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.5f);
        hand.anchoredPosition = new Vector2(23f, -1100);
        LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        currentlyPlayingHandTutorial = StartCoroutine(CheckHand());
    }

    private IEnumerator AnimateHandLevel3Part1()
    {
        float movementTime = 0.5f;
        hand.anchoredPosition = new Vector2(-286, -576);
        yield return new WaitForSeconds(1f);
        isShowingHand = true;
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -396), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(23, -6), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-286, 152), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-586, -6), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-586, -396), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(-286, -576), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);

        currentlyPlayingHandTutorial = StartCoroutine(CheckHand());
    }

    private IEnumerator AnimateHandLevel3Part2()
    {
        hand.gameObject.SetActive(true);
        float movementTime = 0.5f;
        hand.anchoredPosition = new Vector2(643, -396);
        yield return new WaitForSeconds(1f);
        isShowingHand = true;
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(342, -567), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.value(hand.gameObject, hand.anchoredPosition, new Vector2(342, -936), movementTime)
            .setEase(LeanTweenType.easeInOutSine).setOnUpdate(
                (Vector2 value) => { hand.anchoredPosition = value; });
        yield return new WaitForSeconds(0.5f);
        LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 0f, 0.5f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.5f);
        hand.anchoredPosition = new Vector2(643, -396);
        LeanTween.alphaCanvas(hand.GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        currentlyPlayingHandTutorial = StartCoroutine(CheckHand());
    }

    private IEnumerator CheckHand()
    {
        yield return new WaitForSeconds(5f);
        if (!isShowingHand)
            yield break;

        switch (currentlyShowingTutorialStep)
        {
            case 0:
                currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel1());
                break;
            case 1:
                currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel2());
                break;
            case 2:
                currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel3Part1());
                break;
            case 3:
                currentlyPlayingHandTutorial = StartCoroutine(AnimateHandLevel3Part2());
                break;
        }
    }

    public void HideCurrentlyActiveTip()
    {
        int currentlyShowingTip = currentlyActiveTip;

    }
    #endregion
    #region Private

    private void ResetUI()
    {
        img_currentLevelBackground.anchoredPosition = new Vector2(250f, 250f);
        levelText.GetComponent<CanvasGroup>().alpha = 0f;
        btn_quit.transform.localScale = Vector3.zero;
        btn_settings.transform.localScale = Vector3.zero;
        btn_feedback.transform.localScale = Vector3.zero;
        btn_noAds.transform.localScale = Vector3.zero;
        img_bombsRemainingBackground.localScale = Vector3.zero;
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 0f, 0f);
        completeText.transform.localScale = Vector3.zero;
        quitIcon.localScale = Vector3.one;
        ((RectTransform)btn_settings.transform).anchoredPosition = new Vector2(105f, -230f);
        ((RectTransform)btn_feedback.transform).anchoredPosition = new Vector2(105f, -355f);
        ((RectTransform)btn_noAds.transform).anchoredPosition = new Vector2(105f, -480f);
        img_currentLevelBackground.localScale = Vector3.one;

    }
    private IEnumerator AnimateMenuOpen()
    {
        yield return null;
        btn_dim.gameObject.SetActive(true);
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0f);
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform) btn_feedback.transform;
        settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, -50f);
        noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, -50f);
        feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, -50f);
        menuButtons.gameObject.SetActive(true);
        quitIcon.localScale = Vector3.zero;
        yield return null;
        LeanTween.scale(quitIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.15f);
        LeanTween.alpha((RectTransform)btn_dim.transform, 0.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.value(btn_settings.gameObject, -105f, -230f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_feedback.gameObject, -230f, -355f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -480f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
    }

    private IEnumerator AnimateMenuClose()
    {
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -355f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -355f, -230f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_settings.gameObject, -230f, -105f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        btn_dim.gameObject.SetActive(false);
        menuButtons.gameObject.SetActive(false);
    }

    private void Quit()
    {
        GameManager.Instance.QuitLevel();
        canvasGroup.blocksRaycasts = false;
        endGame.gameObject.SetActive(false);
        AdManager.Instance.HideBanner();
        UIController.Instance.HideCurrentlyActiveTip();
        if (GameManager.Instance.IsRandomLevel)
            StartCoroutine(AnimateQuitRandom());
        else
            StartCoroutine(AnimateQuit());
        //GridGenerator.Instance.QuitGame();
        GridGenerator3D.Instance.QuitGame();
    }

    private IEnumerator AnimateQuit()
    {
        HideSkipButton();
        //LevelSelection.Instance.gameObject.SetActive(true);
        StartCoroutine(AnimateButtonUIQuit());
        yield return new WaitForSeconds(0.3f);
        LeanTween.alphaCanvas(levelText.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        //LeanTween.alpha(img_menuIcon, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(gameObject, img_currentLevelBackground.anchoredPosition, new Vector2(275f, 152f), 0.15f)
            .setEase(LeanTweenType.easeInSine).setOnUpdate(
                (Vector2 value) => { img_currentLevelBackground.anchoredPosition = value; });
        /*LeanTween.alpha((RectTransform)btn_menu.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine)
            .setRecursive(false);*/
        LeanTween.scale(img_bombsRemainingBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(StartScreen.Instance.AnimateIn());
        //LevelSelection.Instance.Return();;
        isShown = false;
    }

    private IEnumerator AnimateButtonUIQuit()
    {
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        LeanTween.alpha((RectTransform)btn_dim.transform, 0.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -355f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -355f, -230f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_settings.gameObject, -230f, -105f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
    }

    private void Skip()
    {
        AdManager.Instance.ShowRewardAd(false, () =>
        {
            GameManager.Instance.SkipLevel();
        });

        LeanTween.alphaCanvas(btn_skip.GetComponent<CanvasGroup>(), 0f, 0.25f).setEase(LeanTweenType.easeInSine)
            .setOnComplete(() => btn_skip.gameObject.SetActive(false));
    }

    public void ShowSkipButton()
    {
        btn_skip.gameObject.SetActive(true);
        btn_skip.GetComponent<CanvasGroup>().alpha = 1f;
        btn_skip.transform.localScale = Vector3.zero;
        LeanTween.scale(btn_skip.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }

    public void HideSkipButton()
    {
        LeanTween.alphaCanvas(btn_skip.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine).setOnComplete(
            () => { btn_skip.gameObject.SetActive(false); });
    }

    private void Feedback()
    {
        PopupManager.Instance.ShowFeedbackPopup();
    }

    private IEnumerator AnimateSettingsOpen()
    {
        isOnSettings = true;
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;
        btn_dim.gameObject.SetActive(true);
        LeanTween.alpha((RectTransform)btn_dim.transform, 0.25f, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -355f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_feedback.gameObject, -355f, -230f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_settings.gameObject, -230f, -105f, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        LeanTween.scale(quitIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        settings.gameObject.SetActive(true);
        menuButtons.SetActive(false);
        returnIcon.localScale = Vector3.zero;
        LeanTween.scale(returnIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        RectTransform musicRect = (RectTransform) btn_music.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        RectTransform coloursRect = (RectTransform) btn_colours.transform;
        musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, -50f);
        vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, -50f);
        coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, -50f);

        LeanTween.value(btn_music.gameObject, -50, -175, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_vibration.gameObject, -175, -300, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_colours.gameObject, -300, -425, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
    }

    private IEnumerator AnimateSettingsReturn()
    {
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        RectTransform coloursRect = (RectTransform) btn_colours.transform;
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.value(btn_colours.gameObject, -425, -300, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_vibration.gameObject, -300, -175, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_music.gameObject, -175, -50, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.15f);
        btn_dim.gameObject.SetActive(false);
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        menuButtons.gameObject.SetActive(true);
        settings.gameObject.SetActive(false);
        isOnSettings = false;

        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0f);
        RectTransform settingsRect = (RectTransform)btn_settings.transform;
        RectTransform noAdsRect = (RectTransform)btn_noAds.transform;
        RectTransform feedbackRect = (RectTransform)btn_feedback.transform;

        LeanTween.scale(quitIcon, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.15f);
        
        LeanTween.value(btn_settings.gameObject, -105f, -230f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                settingsRect.anchoredPosition = new Vector2(settingsRect.anchoredPosition.x, value);
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_feedback.gameObject, -230f, -355f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
                feedbackRect.anchoredPosition = new Vector2(feedbackRect.anchoredPosition.x, value);
            });
        yield return new WaitForSeconds(0.2f);
        LeanTween.value(btn_noAds.gameObject, noAdsRect.anchoredPosition.y, -480f, 0.15f).setEase(LeanTweenType.easeOutBack).setOnUpdate(
            (float value) =>
            {
                noAdsRect.anchoredPosition = new Vector2(noAdsRect.anchoredPosition.x, value);
            });
        //yield return AnimateMenuOpen();
    }

    private IEnumerator AnimateSettingsQuit()
    {
        RectTransform musicRect = (RectTransform)btn_music.transform;
        RectTransform vibrationRect = (RectTransform) btn_vibration.transform;
        RectTransform coloursRect = (RectTransform) btn_colours.transform;
        LeanTween.alpha((RectTransform)btn_dim.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine);

        LeanTween.value(btn_colours.gameObject, -425, -300, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_vibration.gameObject, -300, -175, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        LeanTween.value(btn_music.gameObject, -175, -50, 0.15f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) =>
            {
                musicRect.anchoredPosition = new Vector2(musicRect.anchoredPosition.x, value);
                vibrationRect.anchoredPosition = new Vector2(vibrationRect.anchoredPosition.x, value);
                coloursRect.anchoredPosition = new Vector2(coloursRect.anchoredPosition.x, value);
            });

        yield return new WaitForSeconds(0.15f);
        LeanTween.scale(returnIcon, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        isOnSettings = false;
        settings.SetActive(false);
        btn_dim.gameObject.SetActive(false);
    }

    private IEnumerator AnimateQuitRandom()
    {
        StartCoroutine(AnimateButtonUIQuit());
        yield return new WaitForSeconds(0.3f);
        LeanTween.alphaCanvas(levelText.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        //LeanTween.alpha(img_menuIcon, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.alphaCanvas(bombsRemainingText.GetComponent<CanvasGroup>(), 0f, 0.15f)
            .setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        LeanTween.value(gameObject, img_currentLevelBackground.anchoredPosition, new Vector2(275f, 152f), 0.15f)
            .setEase(LeanTweenType.easeInSine).setOnUpdate(
                (Vector2 value) => { img_currentLevelBackground.anchoredPosition = value; });
        /*LeanTween.alpha((RectTransform)btn_menu.transform, 0f, 0.25f).setEase(LeanTweenType.easeInSine)
            .setRecursive(false);*/
        LeanTween.scale(img_bombsRemainingBackground, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(StartScreen.Instance.AnimateInRandomLevel());
        isShown = false;
    }

    public IEnumerator AnimateFinalLevelFinished()
    {
        yield return new WaitForSeconds(2f);
        //yield return GridGenerator.Instance.AnimateCentreBombExplosion();
        yield return GridGenerator3D.Instance.AnimateCentreBombExplosion();
        LeanTween.scale(img_currentLevelBackground, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(img_bombsRemainingBackground, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.5f);
        endGame.gameObject.SetActive(true);
        endGame.alpha = 0f;
        txt_incredible.SetActive(true);
        LeanTween.alphaCanvas(endGame, 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        yield return new WaitForSeconds(4f);
        LeanTween.alphaCanvas(endGame, 0f, 0.5f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.75f);
        txt_incredible.SetActive(false);
        txt_thanks.SetActive(true);
        LeanTween.alphaCanvas(endGame, 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        yield return new WaitForSeconds(5f);
        LeanTween.alphaCanvas(endGame, 0f, 0.5f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.75f);
        txt_thanks.SetActive(false);
        txt_neverExpected.SetActive(true);
        LeanTween.alphaCanvas(endGame, 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
        yield return new WaitForSeconds(6f);
        LeanTween.alphaCanvas(endGame, 0f, 0.5f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.75f);
        txt_neverExpected.SetActive(false);
        txt_tweetMe.SetActive(true);
        btn_playRandomLevel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(endGame, 1f, 0.5f).setEase(LeanTweenType.easeOutSine);
    }

    public void SetTextColour(int colour)
    {
        if (colour == 0 || colour == 2 || colour == 7 || colour == 8 || colour == 9)
        {
            completeText.color = whiteColour;

            txt_neverExpected.GetComponent<TextMeshProUGUI>().color = whiteColour;
            txt_incredible.GetComponent<TextMeshProUGUI>().color = whiteColour;
            txt_thanks.GetComponent<TextMeshProUGUI>().color = whiteColour;
            txt_tweetMe.GetComponent<TextMeshProUGUI>().color = whiteColour;
        }
        else
        {
            completeText.color = greyColour;

            txt_neverExpected.GetComponent<TextMeshProUGUI>().color = greyColour;
            txt_incredible.GetComponent<TextMeshProUGUI>().color = greyColour;
            txt_thanks.GetComponent<TextMeshProUGUI>().color = greyColour;
            txt_tweetMe.GetComponent<TextMeshProUGUI>().color = greyColour;
        }
    }
    #endregion
    #endregion
}
