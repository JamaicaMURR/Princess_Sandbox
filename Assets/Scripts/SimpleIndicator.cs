using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleIndicator : MonoBehaviour
{
    Image _image;
    Sprite _sprite;

    public bool Visible
    {
        set
        {
            if(value)
                _image.sprite = _sprite;
            else
                _image.sprite = null;
        }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _sprite = _image.sprite;
    }
}
