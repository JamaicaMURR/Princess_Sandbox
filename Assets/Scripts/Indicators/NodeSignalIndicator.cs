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
    Text _weightText, _heatText;

    public string headerTextName = "Header";
    public string weightTextFieldName = "Weight";
    public string heatTextFialdName = "Heat";

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

        _weightText = transform.Find(weightTextFieldName).GetComponent<Text>();
        _heatText = transform.Find(heatTextFialdName).GetComponent<Text>();
    }

    private void Start()
    {
        _node = mainBus.Get<Node>(nodeBusKey);

        if(_node.Signal)
            ChangeAtTrue();
        else
            ChangeAtFalse();

        Transform taskRejectionIndicator = transform.Find(taskRejectionIndicatorName);

        if(taskRejectionIndicator != null)
        {
            _node.OnTaskRejected += () => taskRejectionIndicator.gameObject.SetActive(true);
            _node.OnTaskFinished += () => taskRejectionIndicator.gameObject.SetActive(false);
            _node.OnTaskSetted += () => taskRejectionIndicator.gameObject.SetActive(false);
        }

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
            else
                Debug.LogError("Task delegating indicator is not found");

            if(decisionNotFoundIndicator != null)
            {
                vertex.OnDelegatedTaskSet += () => decisionNotFoundIndicator.gameObject.SetActive(false);
                vertex.OnTaskFinished += () => decisionNotFoundIndicator.gameObject.SetActive(false);
                vertex.OnDecisionNotFound += () => decisionNotFoundIndicator.gameObject.SetActive(true);
            }
        }

        _node.OnRise += ChangeAtTrue;
        _node.OnFall += ChangeAtFalse;

        _node.OnTaskSetted += IndicateIntension;
        _node.OnTaskFinished += HideIntension;

        _node.OnRise += RefreshHeatText;
        _node.OnFall += RefreshHeatText;
        _node.OnHeatDrained += RefreshHeatText;

        RefreshHeatText();
        RefreshWeightText();
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

    void RefreshWeightText()
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

    void RefreshHeatText()
    {
        _heatText.text = _node.Heat.ToString("F3");
    }
}
