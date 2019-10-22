using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Transition : MonoBehaviour
{
    private Image background;

    public void Init(Color colorToSet)
    {
        background = GetComponent<Image>();
        background.color = colorToSet;
    }
}
