using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ColourButton : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The current colour selection
    /// </summary>
    private int currentColourSelection = 0;
    /// <summary>
    /// The button that is attached to this object
    /// </summary>
    private Button button;
    #endregion

    #region Methods
    private void Start()
    {
        button = GetComponent<Button>();
        currentColourSelection = PlayerInfoManager.Instance.CurrentColourTheme;
        SetTheme(currentColourSelection);
        button.onClick.AddListener(ToggleColours);
    }

    /// <summary>
    /// Initalise the colour button
    /// </summary>
    public void Init()
    {
        button = GetComponent<Button>();
        currentColourSelection = PlayerInfoManager.Instance.CurrentColourTheme;
        SetTheme(currentColourSelection);
        button.onClick.AddListener(ToggleColours);
    }

    /// <summary>
    /// Toggle the currently selected theme
    /// </summary>
    private void ToggleColours()
    {
        currentColourSelection++;
        if (currentColourSelection >= ColourManager.Instance.AmountOfColorThemes)
            currentColourSelection = 0;
        SetTheme(currentColourSelection);
    }

    /// <summary>
    /// Set the colour theme
    /// </summary>
    /// <param name="theme">The theme to set</param>
    private void SetTheme(int theme)
    {
        ColourManager.Instance.SwitchTheme(theme);
    }
    #endregion
}
