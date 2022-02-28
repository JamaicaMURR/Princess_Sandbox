using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using UnityEngine.UI;
using System;

public class NodeSignalIndicator : MainBusUser
{
    GameObject _signalIsTrueIndicator, _signalIsFalseIndicator, _taskIsTrueIndicator, _taskIsFalseIndicator;
    Node _node;
    Text _weightText;

    Action RefreshWeightText;

    public string signalIsTrueIndicatorName = "IndicatorTrue";
    public string signalIsFalseIndicatorName = "IndicatorFalse";
    public string taskIsTrueIndicatorName = "TaskIsTrueIndicator";
    public string taskIsFalseIndicatorName = "TaskIsFalseIndicator";
    public string headerTextName = "Header";
    public string weightTextFieldName = "Weight";
    public string nodeBusKey;

    private void Awake()
    {
        ConnectMainBus();

        _signalIsTrueIndicator = transform.Find(signalIsTrueIndicatorName).gameObject;
        _signalIsFalseIndicator = transform.Find(signalIsFalseIndicatorName).gameObject;
        _taskIsTrueIndicator = transform.Find(taskIsTrueIndicatorName).gameObject;
        _taskIsFalseIndicator = transform.Find(taskIsFalseIndicatorName).gameObject;

        transform.Find(headerTextName).GetComponent<Text>().text = nodeBusKey;

        Transform t = transform.Find(weightTextFieldName);

        if(t != null)
        {
            _weightText = t.GetComponent<Text>();
            RefreshWeightText = ActualRefreshWeightText;
        }
        else
            RefreshWeightText = () => { };
    }

    private void Start()
    {
        _node = mainBus.Get<Node>(nodeBusKey);

        if(_node.Signal)
            ChangeAtTrue();
        else
            ChangeAtFalse();

        _node.OnRise += ChangeAtTrue;
        _node.OnFall += ChangeAtFalse;

        _node.OnTaskIsSetted += CheckIntension;
        _node.OnTaskIsFinished += CheckIntension;

        CheckIntension();
    }

    void ChangeAtTrue()
    {
        _signalIsTrueIndicator.SetActive(true);
        _signalIsFalseIndicator.SetActive(false);
    }
    void ChangeAtFalse()
    {
        _signalIsTrueIndicator.SetActive(false);
        _signalIsFalseIndicator.SetActive(true);
    }
    void CheckIntension()
    {
        int intension = _node.Intension;

        if(intension == Node.INTENSION_NEUTRAL)
        {
            _taskIsTrueIndicator.SetActive(false);
            _taskIsFalseIndicator.SetActive(false);
        }
        else if(intension == Node.INTENSION_RISE)
        {
            _taskIsTrueIndicator.SetActive(true);
            _taskIsFalseIndicator.SetActive(false);
        }
        else
        {
            _taskIsTrueIndicator.SetActive(false);
            _taskIsFalseIndicator.SetActive(true);
        }

        RefreshWeightText();
    }

    void ActualRefreshWeightText() => _weightText.text = _node.Weight.ToString();
}
