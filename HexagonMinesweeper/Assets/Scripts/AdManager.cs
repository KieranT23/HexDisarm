using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.Monetization;
using ShowResult = UnityEngine.Advertisements.ShowResult;

public class AdManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static AdManager Instance;

    /// <summary>
    /// All the no ads buttons
    /// </summary>
    [SerializeField]
    private CanvasGroup[] noAdsButtons;

    /// <summary>
    /// Check if a banner is currently being shown
    /// </summary>
    private bool isShowingBanner;
    /// <summary>
    /// Check if the user has removed ads
    /// </summary>
    private bool hasRemovedAds;

    /// <summary>
    /// Check if an ad is currently being shown to the user
    /// </summary>
    public bool IsShowingAd { get; private set; }
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        //Initialise advertisements
        hasRemovedAds = PlayerInfoManager.Instance.HasRemovedAds;

        if (hasRemovedAds)
            RemoveAds();

#if UNITY_ANDROID
            Advertisement.Initialize("3329503", false);
#elif UNITY_IOS
            Advertisement.Initialize("3329502", false);
#endif

    }
    #endregion

    #region Public
    /// <summary>
    /// Show the banner advertisment
    /// </summary>
    public void ShowBanner()
    {
        if (isShowingBanner || hasRemovedAds)
            return;

        isShowingBanner = true;

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        if (Advertisement.IsReady("InGameBanner"))
        {
            Advertisement.Banner.Show("InGameBanner");
        }
    }

    /// <summary>
    /// Hide the banner advertisement
    /// </summary>
    public void HideBanner()
    {
        if (!isShowingBanner)
            return;

        isShowingBanner = false;

        Advertisement.Banner.Hide();
    }

    /// <summary>
    /// Show an in-game ad - between levels
    /// </summary>
    public void ShowInGameAd()
    {
        if (hasRemovedAds || IsShowingAd)
            return;

        if (Advertisement.IsReady("video"))
            Advertisement.Show("video");

        IsShowingAd = true;
        StartCoroutine(CheckForAdShowing());
    }

    /// <summary>
    /// Show a rewarded advertisement
    /// </summary>
    /// <param name="levelSelect">Is it on the level selection?</param>
    /// <param name="successCallback">The callback to invoke if the user successfully watches the advert</param>
    public void ShowRewardAd(bool levelSelect, UnityAction successCallback)
    {
        ShowOptions options = new ShowOptions();

        //This is obselete but the other ways didn't work
        options.resultCallback = result =>
        {
            if (result == ShowResult.Finished)
                successCallback.Invoke();
        };

        if (Advertisement.IsReady("UnlockLevel"))
            Advertisement.Show("UnlockLevel", options); 
    }

    /// <summary>
    /// Remove in-game and banner advertismenets
    /// </summary>
    public void RemoveAds()
    {
        foreach (CanvasGroup canvasGroup in noAdsButtons)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        PlayerInfoManager.Instance.HasRemovedAds = true;
        hasRemovedAds = true;
        HideBanner();
    }
    #endregion
    #region Private
    /// <summary>
    /// Check to see if an ad is currently being shown
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckForAdShowing()
    {
        yield return new WaitUntil(() => !Advertisement.isShowing);
        IsShowingAd = false;
    }
    #endregion
    #endregion

    

    
}
