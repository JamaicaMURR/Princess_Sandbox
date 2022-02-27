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
        //leftController.InitiateTask(forceStep);
        //rightController.InitiateTask(-forceStep);
    }
    public void MoveRight()
    {
        //rightController.InitiateTask(forceStep);
        //leftController.InitiateTask(-forceStep);
    }
    public void ResetLR()
    {
        StartCoroutine(ResetAfterDelay());
    }

    public void IncreaseRedZoneDesire()
    {
        redZoneDesire += forceStep;
        //redZoneDesireController.InitiateTask(redZoneDesire);
    }
    public void DecreaseRedZoneDesire()
    {
        redZoneDesire -= forceStep;
        //redZoneDesireController.InitiateTask(redZoneDesire);
    }

    public void IncreaseRightHalfDesire()
    {
        rightHalfDesire += forceStep;
        //rightHalfDesireController.InitiateTask(rightHalfDesire);
    }

    public void DecreaseRightHalfDesire()
    {
        rightHalfDesire -= forceStep;
        //rightHalfDesireController.InitiateTask(rightHalfDesire);
    }

    IEnumerator ResetAfterDelay()
    {
        //leftController.InitiateTask(-forceStep);
        //rightController.InitiateTask(-forceStep);
        yield return new WaitForSeconds(controlDelay);
        leftController.CancelTask();
        rightController.CancelTask();
    }
}
