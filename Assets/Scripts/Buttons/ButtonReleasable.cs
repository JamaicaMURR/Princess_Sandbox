using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ButtonReleasable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Image _targetImage;

    Action SwithOnPointerUp;
    Action SwitchOnPointerEnter;
    Action SwitchOnPointerExit;

    public Sprite defaultSprite;
    public Sprite highlitedSprite;
    public Sprite pressedSprite;

    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        SwitchOnPointerEnter = ()=> _targetImage.sprite = highlitedSprite;
        SwitchOnPointerExit = () => _targetImage.sprite = defaultSprite;
        SwithOnPointerUp = () => _targetImage.sprite = highlitedSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SwitchOnPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SwitchOnPointerExit();
        SwithOnPointerUp = () => _targetImage.sprite = defaultSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetImage.sprite = pressedSprite;
        SwitchOnPointerEnter = () => { };
        SwitchOnPointerExit = () => { };
        OnClick?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SwithOnPointerUp();
        SwitchOnPointerEnter = () => _targetImage.sprite = highlitedSprite;
        SwitchOnPointerExit = () => _targetImage.sprite = defaultSprite;
        OnRelease?.Invoke();
    }

    public void Press() => OnPointerDown(null);
    public void Release() => OnPointerUp(null);
}
