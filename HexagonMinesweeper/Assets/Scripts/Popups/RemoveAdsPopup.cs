using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsPopup : PopupBase
{
    #region Variables
    /// <summary>
    /// The button to remove ads
    /// </summary>
    [SerializeField]
    private Button btn_yes;

    /// <summary>
    /// The button to close the popup
    /// </summary>
    [SerializeField]
    private Button btn_cancel;
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();
        btn_yes.onClick.AddListener(Yes);
        btn_cancel.onClick.AddListener(Cancel);
    }

    /// <summary>
    /// Animate the popup out and invoke the success callback
    /// </summary>
    private void Yes()
    {
        AnimateOut();
        successCallback?.Invoke();
    }

    /// <summary>
    /// Close the popup
    /// </summary>
    private void Cancel()
    {
        AnimateOut();
    }
    #endregion
}
