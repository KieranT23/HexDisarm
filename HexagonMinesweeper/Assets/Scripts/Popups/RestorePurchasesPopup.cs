using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestorePurchasesPopup : PopupBase
{
    [SerializeField] private Button btn_okay;

    protected override void Start()
    {
        base.Start();
        btn_okay.onClick.AddListener(AnimateOut);
    }
}
