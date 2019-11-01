using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataCollectionPopup : PopupBase
{
    [SerializeField] private Button btn_return;

    [SerializeField] private Button btn_privacyPolicy;

    [SerializeField] private Toggle tgl_dataCollection;

    protected override void Start()
    {
        base.Start();
        btn_return.onClick.AddListener(AnimateOut);
        btn_privacyPolicy.onClick.AddListener(OpenPrivacyPolicy);
        tgl_dataCollection.onValueChanged.AddListener((value) => ToggleDataCollection(!value));
    }

    private void OnEnable()
    {
        if (PlayerInfoManager.Instance == null)
            return;
        tgl_dataCollection.isOn = !PlayerInfoManager.Instance.HasDisabledAnalytics;
        ToggleDataCollection(PlayerInfoManager.Instance.HasDisabledAnalytics);
    }

    private void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://kierantownley.com/privacy-policy/");
    }

    private void ToggleDataCollection(bool value)
    {
        PlayerInfoManager.Instance.HasDisabledAnalytics = value;
        AnalyticsManager.Instance.SetDisableAnalytics(value);
    }
}
