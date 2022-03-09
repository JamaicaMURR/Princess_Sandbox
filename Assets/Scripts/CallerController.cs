using Princess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerController : MainBusUser
{
    ControllableCaller<Cannon> _caller;
    Switcher _switcherStraight, _switcherInverted;

    public string callerBusKey = "Caller";
    public string switcherStraightName = "Straight";
    public string switcherInvertedName = "Inverted";

    private void Awake()
    {
        ConnectMainBus();
    }

    private void Start()
    {
        _caller = mainBus.Get<ControllableCaller<Cannon>>(callerBusKey);
        _switcherStraight = transform.Find(switcherStraightName).GetComponent<Switcher>();
        _switcherInverted = transform.Find(switcherInvertedName).GetComponent<Switcher>();

        _switcherStraight.IsUp = _caller.StraightCalling;
        _switcherInverted.IsUp = _caller.InvertedCalling;

        _switcherStraight.OnUp.AddListener(() => _caller.StraightCalling = true);
        _switcherStraight.OnDown.AddListener(() => _caller.StraightCalling = false);
        _switcherInverted.OnUp.AddListener(() => _caller.InvertedCalling = true);
        _switcherInverted.OnDown.AddListener(() => _caller.InvertedCalling = false);
    }
}
