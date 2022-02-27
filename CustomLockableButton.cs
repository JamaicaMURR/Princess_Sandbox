using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomLockableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Image _targetImage;

    public Sprite defaultSprite;
    public Sprite highlitedSprite;
    public Sprite pressedSprite;

    public CustomLockableButton[] counterButtons;

    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetImage.sprite = highlitedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetImage.sprite = defaultSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetImage.sprite = pressedSprite;
        OnClick?.Invoke();

        foreach(CustomLockableButton button in counterButtons)
            button.Release();
    }

    public void Release()
    {
        _targetImage.sprite = defaultSprite;
    }
}
