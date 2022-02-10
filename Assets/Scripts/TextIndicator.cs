using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using UnityEngine.UI;

public class TextIndicator : MainBusUser
{
    IRawProvider _valueProvider;
    Text _text;

    public string valueKey;

    public float refreshingPeriod = 0.25f;

    private void Awake()
    {
        ConnectMainBus();

        _text = GetComponent<Text>();
    }

    private void Start()
    {
        _valueProvider = mainBus.Get<IRawProvider>(valueKey);
        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        while(true)
        {
            _text.text = _valueProvider.GetRaw().ToString();
            yield return new WaitForSeconds(refreshingPeriod);
        }
    }
}
