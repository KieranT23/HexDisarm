using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;

    private const string KEY_LEVELS_UNLOCKED = "levelsUnlocked";

    public int LevelsUnlocked
    {
        get { return PlayerPrefs.GetInt(KEY_LEVELS_UNLOCKED); }
        set { PlayerPrefs.SetInt(KEY_LEVELS_UNLOCKED, value);}
    }

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }

        if (!PlayerPrefs.HasKey(KEY_LEVELS_UNLOCKED))
            LevelsUnlocked = 1;

        LevelsUnlocked = 6;
    }
}
