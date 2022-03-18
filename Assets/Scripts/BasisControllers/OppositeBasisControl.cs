using Princess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OppositeBasisControl : MainBusUser
{
    public string basis1Key, basis2Key;

    public string arrow1Name = "Arrow1";
    public string arrow2Name = "Arrow2";

    public float force = float.PositiveInfinity;
    public float stopDelay = 0.1f;

    Basis _basis1, _basis2;

    private void Awake()
    {
        ConnectMainBus();

        ButtonReleasable arrow1 = transform.Find(arrow1Name).GetComponent<ButtonReleasable>();
        ButtonReleasable arrow2 = transform.Find(arrow2Name).GetComponent<ButtonReleasable>();

        arrow1.OnClick.AddListener(() => Shift(_basis1, _basis2));
        arrow1.OnRelease.AddListener(StopBoth);

        arrow2.OnClick.AddListener(() => Shift(_basis2, _basis1));
        arrow2.OnRelease.AddListener(StopBoth);
    }

    private void Start()
    {
        _basis1 = mainBus.Get<Basis>(basis1Key);
        _basis2 = mainBus.Get<Basis>(basis2Key);
    }

    void Shift(Basis active, Basis unactive)
    {
        active.InitiateTask(true, force);
        unactive.InitiateTask(false, force);
    }

    void StopBoth() => StartCoroutine(StopAndCancel());

    IEnumerator StopAndCancel()
    {
        _basis1.InitiateTask(false, force);
        _basis2.InitiateTask(false, force);
        yield return new WaitForSeconds(stopDelay);
        _basis1.CancelTask();
        _basis2.CancelTask();
    }
}
