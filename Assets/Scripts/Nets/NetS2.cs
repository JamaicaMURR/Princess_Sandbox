using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using System;

public class NetS2 : Net
{
    Sink _leftSink, _rightSink;
    Vertex _redZoneVertex, _rightHalfVertex;

    public string redZoneDetectorKey = "Is_On_RedZone";
    public string rightHalfDetectorKey = "Is_On_RightHalf";

    public string redZoneVertexKey = "RedZone_Vertex";
    public string rightHalfVertexKey = "RightHalf_Vertex";

    public string leftSinkKey = "Left_Sink";
    public string rightSinkKey = "Right_Sink";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string redZoneBasisKey = "RedZone_Basis";
    public string rightHalfBasisKey = "RightHalf_Basis";

    private void Awake()
    {
        OppositeDiggerDispenser = new OneInstanceDispenser<Digger>(new OppositeFASFDigger(new Competent(), int.MaxValue));

        InitiateNet(netName: "S2");

        Sink leftSink = SpawnSink(leftSinkKey);
        Sink rightSink = SpawnSink(rightSinkKey);

        _redZoneVertex = SpawnVertex(redZoneVertexKey);
        _rightHalfVertex = SpawnVertex(rightHalfVertexKey);

        AddBasis(leftSink, leftBasisKey);
        AddBasis(rightSink, rightBasisKey);

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
    }

    private void Update()
    {
        Proceed();
    }
}
