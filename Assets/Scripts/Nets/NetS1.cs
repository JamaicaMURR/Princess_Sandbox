using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;
using System;

public class NetS1 : Net
{
    Sink _left, _right;
    Vertex _vertex;
    Exponential _cooler;

    float _coolingMultiplier = ControlledExponentialCoolersDispenser.DEFAULT_MULTIPLIER;

    public string redZoneDetectorKey = "Is_On_RedZone";

    public string vertexKey = "Vertex_RedZone";
    public string leftSinkKey = "Go_Left";
    public string rightSinkKey = "Go_Right";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string redZoneBasisKey = "RedZone_Basis";

    public float CoolingMultiplier 
    { 
        get => _coolingMultiplier; 
        set
        {
            try
            {
                _cooler.Multiplier = value;
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void Awake()
    {
        InitiateNet(netName: "S1");

        _left = SpawnSink(leftSinkKey, true);
        _right = SpawnSink(rightSinkKey, true);

        _vertex = SpawnVertex(vertexKey, true);

        // Assembling
        AssembleParagon();
    }

    private void Start()
    {
        _vertex.SignalSource = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        _cooler = mainBus.Get<Exponential>("S1_Cooler");
    }

    private void Update()
    {
        Proceed();
    }
}
