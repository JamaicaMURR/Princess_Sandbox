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

        for(int i=0; i<doublers.Length; i++)
            if(doublers[i] is IPointerEnterHandler)
                (doublers[i] as IPointerEnterHandler).OnPointerEnter(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SwitchOnPointerExit();
        SwithOnPointerUp = () => _targetImage.sprite = defaultSprite;

        for(int i = 0; i < doublers.Length; i++)
            if(doublers[i] is IPointerExitHandler)
                (doublers[i] as IPointerExitHandler).OnPointerExit(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetImage.sprite = pressedSprite;
        SwitchOnPointerEnter = () => { };
        SwitchOnPointerExit = () => { };
        OnClick?.Invoke();

        for(int i = 0; i < doublers.Length; i++)
            if(doublers[i] is IPointerDownHandler)
                (doublers[i] as IPointerDownHandler).OnPointerDown(null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SwithOnPointerUp();
        SwitchOnPointerEnter = () => _targetImage.sprite = highlitedSprite;
        SwitchOnPointerExit = () => _targetImage.sprite = defaultSprite;
        OnRelease?.Invoke();

        for(int i = 0; i < doublers.Length; i++)
            if(doublers[i] is IPointerUpHandler)
                (doublers[i] as IPointerUpHandler).OnPointerUp(null);
    }

    public override void Press() => OnPointerDown(null);
    public override void Release() => OnPointerUp(null);
}
