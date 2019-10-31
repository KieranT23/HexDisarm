using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.Monetization;
using ShowResult = UnityEngine.Advertisements.ShowResult;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private bool isShowingBanner;

    public bool IsShowingAd { get; private set; }

    private bool hasRemovedAds;

    [SerializeField] private CanvasGroup[] noAdsButtons;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        hasRemovedAds = PlayerInfoManager.Instance.HasRemovedAds;
        if (hasRemovedAds)
            RemoveAds();
        else
            Advertisement.Initialize("3329503", false);
        
    }
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            if (Advertisement.IsReady("InGameBanner"))
            {
                Advertisement.Banner.Show("InGameBanner");
            }
        }*/
    }

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

    public void HideBanner()
    {
        if (!isShowingBanner)
            return;
        isShowingBanner = false;

        Advertisement.Banner.Hide();
    }

    public void ShowInGameAd()
    {
        if (hasRemovedAds || IsShowingAd)
            return;
        ShowOptions options = new ShowOptions();;
        if (Advertisement.IsReady("video"))
            Advertisement.Show("video");
        IsShowingAd = true;
        StartCoroutine(CheckForAdShowing());
    }

    public void ShowRewardAd(bool levelSelect, UnityAction successCallback)
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = result =>
        {
            if (result == ShowResult.Finished)
            {
                successCallback.Invoke();
            }
            else if (result == ShowResult.Skipped)
            {
                Debug.Log("Skipped");
                // Do not reward the user for skipping the ad.
            }
            else if (result == ShowResult.Failed)
            {
                Debug.Log("Failed");
                //Debug.LogWarning(“The ad did not finish due to an error.”);
            }
        };
        if (Advertisement.IsReady("UnlockLevel"))
            Advertisement.Show("UnlockLevel", options);

        
    }

    private IEnumerator CheckForAdShowing()
    {
        yield return new WaitUntil(() => !Advertisement.isShowing);
        IsShowingAd = false;
    }

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

    
}
