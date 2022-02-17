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

    public float forceStep = 5;

    public float controlDelay = 0.125f;

    float _leftControlForce;
    float _rightControlForce;

    float _upControlForce;
    float _downControlForce;

    float _redZoneDesire;
    float _blueZoneDesire;

    private void Awake()
    {
        ConnectMainBus();

        mainBus.Add(new RawWrap(() => _leftControlForce), leftControllerKey);
        mainBus.Add(new RawWrap(() => _rightControlForce), rightControllerKey);

        mainBus.Add(new RawWrap(() => _upControlForce), upControllerKey);
        mainBus.Add(new RawWrap(() => _downControlForce), downControllerKey);

        mainBus.Add(new RawWrap(() => _redZoneDesire), redZoneDesireControllerKey);
        mainBus.Add(new RawWrap(() => _blueZoneDesire), blueZoneDesireControllerKey);
    }

    public void MoveLeft()
    {
        _leftControlForce = forceStep;
        _rightControlForce = -forceStep;
    }
    public void MoveRight()
    {
        _rightControlForce = forceStep;
        _leftControlForce = -forceStep;
    }
    public void MoveUp()
    {
        _upControlForce = forceStep;
        _downControlForce = -forceStep;
    }
    public void MoveDown()
    {
        _downControlForce = forceStep;
        _upControlForce = -forceStep;
    }
    public void ResetLRUD()
    {
        StartCoroutine(ResetAfterDelay());
    }

    public void IncreaseRedZoneDesire() => _redZoneDesire += forceStep;
    public void DecreaseRedZoneDesire() => _redZoneDesire -= forceStep;

    public void IncreaseBlueZoneDesire() => _blueZoneDesire += forceStep;
    public void DecreaseBlueZoneDesire() => _blueZoneDesire -= forceStep;

    IEnumerator ResetAfterDelay()
    {
        _leftControlForce = -forceStep;
        _rightControlForce = -forceStep;
        _upControlForce = -forceStep;
        _downControlForce = -forceStep;
        yield return new WaitForSeconds(controlDelay);
        _leftControlForce = default;
        _rightControlForce = default;
        _upControlForce = default;
        _downControlForce = default;
    }
}
