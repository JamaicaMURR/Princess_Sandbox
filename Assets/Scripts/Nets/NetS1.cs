using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;
using System;

public class NetS1 : Net
{
    Vertex _vertex;

    public string redZoneDetectorKey = "Is_On_RedZone";

    public string vertexKey = "RedZone_Vertex";
    public string leftSinkKey = "Left_Sink";
    public string rightSinkKey = "Right_Sink";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string redZoneBasisKey = "RedZone_Basis";

    private void Awake()
    {
        InitiateNet(netName: "S1");

        Sink left = SpawnSink(leftSinkKey);
        Sink right = SpawnSink(rightSinkKey);

        _vertex = SpawnVertex(vertexKey);

        AddBasis(left, leftBasisKey);
        AddBasis(right, rightBasisKey);
        AddBasis(_vertex, redZoneBasisKey);

        // Assembling
        AssembleParagon();
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        _vertex.SignalSource = redZoneDetector;
    }

    private void Update()
    {
        Proceed();
    }
}
