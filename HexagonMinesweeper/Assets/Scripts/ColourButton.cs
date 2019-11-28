using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ColourButton : MonoBehaviour
{
    private int currentColourSelection = 0;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        currentColourSelection = PlayerInfoManager.Instance.CurrentColourTheme;
        SetTheme(currentColourSelection);
        button.onClick.AddListener(ToggleColours);
    }

    public void Init()
    {
        button = GetComponent<Button>();
        currentColourSelection = PlayerInfoManager.Instance.CurrentColourTheme;
        SetTheme(currentColourSelection);
        button.onClick.AddListener(ToggleColours);
    }

    private void ToggleColours()
    {
        currentColourSelection++;
        if (currentColourSelection >= ColourManager.Instance.AmountOfColorThemes)
            currentColourSelection = 0;
        SetTheme(currentColourSelection);
    }

    private void SetTheme(int theme)
    {
        ColourManager.Instance.SwitchTheme(theme);
    }
}
