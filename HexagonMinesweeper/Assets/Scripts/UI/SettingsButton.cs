using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Settings button base class
/// </summary>
[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The colour to set the background when the button is selected
    /// </summary>
    [SerializeField]
    private Color selectedColor;

    /// <summary>
    /// The colour to set the icon when the button is selected
    /// </summary>
    [SerializeField]
    private Color selectedIconColor;

    /// <summary>
    /// The colour to set the background when the button is not selected
    /// </summary>
    [SerializeField]
    private Color notSelectedColor;

    /// <summary>
    /// The colour to set the icon when the button is not selected
    /// </summary>
    [SerializeField]
    private Color notSelectedIconColor;

    /// <summary>
    /// The background image on the button
    /// </summary>
    [SerializeField]
    private Image img_backgroundImage;

    /// <summary>
    /// The icon image
    /// </summary>
    [SerializeField]
    private Image img_iconImage;

    /// <summary>
    /// The icon to use when the setting is active
    /// </summary>
    [SerializeField]
    private Sprite onIcon;

    /// <summary>
    /// The icon to use when the setting is inactive
    /// </summary>
    [SerializeField]
    private Sprite offIcon;

    /// <summary>
    /// The button that is attached to this objects
    /// </summary>
    private Button button;

    /// <summary>
    /// Check if the button setting is currently on
    /// </summary>
    protected bool isOn;
    #endregion
    #region Methods
    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleState);
    }

    /// <summary>
    /// Set the current button state
    /// </summary>
    /// <param name="on">Is the button setting on?</param>
    protected void SetButtonState(bool on)
    {
        img_backgroundImage.color = on ? selectedColor : notSelectedColor;
        img_iconImage.color = on ? selectedIconColor : notSelectedIconColor;
        img_iconImage.sprite = on ? onIcon : offIcon;
    }

    /// <summary>
    /// Toggle the currently active state of the button
    /// </summary>
    protected virtual void ToggleState()
    {
        isOn = !isOn;
        SetButtonState(isOn);
    }
    #endregion
}
