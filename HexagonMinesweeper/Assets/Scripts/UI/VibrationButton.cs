using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationButton : SettingsButton
{
    protected override void Start()
    {
        base.Start();
        isOn = PlayerInfoManager.Instance.VibrationOn;
        SetButtonState(isOn);
    }

    protected override void ToggleState()
    {
        base.ToggleState();
        PlayerInfoManager.Instance.VibrationOn = isOn;
    }

    public void Init()
    {
        isOn = PlayerInfoManager.Instance.VibrationOn;
        SetButtonState(isOn);
    }
}
