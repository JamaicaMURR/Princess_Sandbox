using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class NodeSignalIndicator : MainBusUser
{
    SpriteRenderer _indicatorRenderer;

    public string indicatorName = "Indicator";
    public string nodeBusKey;

    public Sprite indicatorSpriteOnTrue;
    public Sprite indicatorSpriteOnFalse;


    private void Awake()
    {
        ConnectMainBus();

        _indicatorRenderer = transform.Find(indicatorName).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Node node;

        node = mainBus.Get<Node>(nodeBusKey);

        node.OnRise += ChangeAtTrue;
        node.OnFall += ChangeAtFalse;
    }

    void ChangeAtTrue() => _indicatorRenderer.sprite = indicatorSpriteOnTrue;
    void ChangeAtFalse() => _indicatorRenderer.sprite = indicatorSpriteOnFalse;
}
