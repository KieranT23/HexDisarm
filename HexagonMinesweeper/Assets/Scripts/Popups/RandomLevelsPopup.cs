using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLevelsPopup : PopupBase
{
    #region Variables
    /// <summary>
    /// The continue button
    /// </summary>
    [SerializeField]
    private Button btn_yes;

    /// <summary>
    /// The button to close the popup
    /// </summary>
    [SerializeField]
    private Button btn_notYet;
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();
        btn_yes.onClick.AddListener(Yes);
        btn_notYet.onClick.AddListener(NotYet);
    }
    /// <summary>
    /// Start playing random levels
    /// </summary>
    private void Yes()
    {
        AnalyticsManager.Instance.LogRandomPopupAction(AnalyticsManager.RandomPopupAction.Yes);
        AnimateOut();
        successCallback?.Invoke();
    }

    /// <summary>
    /// Do not play a random level yet
    /// </summary>
    private void NotYet()
    {
        AnalyticsManager.Instance.LogRandomPopupAction(AnalyticsManager.RandomPopupAction.No);
        AnimateOut();
    }
    #endregion
}
