using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedbackPopup : PopupBase
{
    [SerializeField] private Button btn_feedback;

    [SerializeField] private Button btn_reviewUs;

    [SerializeField] private Button btn_maybeLater;

    [SerializeField] private Image storeIcon;

    [SerializeField] private Sprite androidStore;

    [SerializeField] private Sprite iosStore;

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
}
