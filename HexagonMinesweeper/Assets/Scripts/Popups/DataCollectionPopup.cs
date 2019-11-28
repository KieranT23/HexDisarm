using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataCollectionPopup : PopupBase
{
    #region Variables
    /// <summary>
    /// The return button on the popup
    /// </summary>
    [SerializeField]
    private Button btn_return;

    /// <summary>
    /// The privacy policy button on the popup
    /// </summary>
    [SerializeField]
    private Button btn_privacyPolicy;

    /// <summary>
    /// The data collection toggle on the popup
    /// </summary>
    [SerializeField]
    private Toggle tgl_dataCollection;
    #endregion
    #region Methods
    #region Unity
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
    #endregion
    #region Private

    /// <summary>
    /// Open the privacy policy URL
    /// </summary>
    private void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://kierantownley.com/privacy-policy/");
    }

    /// <summary>
    /// Toggle data collection settings
    /// </summary>
    /// <param name="value">True if analytics are disabled</param>
    private void ToggleDataCollection(bool value)
    {
        PlayerInfoManager.Instance.HasDisabledAnalytics = value;
        AnalyticsManager.Instance.SetDisableAnalytics(value);
    }
    #endregion
    #endregion
}
