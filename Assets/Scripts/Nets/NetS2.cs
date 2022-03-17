using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using System;

public class NetS2 : Net
{
    Sink _left, _right;
    Vertex _redZoneVertex, _rightHalfVertex;
    Exponential _cooler;

    float _coolingMultiplier = ControlledExponentialCoolersDispenser.DEFAULT_MULTIPLIER;

    public string redZoneDetectorKey = "Is_On_RedZone";
    public string rightHalfDetectorKey = "Is_On_RightHalf";
    public string redZoneVertexKey = "Vertex_RedZone";
    public string rightHalfVertexKey = "Vertex_RightHalf";
    public string leftSinkKey = "Go_Left";
    public string rightSinkKey = "Go_Right";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string redZoneBasisKey = "RedZone_Basis";
    public string rightHalfBasisKey = "RightHalf_Basis";

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
        InitiateNet(netName: "S2");

        _left = SpawnSink(leftSinkKey);
        _right = SpawnSink(rightSinkKey);

        _redZoneVertex = SpawnVertex(redZoneVertexKey);
        _rightHalfVertex = SpawnVertex(rightHalfVertexKey);

        AddBasis(_left, leftBasisKey);
        AddBasis(_right, rightBasisKey);
        AddBasis(_redZoneVertex, redZoneBasisKey);
        AddBasis(_rightHalfVertex, rightHalfBasisKey);

        // Assembling
        AssembleParagon();
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        _redZoneVertex.SignalSource = redZoneDetector;

        ISignalSource rightHalfDetector = mainBus.Get<ISignalSource>(rightHalfDetectorKey);
        _rightHalfVertex.SignalSource = rightHalfDetector;

        _cooler = mainBus.Get<Exponential>("S2_ICooler");
    }

    private void Update()
    {
        Proceed();
    }
}
