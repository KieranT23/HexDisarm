using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private Button btn_close;

    [SerializeField] private Button btn_continue;

    [SerializeField] private CanvasGroup img_dim;

    //private void 

    public void Init(UnityAction closeAction = null, UnityAction continueAction = null)
    {
        btn_close.onClick.AddListener(closeAction);
        btn_close.onClick.AddListener(AnimateOut);
        btn_continue.onClick.AddListener(AnimateOut);
        btn_continue.onClick.AddListener(continueAction);
        AnimateIn();
    }

    public void AnimateIn()
    {
        transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(img_dim, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }

    public void AnimateOut()
    {

    }
}
