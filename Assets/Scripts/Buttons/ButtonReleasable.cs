using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ButtonReleasable : AdvancedButton, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Image _targetImage;

    Action SwithOnPointerUp;
    Action SwitchOnPointerEnter;
    Action SwitchOnPointerExit;

    public Sprite defaultSprite;
    public Sprite highlitedSprite;
    public Sprite pressedSprite;

    public AdvancedButton[] doublers;

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

        foreach(AdvancedButton button in doublers)
            if(button is IPointerEnterHandler)
                (button as IPointerEnterHandler).OnPointerEnter(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SwitchOnPointerExit();
        SwithOnPointerUp = () => _targetImage.sprite = defaultSprite;

        foreach(AdvancedButton button in doublers)
            if(button is IPointerExitHandler)
                (button as IPointerExitHandler).OnPointerExit(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetImage.sprite = pressedSprite;
        SwitchOnPointerEnter = () => { };
        SwitchOnPointerExit = () => { };
        OnClick?.Invoke();

        foreach(AdvancedButton button in doublers)
            if(button is IPointerDownHandler)
                (button as IPointerDownHandler).OnPointerDown(null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SwithOnPointerUp();
        SwitchOnPointerEnter = () => _targetImage.sprite = highlitedSprite;
        SwitchOnPointerExit = () => _targetImage.sprite = defaultSprite;
        OnRelease?.Invoke();

        foreach(AdvancedButton button in doublers)
            if(button is IPointerUpHandler)
                (button as IPointerUpHandler).OnPointerUp(null);
    }

    public override void Press() => OnPointerDown(null);
    public override void Release() => OnPointerUp(null);
}
