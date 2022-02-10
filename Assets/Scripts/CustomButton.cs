using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Image _targetImage;

    public Sprite defaultSprite;
    public Sprite highlitedSprite;
    public Sprite pressedSprite;

    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
    }

    private void OnMouseDown()
    {
        _targetImage.sprite = pressedSprite;
        OnClick?.Invoke();
    }

    private void OnMouseUp()
    {
        _targetImage.sprite = highlitedSprite;
        OnRelease?.Invoke();
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _targetImage.sprite = highlitedSprite;
        OnRelease?.Invoke();
    }
}
