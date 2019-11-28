using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public static ColourManager Instance;

    [SerializeField] private Color[] baseColours;

    [SerializeField] private Color[] pinkTheme;

    [SerializeField] private Color[] hotColdTheme;

    [SerializeField] private Color[] blueTheme;

    [SerializeField] private Color[] greenTheme;

    public int AmountOfColorThemes { get; private set; } = 5;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        
    }

    private void Start()
    {
        //SwitchTheme(PlayerInfoManager.Instance.CurrentColourTheme);
    }

    public void SwitchTheme(int colourTheme)
    {
        Color[] currentTheme = baseColours;
        switch (colourTheme)
        {
            case 0:
                currentTheme = baseColours;
                break;
            case 1:
                currentTheme = pinkTheme;
                break;
            case 2:
                currentTheme = hotColdTheme;
                break;
            case 3:
                currentTheme = blueTheme;
                break;
            case 4:
                currentTheme = greenTheme;
                break;
        }

        GridGenerator3D.Instance.SetTileColours(currentTheme);
        PlayerInfoManager.Instance.CurrentColourTheme = colourTheme;
    }

}
