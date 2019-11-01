using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private FeedbackPopup feedbackPopup;

    [SerializeField] private RandomLevelsPopup randomPopup;

    [SerializeField] private RemoveAdsPopup removeAdsPopup;

    [SerializeField] private RestorePurchasesPopup restorePurchasesPopup;

    [SerializeField] private DataCollectionPopup dataCollectionPopup;

    [SerializeField] private CanvasGroup img_dim;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        img_dim.alpha = 0f;
        img_dim.blocksRaycasts = false;
    }

    public void ShowFeedbackPopup()
    {
        feedbackPopup.gameObject.SetActive(true);
        feedbackPopup.Init();
    }

    public void ShowRandomPopup(UnityAction callback)
    {
        randomPopup.gameObject.SetActive(true);
        randomPopup.Init(callback);
    }

    public void ShowRemoveAds(UnityAction callback)
    {
        removeAdsPopup.gameObject.SetActive(true);
        removeAdsPopup.Init(callback);
    }

    public void ShowRestorePurchases()
    {
        restorePurchasesPopup.gameObject.SetActive(true);
        restorePurchasesPopup.Init();
    }

    public void ShowDataCollectionPopup()
    {
        dataCollectionPopup.gameObject.SetActive(true);
        dataCollectionPopup.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
