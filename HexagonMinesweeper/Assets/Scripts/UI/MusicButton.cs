using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicButton : SettingsButton
{
    protected override void Start()
    {
        base.Start();
        isOn = PlayerInfoManager.Instance.MusicOn;
        SetButtonState(isOn);
    }

    protected override void ToggleState()
    {
        base.ToggleState();
        PlayerInfoManager.Instance.MusicOn = isOn;
        AudioManager.Instance.ToggleMusic(isOn);
    }

    public void Init()
    {
        isOn = PlayerInfoManager.Instance.MusicOn;
        SetButtonState(isOn);
    }
}
