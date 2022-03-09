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
    public string delegatedTaskIndicatorName = "DelegatedTaskIndicator";
    public string taskFinishIndicatorName = "TaskFinishIndicator";
    public string callNotFoundIndicatorName = "TaskNotFoundIndicator";
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

        if(_node is Vertex)
        {
            Vertex vertex = _node as Vertex;

            Transform dti = transform.Find(delegatedTaskIndicatorName);
            Transform tfi = transform.Find(taskFinishIndicatorName);
            Transform cnfi = transform.Find(callNotFoundIndicatorName);

            if(dti != null)
            {
                // Capturing of local variable
                vertex.OnDelegatedTaskSet += () => dti.gameObject.SetActive(true);
                vertex.OnDelegatedTaskFinish += () => dti.gameObject.SetActive(false);
            }

            if(tfi != null)
            {
                vertex.OnDelegatedTaskSet += () => tfi.gameObject.SetActive(false);
                vertex.OnCallNotFound += () => tfi.gameObject.SetActive(false);
                vertex.OnTaskIsFinished += () => tfi.gameObject.SetActive(false);
                vertex.OnDelegatedTaskFinish += () => tfi.gameObject.SetActive(true);
            }

            if(cnfi != null)
            {
                vertex.OnDelegatedTaskSet += () => cnfi.gameObject.SetActive(false);
                vertex.OnTaskIsFinished += () => cnfi.gameObject.SetActive(false);
                vertex.OnCallNotFound += () => cnfi.gameObject.SetActive(true);
            }

        }

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
        if(_node.IsOnTask)
        {
            if(_node.Intension)
            {
                _taskIsTrueIndicator.SetActive(true);
                _taskIsFalseIndicator.SetActive(false);
            }
            else
            {
                _taskIsTrueIndicator.SetActive(false);
                _taskIsFalseIndicator.SetActive(true);
            }
        }
        else
        {
            _taskIsTrueIndicator.SetActive(false);
            _taskIsFalseIndicator.SetActive(false);
        }

        RefreshWeightText();
    }

    void ActualRefreshWeightText()
    {
        if(float.IsInfinity(_node.Weight))
        {
            if(float.IsNegativeInfinity(_node.Weight))
                _weightText.text = "---";
            else
                _weightText.text = "+++";
        }
        else
            _weightText.text = _node.Weight.ToString();
    }
}
