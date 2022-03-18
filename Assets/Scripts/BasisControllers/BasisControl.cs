using Princess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasisControl : MainBusUser
{
    int _weight;

    Basis _basis;
    Text _weightText;
    ButtonLockable _taskTrueButton, _taskFalseButton;

    public string basisKey;

    private void Awake()
    {
        ConnectMainBus();

        _taskTrueButton = transform.Find("Task_true").GetComponent<ButtonLockable>();
        _taskFalseButton = transform.Find("Task_false").GetComponent<ButtonLockable>();

        Button up_weight = transform.Find("Up_weight").GetComponent<Button>();
        Button down_weight = transform.Find("Down_weight").GetComponent<Button>();
        Button cancel = transform.Find("Cancel").GetComponent<Button>();

        _weightText = cancel.transform.Find("Text").GetComponent<Text>();
        Text header = transform.Find("Header").GetComponent<Text>();

        header.text = basisKey;

        _taskTrueButton.OnClick.AddListener(_taskFalseButton.Release);
        _taskTrueButton.OnClick.AddListener(() => InitiateTask(true));

        _taskFalseButton.OnClick.AddListener(_taskTrueButton.Release);
        _taskFalseButton.OnClick.AddListener(() => InitiateTask(false));

        cancel.onClick.AddListener(_taskTrueButton.Release);
        cancel.onClick.AddListener(_taskFalseButton.Release);
        cancel.onClick.AddListener(CancelTask);

        up_weight.onClick.AddListener(IncreaseWeight);
        down_weight.onClick.AddListener(DecreaseWeight);
    }

    private void Start()
    {
        _basis = mainBus.Get<Basis>(basisKey);
    }

    void IncreaseWeight()
    {
        _weight++;
        RefreshWeightText();
        ContinueTask();
    }

    void DecreaseWeight()
    {
        if(_weight > 0)
        {
            _weight--;
            RefreshWeightText();
            ContinueTask();
        }
    }

    void ContinueTask()
    {
        if(_taskTrueButton.IsOnLock)
            InitiateTask(true);
        else if(_taskFalseButton.IsOnLock)
            InitiateTask(false);
    }

    void InitiateTask(bool objective) => _basis.InitiateTask(objective, _weight);

    void CancelTask() => _basis.CancelTask();

    void RefreshWeightText() => _weightText.text = _weight.ToString();
}
