using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedbackPopup : PopupBase
{
    #region Variables
    /// <summary>
    /// The feedback button
    /// </summary>
    [SerializeField]
    private Button btn_feedback;

    /// <summary>
    /// The review us button
    /// </summary>
    [SerializeField]
    private Button btn_reviewUs;

    /// <summary>
    /// The maybe later button
    /// </summary>
    [SerializeField]
    private Button btn_maybeLater;

    /// <summary>
    /// The icon that is used to indicate which store the user will be taken to
    /// </summary>
    [SerializeField]
    private Image storeIcon;

    /// <summary>
    /// The icon that will be used on android devices
    /// </summary>
    [SerializeField]
    private Sprite androidStore;

    /// <summary>
    /// The icon that will be used on iOS devices
    /// </summary>
    [SerializeField]
    private Sprite iosStore;
    #endregion

    #region Methods
    #region Unity
    protected override void Start()
    {
        base.Start();
        btn_feedback.onClick.AddListener(Feedback);
        btn_reviewUs.onClick.AddListener(Review);
        btn_maybeLater.onClick.AddListener(() =>
        {
            AnalyticsManager.Instance.LogFeedbackAction(AnalyticsManager.FeedbackAction.Closed);
            AnimateOut();
        });

#if UNITY_ANDROID
        storeIcon.sprite = androidStore;
#elif UNITY_IOS
        storeIcon.sprite = iosStore;
#endif
    }
    #endregion
    #region Private
    /// <summary>
    /// Open the feedback email and prefill some details about the device
    /// </summary>
    private void Feedback()
    {
        AnalyticsManager.Instance.LogFeedbackAction(AnalyticsManager.FeedbackAction.Emailed);
#if UNITY_ANDROID
        PlayerInfoManager.Instance.HasProvidedFeedback = true;
#endif
        string bodyFill = "Android version: " + SystemInfo.operatingSystem + Environment.NewLine +
                      "Device model: " + SystemInfo.deviceModel + Environment.NewLine +
                      "App Version: " + Application.version;

        string email = "hexdisarm@gmail.com";
        string subject = UnityWebRequest.EscapeURL("Hex Disarm feedback").Replace("+", "%20");
        string body = UnityWebRequest.EscapeURL(Environment.NewLine + Environment.NewLine + bodyFill.Replace("+", "%20"));
        Application.OpenURL(string.Format("mailto:{0}?subject={1}&body={2}", email, subject, body));
        AnimateOut();
    }
    /// <summary>
    /// Open the store page for this device to enable the user to review the application
    /// </summary>
    private void Review()
    {
        AnalyticsManager.Instance.LogFeedbackAction(AnalyticsManager.FeedbackAction.Reviewed);
#if UNITY_ANDROID
        PlayerInfoManager.Instance.HasProvidedFeedback = true;
        Application.OpenURL("https://play.google.com/store/apps/details?id=uk.KieranTownley.HexDisarm");
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/app/hex-disarm/id1485861396");
#endif

        AnimateOut();
    }
    #endregion
    #endregion
}
