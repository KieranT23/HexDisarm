using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicButton : SettingsButton
{
    #region Methods
    protected override void Start()
    {
        base.Start();
        isOn = PlayerInfoManager.Instance.MusicOn;
        SetButtonState(isOn);
    }
    /// <summary>
    /// Toggle the button state
    /// </summary>
    protected override void ToggleState()
    {
        base.ToggleState();
        PlayerInfoManager.Instance.MusicOn = isOn;
        AudioManager.Instance.ToggleMusic(isOn);
    }

    /// <summary>
    /// Initalise the button
    /// </summary>
    public void Init()
    {
        isOn = PlayerInfoManager.Instance.MusicOn;
        SetButtonState(isOn);
    }
    #endregion
}
