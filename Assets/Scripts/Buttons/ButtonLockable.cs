using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonLockable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Image _targetImage;

    Action DoOnPointerEnter;
    Action DoOnPointerExit;
    Action DoOnPointerDown;

    public Sprite defaultSprite;
    public Sprite highlitedSprite;
    public Sprite pressedSprite;

    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    public bool IsOnLock 
    {
        get => DoOnPointerDown != ActualLock;
        private set
        {
            if(!value)
            {
                DoOnPointerEnter = SwitchToHLSprite;
                DoOnPointerExit = SwitchToDefaultSprite;
                DoOnPointerDown = ActualLock;
            }
            else
            {
                DoOnPointerEnter = () => { };
                DoOnPointerExit = () => { };
                DoOnPointerDown = () => { };
            }
        }
    }

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        IsOnLock = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => DoOnPointerEnter();

    public void OnPointerExit(PointerEventData eventData) => DoOnPointerExit();

    public void OnPointerDown(PointerEventData eventData) => DoOnPointerDown();

    public void Release()
    {
        SwitchToDefaultSprite();
        
        IsOnLock = false;
        OnRelease?.Invoke();
    }

    void SwitchToHLSprite() => _targetImage.sprite = highlitedSprite;
    void SwitchToDefaultSprite() => _targetImage.sprite = defaultSprite;
    void ActualLock()
    {
        _targetImage.sprite = pressedSprite;
        IsOnLock = true;
        OnClick?.Invoke();
    }
}
