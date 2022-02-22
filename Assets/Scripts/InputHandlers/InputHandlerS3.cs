using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class InputHandlerS3 : MainBusUser
{
    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";

    public string upControllerKey = "uc";
    public string downControllerKey = "dc";

    public string redZoneDesireControllerKey = "rzdc";
    public string blueZoneDesireControllerKey = "bzdc";

    public string redZoneDesireValueKey = "rzdv";
    public string blueZoneDesireValueKey = "bzdv";

    public float forceStep = 5;

    public float controlDelay = 0.125f;

    Basis leftController, rightController, upController, downController, redZoneDesireController, blueZoneDesireController;

    float _redZoneDesire;
    float _blueZoneDesire;

    private void Awake()
    {
        ConnectMainBus();

        mainBus.Add(new RawWrap(() => _redZoneDesire), redZoneDesireValueKey);
        mainBus.Add(new RawWrap(() => _blueZoneDesire), blueZoneDesireValueKey);
    }

    private void Start()
    {
        leftController = mainBus.Get<Basis>(leftControllerKey);
        rightController = mainBus.Get<Basis>(rightControllerKey);
        upController = mainBus.Get<Basis>(upControllerKey);
        downController = mainBus.Get<Basis>(downControllerKey);
        redZoneDesireController = mainBus.Get<Basis>(redZoneDesireControllerKey);
        blueZoneDesireController = mainBus.Get<Basis>(blueZoneDesireControllerKey);
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
    
    public void MoveUp()
    {
        upController.PushTask(forceStep);
        downController.PushTask(-forceStep);
    }
    public void MoveDown()
    {
        downController.PushTask(forceStep);
        upController.PushTask(-forceStep);
    }
    public void ResetLRUD()
    {
        StartCoroutine(ResetAfterDelay());
    }

    public void IncreaseRedZoneDesire()
    {
        _redZoneDesire += forceStep;
        redZoneDesireController.PushTask(_redZoneDesire);
    }
    public void DecreaseRedZoneDesire()
    {
        _redZoneDesire -= forceStep;
        redZoneDesireController.PushTask(_redZoneDesire);
    }
    public void IncreaseBlueZoneDesire()
    {
        _blueZoneDesire += forceStep;
        blueZoneDesireController.PushTask(_blueZoneDesire);
    }
    public void DecreaseBlueZoneDesire()
    {
        _blueZoneDesire -= forceStep;
        blueZoneDesireController.PushTask(_blueZoneDesire);
    }

    IEnumerator ResetAfterDelay()
    {
        leftController.PushTask(-forceStep);
        rightController.PushTask(-forceStep);
        upController.PushTask(-forceStep);
        downController.PushTask(-forceStep);
        yield return new WaitForSeconds(controlDelay);
        leftController.CancelTask();
        rightController.CancelTask();
        upController.CancelTask();
        downController.CancelTask();
    }
}
