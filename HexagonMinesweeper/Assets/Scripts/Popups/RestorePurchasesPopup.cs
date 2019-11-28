using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestorePurchasesPopup : PopupBase
{
    #region Variables
    /// <summary>
    /// The button to close the popup
    /// </summary>
    [SerializeField]
    private Button btn_okay;
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();
        btn_okay.onClick.AddListener(AnimateOut);
    }
    #endregion
}
