using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The base popup class that is used for all popups
/// </summary>
public class PopupBase : MonoBehaviour
{
    #region Variabless
    /// <summary>
    /// The dim image that is used to create the 'modal' effect with the popups
    /// </summary>
    [SerializeField]
    private CanvasGroup img_dim;

    protected UnityAction successCallback;
    #endregion
    #region Methods
    protected virtual void Start()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Initialise the current popup with a success callback
    /// </summary>
    /// <param name="callback">The callback to invoke when the popup has successfully completed</param>
    public void Init(UnityAction callback = null)
    {
        successCallback = callback;
        AnimateIn();
    }

    /// <summary>
    /// Animate the popup into view
    /// </summary>
    private void AnimateIn()
    {
        transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(img_dim, 1f, 0.15f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
        img_dim.blocksRaycasts = true;
    }

    /// <summary>
    /// Animate the popup out of view
    /// </summary>
    protected void AnimateOut()
    {
        LeanTween.alphaCanvas(img_dim, 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInSine);
        img_dim.blocksRaycasts = false;
        PopupManager.Instance.Hide();
    }
    #endregion
}
