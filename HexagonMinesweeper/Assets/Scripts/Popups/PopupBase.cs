using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private CanvasGroup img_dim;

    protected UnityAction successCallback;
    //private void 

    protected virtual void Start()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Init(UnityAction callback = null)
    {
        successCallback = callback;
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
        LeanTween.alphaCanvas(img_dim, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
    }
}
