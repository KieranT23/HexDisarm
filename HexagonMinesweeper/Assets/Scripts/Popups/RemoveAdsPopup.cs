using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsPopup : PopupBase
{
    [SerializeField] private Button btn_yes;

    [SerializeField] private Button btn_cancel;

    protected override void Start()
    {
        base.Start();
        btn_yes.onClick.AddListener(Yes);
        btn_cancel.onClick.AddListener(Cancel);
    }

    private void Yes()
    {
        AnimateOut();
        successCallback?.Invoke();
    }

    private void Cancel()
    {
        AnimateOut();
    }
}
