using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// A static instance of this script
    /// </summary>
    public static PopupManager Instance;

    /// <summary>
    /// The feedback popup
    /// </summary>
    [SerializeField]
    private FeedbackPopup feedbackPopup;

    /// <summary>
    /// The random levels warning popup
    /// </summary>
    [SerializeField]
    private RandomLevelsPopup randomPopup;

    /// <summary>
    /// The remove ads popup
    /// </summary>
    [SerializeField]
    private RemoveAdsPopup removeAdsPopup;

    /// <summary>
    /// The restore purchases popup
    /// </summary>
    [SerializeField]
    private RestorePurchasesPopup restorePurchasesPopup;

    /// <summary>
    /// The data collection settings popup
    /// </summary>
    [SerializeField]
    private DataCollectionPopup dataCollectionPopup;

    /// <summary>
    /// The credits popup
    /// </summary>
    [SerializeField]
    private CreditsPopup creditsPopup;

    /// <summary>
    /// The dim image
    /// </summary>
    [SerializeField]
    private CanvasGroup img_dim;

    /// <summary>
    /// Used to check if a popup is currently being shown
    /// </summary>
    public bool IsShowingPopup { get; private set; }
    #endregion
    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        img_dim.alpha = 0f;
        img_dim.blocksRaycasts = false;
    }
    #endregion
    #region Public
    /// <summary>
    /// Show the feedback popup
    /// </summary>
    public void ShowFeedbackPopup()
    {
        IsShowingPopup = true;
        feedbackPopup.gameObject.SetActive(true);
        feedbackPopup.Init();
    }
    /// <summary>
    /// Show the random levels warning popup
    /// </summary>
    /// <param name="callback">The callback to invoke when the popup has successfully completed</param>
    public void ShowRandomPopup(UnityAction callback)
    {
        IsShowingPopup = true;
        randomPopup.gameObject.SetActive(true);
        randomPopup.Init(callback);
    }

    /// <summary>
    /// Show the remove ads popup
    /// </summary>
    /// <param name="callback">The callback to invoke when the popup has successfully completed</param>
    public void ShowRemoveAds(UnityAction callback)
    {
        IsShowingPopup = true;
        removeAdsPopup.gameObject.SetActive(true);
        removeAdsPopup.Init(callback);
    }

    /// <summary>
    /// Show the restore purchases popup
    /// </summary>
    public void ShowRestorePurchases()
    {
        IsShowingPopup = true;
        restorePurchasesPopup.gameObject.SetActive(true);
        restorePurchasesPopup.Init();
    }

    /// <summary>
    /// Show the data collection settings popup
    /// </summary>
    public void ShowDataCollectionPopup()
    {
        IsShowingPopup = true;
        dataCollectionPopup.gameObject.SetActive(true);
        dataCollectionPopup.Init();
    }

    /// <summary>
    /// Show the credits popup
    /// </summary>
    public void ShowCreditsPopup()
    {
        IsShowingPopup = true;
        creditsPopup.gameObject.SetActive(true);
        creditsPopup.Init();
    }

    /// <summary>
    /// Reset the is showing popup variable when a popup is closed
    /// </summary>
    public void Hide()
    {
        IsShowingPopup = false;
    }
    #endregion
    #endregion
}
