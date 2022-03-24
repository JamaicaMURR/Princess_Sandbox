using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS3 : Net
{
    Vertex _redZoneVertex, _rightHalfVertex;

    public string redZoneDetectorKey = "Is_On_RedZone";
    public string rightHalfDetectorKey = "Is_On_RightHalf";

    public string redZoneVertexKey = "RedZone_Vertex";
    public string rightHalfVertexKey = "RightHalf_Vertex";

    public string leftSinktKey = "Left_Sink";
    public string rightSinkKey = "Right_Sink";
    public string upSinkKey = "Up_Sink";
    public string downSinkKey = "Down_Sink";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string upBasisKey = "Up_Basis";
    public string downBasisKey = "Down_Basis";

    public string redZoneBasisKey = "RedZone_Basis";
    public string rightHalfBasisKey = "RightHalf_Basis";

    private void Awake()
    {
        //OppositeDiggerDispenser = new OneInstanceDispenser<Digger>(new OppositeFASFDigger(new Competent(), int.MaxValue));

        InitiateNet(netName: "S3");

        Sink left = SpawnSink(leftSinktKey);
        Sink right = SpawnSink(rightSinkKey);
        Sink up = SpawnSink(upSinkKey);
        Sink down = SpawnSink(downSinkKey);

        _redZoneVertex = SpawnVertex(redZoneVertexKey);
        _rightHalfVertex = SpawnVertex(rightHalfVertexKey);

        AddBasis(left, leftBasisKey);
        AddBasis(right, rightBasisKey);
        AddBasis(up, upBasisKey);
        AddBasis(down, downBasisKey);

        AddBasis(_redZoneVertex, redZoneBasisKey);
        AddBasis(_rightHalfVertex, rightHalfBasisKey);

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
