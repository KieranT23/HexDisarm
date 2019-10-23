using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButton : SettingsButton
{
    protected override void Start()
    {
        base.Start();
        isOn = PlayerInfoManager.Instance.AudioOn;
        SetButtonState(isOn);
    }

    protected override void ToggleState()
    {
        base.ToggleState();
        PlayerInfoManager.Instance.AudioOn = isOn;
    }

    public void Init()
    {
        isOn = PlayerInfoManager.Instance.AudioOn;
        SetButtonState(isOn);
    }
}
