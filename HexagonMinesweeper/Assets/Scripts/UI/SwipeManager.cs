using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// SwipeManager - found here: https://stackoverflow.com/questions/41491765/detect-swipe-gesture-direction
/// </summary>
public class SwipeManager : MonoBehaviour
{
    public static SwipeManager Instance;

    private float swipeThreshold = 50f;
    private float timeThreshold = 0.3f;

    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;

    private Vector2 fingerDown;
    private DateTime fingerDownTime;
    private Vector2 fingerUp;
    private DateTime fingerUpTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
    }

    private void Update()
    {
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
}