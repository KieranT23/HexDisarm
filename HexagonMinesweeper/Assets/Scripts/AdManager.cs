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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        Advertisement.Initialize("3329503", true);
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
        if (isShowingBanner)
            return;

        if (Advertisement.IsReady("InGameBanner"))
        {
            Advertisement.Banner.Show("InGameBanner");
        }
    }

    public void HideBanner()
    {
        if (!isShowingBanner)
            return;

        Advertisement.Banner.Hide();
    }

    public void ShowInGameAd()
    {
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
        yield return new WaitUntil(() => Advertisement.isShowing);
        IsShowingAd = false;
    }

    
}
