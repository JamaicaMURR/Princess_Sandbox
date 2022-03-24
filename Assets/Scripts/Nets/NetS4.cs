using Princess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetS4 : Net
{
    Vertex _redZoneVertex, _blueZoneVertex;

    public string redZoneDetectorKey = "Is_On_RedZone";
    public string blueZoneDetectorKey = "Is_On_BlueZone";

    public string redZoneVertexKey = "RedZone_Vertex";
    public string blueZoneVertexKey = "BlueZone_Vertex";

    public string leftSinktKey = "Left_Sink";
    public string rightSinkKey = "Right_Sink";
    public string upSinkKey = "Up_Sink";
    public string downSinkKey = "Down_Sink";

    public string leftBasisKey = "Left_Basis";
    public string rightBasisKey = "Right_Basis";
    public string upBasisKey = "Up_Basis";
    public string downBasisKey = "Down_Basis";

    public string redZoneBasisKey = "RedZone_Basis";
    public string blueZoneBasisKey = "BlueZone_Basis";

    private void Awake()
    {
        //OppositeDiggerDispenser = new OneInstanceDispenser<Digger>(new OppositeFASFDigger(new Competent(), int.MaxValue));

        InitiateNet(netName: "S4");

        Sink left = SpawnSink(leftSinktKey);
        Sink right = SpawnSink(rightSinkKey);
        Sink up = SpawnSink(upSinkKey);
        Sink down = SpawnSink(downSinkKey);

        _redZoneVertex = SpawnVertex(redZoneVertexKey);
        _blueZoneVertex = SpawnVertex(blueZoneVertexKey);

        AddBasis(left, leftBasisKey);
        AddBasis(right, rightBasisKey);
        AddBasis(up, upBasisKey);
        AddBasis(down, downBasisKey);

        AddBasis(_redZoneVertex, redZoneBasisKey);
        AddBasis(_blueZoneVertex, blueZoneBasisKey);

        AssembleParagon();
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        _redZoneVertex.SignalSource = redZoneDetector;

        ISignalSource blueZoneDetector = mainBus.Get<ISignalSource>(blueZoneDetectorKey);
        _blueZoneVertex.SignalSource = blueZoneDetector;
    }

    private void Update()
    {
        Proceed();
    }
}
