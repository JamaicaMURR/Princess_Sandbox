using Princess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorZone : MainBusUser
{
    bool _state;

    public string busKey;
    public string invaderTag;

    private void Awake()
    {
        ConnectMainBus();

        mainBus.Add(new SignalWrap(() => _state), busKey);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == invaderTag)
            _state = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == invaderTag)
            _state = false;;
    }
}
