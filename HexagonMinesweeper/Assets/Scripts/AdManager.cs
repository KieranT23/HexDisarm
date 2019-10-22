using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    private void Start()
    {
        Advertisement.Initialize("3329503", true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Advertisement.IsReady("InGameBanner"))
            {
                Advertisement.Banner.Show("InGameBanner");
            }
        }
    }
}
