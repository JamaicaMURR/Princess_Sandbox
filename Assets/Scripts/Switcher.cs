using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Switcher : MonoBehaviour, IPointerDownHandler
{
    Animator _animator;
    Action DoOnPress;

    public string upTriggerName = "Up";
    public string downTriggerName = "Down";

    public UnityEvent OnUp;
    public UnityEvent OnDown;

    public bool State
    {
        get => DoOnPress == GoDown;
        set
        {
            if(value && !State)
                GoUp();
            else if(!value && State)
                GoDown();
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        DoOnPress = GoUp;
    }

    public void OnPointerDown(PointerEventData eventData) => DoOnPress();  

    void GoUp()
    {
        _animator.SetTrigger(upTriggerName);
        OnUp?.Invoke();
        DoOnPress = GoDown;
    }

    void GoDown()
    {
        _animator.SetTrigger(downTriggerName);
        OnDown?.Invoke();
        DoOnPress = GoUp;
    }
}
