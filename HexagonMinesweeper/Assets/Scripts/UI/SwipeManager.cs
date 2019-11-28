using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class SwipeManager : MonoBehaviour
{
    #region Variables
    #region Static
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static SwipeManager Instance;
    #endregion
    #region Public
    /// <summary>
    /// The event to invoke when a swipe left has occured
    /// </summary>
    public UnityEvent OnSwipeLeft;
    /// <summary>
    /// The event to invoke when a swipe right has occured
    /// </summary>
    public UnityEvent OnSwipeRight;
    /// <summary>
    /// The event to invoke when a swipe up has occured
    /// </summary>
    public UnityEvent OnSwipeUp;
    /// <summary>
    /// The event to invoke when a swipe down has occured
    /// </summary>
    public UnityEvent OnSwipeDown;
    #endregion
    #region Private
    /// <summary>
    /// The threshold in which a swipe is registered
    /// </summary>
    private float swipeThreshold = 50f;
    /// <summary>
    /// The amount of time the user must be swiping for before it is registered
    /// </summary>
    private float timeThreshold = 0.3f;
    /// <summary>
    /// The finger down position
    /// </summary>
    private Vector2 fingerDown;
    /// <summary>
    /// The time in which a finger went down
    /// </summary>
    private DateTime fingerDownTime;
    /// <summary>
    /// The finger up position
    /// </summary>
    private Vector2 fingerUp;
    /// <summary>
    /// The finger up time
    /// </summary>
    private DateTime fingerUpTime;
    #endregion
    #endregion

    #region Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        //Make sure the swipe manager is only used on the level selection
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //Check whether a swipe is occuring
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            fingerDown = Input.mousePosition;
            fingerUp = Input.mousePosition;
            fingerDownTime = DateTime.Now;
        }
        if (Input.GetMouseButtonUp(0))
        {
            fingerDown = Input.mousePosition;
            fingerUpTime = DateTime.Now;
            CheckSwipe();
        }
#elif UNITY_IOS || UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerDown = touch.position;
                fingerUp = touch.position;
                fingerDownTime = DateTime.Now;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                fingerUpTime = DateTime.Now;
                CheckSwipe();
            }
        }
#endif
    }
    
    /// <summary>
    /// Check which directing the user is swiping
    /// </summary>
    private void CheckSwipe()
    {
        float duration = (float)fingerUpTime.Subtract(fingerDownTime).TotalSeconds;

        if (duration > timeThreshold)
            return;

        float deltaX = fingerDown.x - fingerUp.x;
        if (Mathf.Abs(deltaX) > swipeThreshold)
        {
            if (deltaX > 0)
            {
                OnSwipeRight.Invoke();
            }
            else if (deltaX < 0)
            {
                OnSwipeLeft.Invoke();
            }
        }

        float deltaY = fingerDown.y - fingerUp.y;
        if (Mathf.Abs(deltaY) > swipeThreshold)
        {
            if (deltaY > 0)
            {
                OnSwipeUp.Invoke();
            }
            else if (deltaY < 0)
            {
                OnSwipeDown.Invoke();
            }
        }

        fingerUp = fingerDown;
    }
    #endregion
}