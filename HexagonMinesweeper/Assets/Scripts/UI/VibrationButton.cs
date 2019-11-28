using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationButton : SettingsButton
{
    #region Methods
    protected override void Start()
    {
        base.Start();
        isOn = PlayerInfoManager.Instance.VibrationOn;
        SetButtonState(isOn);
    }

    /// <summary>
    /// Toggle the currently active state of the button
    /// </summary>
    protected override void ToggleState()
    {
        base.ToggleState();
        PlayerInfoManager.Instance.VibrationOn = isOn;
        iOSHapticFeedback.Instance.ToggleVibration(isOn);
    }

    /// <summary>
    /// Initialise the vibration button
    /// </summary>
    public void Init()
    {
        isOn = PlayerInfoManager.Instance.VibrationOn;
        SetButtonState(isOn);
    }
    #endregion
}
