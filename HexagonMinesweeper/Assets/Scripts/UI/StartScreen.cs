using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_currentLevel;

    [SerializeField] private Button btn_continue;

    private void Start()
    {
        Init();
        btn_continue.onClick.AddListener(Continue);
    }

    public void Init()
    {
        txt_currentLevel.text = string.Format("Level\n<size=200>{0}</size>", PlayerInfoManager.Instance.LevelsUnlocked);
    }

    private void Continue()
    {
        gameObject.SetActive(false);
    }
}
