using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static ColourManager Instance;

    /// <summary>
    /// The base colours that are used
    /// </summary>
    [SerializeField]
    private Color[] baseColours;
    /// <summary>
    /// The pink theme
    /// </summary>
    [SerializeField]
    private Color[] pinkTheme;
    /// <summary>
    /// The hot cold them
    /// </summary>
    [SerializeField]
    private Color[] hotColdTheme;
    /// <summary>
    /// The blue theme
    /// </summary>
    [SerializeField]
    private Color[] blueTheme;
    /// <summary>
    /// The green theme
    /// </summary>
    [SerializeField]
    private Color[] greenTheme;

    /// <summary>
    /// The total amout of colour themes
    /// </summary>
    public int AmountOfColorThemes { get; private set; } = 5;
    #endregion

    #region Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        
    }
    /// <summary>
    /// Switch colour theme
    /// </summary>
    /// <param name="colourTheme">The theme to switch to</param>
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
    #endregion
}
