using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class NodeSignalIndicator : MainBusUser
{
    SpriteRenderer _indicatorRenderer;
    SpriteRenderer _upArrowRendrer;
    SpriteRenderer _downArrowRenderer;

    Node _node;

    public string indicatorName = "Indicator";
    public string upArrowName = "UpArrow";
    public string downArrowName = "DownArrow";
    public string nodeBusKey;

    public Sprite indicatorSpriteOnTrue;
    public Sprite indicatorSpriteOnFalse;
    public Sprite upArrowIndication;
    public Sprite downArrowIndication;


    private void Awake()
    {
        ConnectMainBus();

        _indicatorRenderer = transform.Find(indicatorName).GetComponent<SpriteRenderer>();
        _upArrowRendrer = transform.Find(upArrowName).GetComponent<SpriteRenderer>();
        _downArrowRenderer = transform.Find(downArrowName).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _node = mainBus.Get<Node>(nodeBusKey);

        _node.OnRise += ChangeAtTrue;
        _node.OnFall += ChangeAtFalse;

        _node.OnTaskIsSetted += CheckIntension;
        _node.OnTaskIsFinished += CheckIntension;

        CheckIntension();
    }

    void ChangeAtTrue() => _indicatorRenderer.sprite = indicatorSpriteOnTrue;
    void ChangeAtFalse() => _indicatorRenderer.sprite = indicatorSpriteOnFalse;
    void CheckIntension()
    {
        int intension = _node.Intension;

        if(intension==Node.INTENSION_NEUTRAL)
        {
            _upArrowRendrer.sprite = null;
            _downArrowRenderer.sprite = null;
        }
        else if(intension==Node.INTENSION_RISE)
        {
            _upArrowRendrer.sprite = upArrowIndication;
            _downArrowRenderer.sprite = null;
        }
        else
        {
            _downArrowRenderer.sprite = downArrowIndication;
            _upArrowRendrer.sprite = null;
        }

    }
}
