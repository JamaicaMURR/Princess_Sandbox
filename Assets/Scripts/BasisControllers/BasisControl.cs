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
    ButtonLockable _task_true, _task_false;

    public string basisKey;

    private void Awake()
    {
        ConnectMainBus();

        _task_true = transform.Find("Task_true").GetComponent<ButtonLockable>();
        _task_false = transform.Find("Task_false").GetComponent<ButtonLockable>();
        Button up_weight = transform.Find("Up_weight").GetComponent<Button>();
        Button down_weight = transform.Find("Down_weight").GetComponent<Button>();
        Button cancel = transform.Find("Cancel").GetComponent<Button>();
        _weightText = cancel.transform.Find("Text").GetComponent<Text>();
        Text header = transform.Find("Header").GetComponent<Text>();

        header.text = basisKey;

        _task_true.OnClick.AddListener(_task_false.Release);
        _task_true.OnClick.AddListener(() => InitiateTask(true));

        _task_false.OnClick.AddListener(_task_true.Release);
        _task_false.OnClick.AddListener(() => InitiateTask(false));

        cancel.onClick.AddListener(_task_true.Release);
        cancel.onClick.AddListener(_task_false.Release);
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

    void InitiateTask(bool objective) => _basis.InitiateTask(objective, _weight);

    void CancelTask() => _basis.CancelTask();

    void RefreshWeightText() => _weightText.text = _weight.ToString();

    void ContinueTask()
    {
        if(_task_true.IsOnLock)
            InitiateTask(true);
        else if(_task_false.IsOnLock)
            InitiateTask(false);
    }
}
