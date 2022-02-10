using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class InputHandler : MainBusUser
{
    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string redZoneDesireControllerKey = "rzdc";

    public float forceStep = 5;

    public float controlDelay = 0.125f;

    float _redZoneDesire;
    float _leftControlForce;
    float _rightControlForce;

    private void Awake()
    {
        ConnectMainBus();

        mainBus.Add(new RawWrap(() => _leftControlForce), leftControllerKey);
        mainBus.Add(new RawWrap(() => _rightControlForce), rightControllerKey);
        mainBus.Add(new RawWrap(() => _redZoneDesire), redZoneDesireControllerKey);
    }

    public void MoveLeft()
    {
        _leftControlForce = forceStep;
    }
    public void MoveRight()
    {
        _rightControlForce = forceStep;
    }
    public void ResetLR()
    {
        StartCoroutine(ResetAfterDelay());
    }

    public void IncreaseRedZoneDesire() => _redZoneDesire += forceStep;
    public void DecreaseRedZoneDesire() => _redZoneDesire -= forceStep;

    IEnumerator ResetAfterDelay()
    {
        _leftControlForce = -forceStep;
        _rightControlForce = -forceStep;
        yield return new WaitForSeconds(controlDelay);
        _leftControlForce = default;
        _rightControlForce = default;
    }
}
