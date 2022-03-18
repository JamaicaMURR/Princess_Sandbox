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

    public string headerTextName = "Header";
    public string weightTextFieldName = "Weight";

    public string signalIsTrueIndicatorName = "IndicatorTrue";
    public string signalIsFalseIndicatorName = "IndicatorFalse";

    public string taskIsTrueIndicatorName = "TaskIsTrueIndicator";
    public string taskIsFalseIndicatorName = "TaskIsFalseIndicator";

    public string delegatedTaskIndicatorName = "TaskDelegatingIndicator";
    public string decisionNotFoundIndicatorName = "DecisionNotFoundIndicator";

    public string taskRejectionIndicatorName = "TaskRejectionIndicator";

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

            Transform taskDelegatingIndicator = transform.Find(delegatedTaskIndicatorName);
            Transform decisionNotFoundIndicator = transform.Find(decisionNotFoundIndicatorName);

            if(taskDelegatingIndicator != null)
            {
                // Capturing of local variable
                vertex.OnDelegatedTaskSet += () => taskDelegatingIndicator.gameObject.SetActive(true);
                vertex.OnDelegatedTaskFinish += () => taskDelegatingIndicator.gameObject.SetActive(false);
            }

            if(decisionNotFoundIndicator != null)
            {
                vertex.OnDelegatedTaskSet += () => decisionNotFoundIndicator.gameObject.SetActive(false);
                vertex.OnTaskIsFinished += () => decisionNotFoundIndicator.gameObject.SetActive(false);
                vertex.OnDecisionIsNotFound += () => decisionNotFoundIndicator.gameObject.SetActive(true);
            }

        }

        Transform taskRejectionIndicator = transform.Find(taskRejectionIndicatorName);

        if(taskRejectionIndicator != null)
        {
            _node.OnNewTaskRejected += () => taskRejectionIndicator.gameObject.SetActive(true);
            _node.OnTaskIsFinished += () => taskRejectionIndicator.gameObject.SetActive(false);
            _node.OnTaskIsSetted += () => taskRejectionIndicator.gameObject.SetActive(false);
        }

        _node.OnRise += ChangeAtTrue;
        _node.OnFall += ChangeAtFalse;

        _node.OnTaskIsSetted += IndicateIntension;
        _node.OnTaskIsFinished += HideIntension;
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

    void IndicateIntension()
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

        RefreshWeightText();
    }

    void HideIntension()
    {
        _taskIsTrueIndicator.SetActive(false);
        _taskIsFalseIndicator.SetActive(false);

        RefreshWeightText();
    }

    void ActualRefreshWeightText()
    {
        if(float.IsInfinity(_node.TaskWeight))
        {
            if(float.IsNegativeInfinity(_node.TaskWeight))
                _weightText.text = "---";
            else
                _weightText.text = "+++";
        }
        else
            _weightText.text = _node.TaskWeight.ToString();
    }
}
