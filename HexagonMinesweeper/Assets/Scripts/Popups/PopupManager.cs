using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private FeedbackPopup feedbackPopup;

    [SerializeField] private CanvasGroup img_dim;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        img_dim.alpha = 0f;
        img_dim.blocksRaycasts = false;
    }

    public void ShowFeedbackPopup()
    {
        feedbackPopup.gameObject.SetActive(true);
        feedbackPopup.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
