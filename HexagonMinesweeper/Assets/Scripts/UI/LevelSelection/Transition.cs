using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Transition : MonoBehaviour
{
    /// <summary>
    /// The background that is attached to this object
    /// </summary>
    private Image background;

    /// <summary>
    /// Initialise this object
    /// </summary>
    /// <param name="colorToSet">The colour to set this transition</param>
    public void Init(Color colorToSet)
    {
        background = GetComponent<Image>();
        background.color = colorToSet;
    }
}
