using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLevelsPopup : PopupBase
{
    [SerializeField] private Button btn_yes;

    [SerializeField] private Button btn_notYet;

    protected override void Start()
    {
        base.Start();
        btn_yes.onClick.AddListener(Yes);
        btn_notYet.onClick.AddListener(NotYet);
    }

    private void Yes()
    {
        AnalyticsManager.Instance.LogRandomPopupAction(AnalyticsManager.RandomPopupAction.Yes);
        AnimateOut();
        successCallback?.Invoke();
    }

    private void NotYet()
    {
        AnalyticsManager.Instance.LogRandomPopupAction(AnalyticsManager.RandomPopupAction.No);
        AnimateOut();
    }
}
