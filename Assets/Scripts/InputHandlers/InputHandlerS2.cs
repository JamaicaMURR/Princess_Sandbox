using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class InputHandlerS2 : MainBusUser
{
    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string redZoneDesireControllerKey = "rzdc";
    public string rightHalfDesireControllerKey = "rhdc";

    public string redZoneDesireValueKey = "rzdv";
    public string rightHalfDesireValueKey = "rhdv";

    public float forceStep = 5;

    public float controlDelay = 0.125f;

    Basis leftController, rightController, redZoneDesireController, rightHalfDesireController;

    float redZoneDesire, rightHalfDesire;

    private void Awake()
    {
        ConnectMainBus();

        mainBus.Add(new RawWrap(() => redZoneDesire), redZoneDesireValueKey);
        mainBus.Add(new RawWrap(() => rightHalfDesire), rightHalfDesireValueKey);
    }

    private void Start()
    {
        leftController = mainBus.Get<Basis>(leftControllerKey);
        rightController = mainBus.Get<Basis>(rightControllerKey);
        redZoneDesireController = mainBus.Get<Basis>(redZoneDesireControllerKey);
        rightHalfDesireController = mainBus.Get<Basis>(rightHalfDesireControllerKey);
    }

    public void MoveLeft()
    {
        leftController.PushTask(forceStep);
        rightController.PushTask(-forceStep);
    }
    public void MoveRight()
    {
        rightController.PushTask(forceStep);
        leftController.PushTask(-forceStep);
    }
    public void ResetLR()
    {
        StartCoroutine(ResetAfterDelay());
    }

    public void IncreaseRedZoneDesire()
    {
        redZoneDesire += forceStep;
        redZoneDesireController.PushTask(redZoneDesire);
    }
    public void DecreaseRedZoneDesire()
    {
        redZoneDesire -= forceStep;
        redZoneDesireController.PushTask(redZoneDesire);
    }

    public void IncreaseRightHalfDesire()
    {
        rightHalfDesire += forceStep;
        rightHalfDesireController.PushTask(rightHalfDesire);
    }

    public void DecreaseRightHalfDesire()
    {
        rightHalfDesire -= forceStep;
        rightHalfDesireController.PushTask(rightHalfDesire);
    }

    IEnumerator ResetAfterDelay()
    {
        leftController.PushTask(-forceStep);
        rightController.PushTask(-forceStep);
        yield return new WaitForSeconds(controlDelay);
        leftController.CancelTask();
        rightController.CancelTask();
    }
}
