using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour
{
    [SerializeField] private Color selectedColor;

    [SerializeField] private Color selectedIconColor;

    [SerializeField] private Color notSelectedColor;

    [SerializeField] private Color notSelectedIconColor;

    [SerializeField] private Image img_backgroundImage;

    [SerializeField] private Image img_iconImage;

    [SerializeField] private Sprite onIcon;

    [SerializeField] private Sprite offIcon;

    private Button button;

    protected bool isOn;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleState);
    }

    protected void SetButtonState(bool on)
    {
        img_backgroundImage.color = on ? selectedColor : notSelectedColor;
        img_iconImage.color = on ? selectedIconColor : notSelectedIconColor;
        img_iconImage.sprite = on ? onIcon : offIcon;
    }

    protected virtual void ToggleState()
    {
        isOn = !isOn;
        SetButtonState(isOn);
    }
}
