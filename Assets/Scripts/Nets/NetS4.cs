using Princess;
using Princess.ConnectionToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetS4 : Net
{
    Vertex _redZoneVertex, _blueZoneVertex;
    Vertex[] _xVertices, _yVertices;

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

    public string[] xKeys = new string[] { "x>-2", "x>0", "x>2" };
    public string[] yKeys = new string[] { "y>-2", "y>0", "y>2" };

    public string xKey = "x";
    public string yKey = "y";

    private void Awake()
    {
        OppositeDiggerDispenser = new OneInstanceDispenser<Digger>(new OppositeFASFDigger(new Competent(), int.MaxValue));

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

        _xVertices = SpawnVertices(xKeys);
        _yVertices = SpawnVertices(yKeys);

        AssembleParagon();

        Vertex[] SpawnVertices(string[] keys)
        {
            Vertex[] vertices = new Vertex[keys.Length];

            for(int i = 0; i < keys.Length; i++)
                vertices[i] = SpawnVertex(keys[i]);

            return vertices;
        }
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        _redZoneVertex.SignalSource = redZoneDetector;

        ISignalSource blueZoneDetector = mainBus.Get<ISignalSource>(blueZoneDetectorKey);
        _blueZoneVertex.SignalSource = blueZoneDetector;

        List<ISignalSource> xPosition = SensorMaker.MakeLadder(mainBus.Get<IRawProvider>(xKey).GetRaw, 4, -3.75f, 3.75f);
        ConnectSignalSources(_xVertices, xPosition);

        List<ISignalSource> yPosition = SensorMaker.MakeLadder(mainBus.Get<IRawProvider>(yKey).GetRaw, 4, -3.75f, 3.75f);
        ConnectSignalSources(_yVertices, yPosition);

        void ConnectSignalSources(Vertex[] vertices, List<ISignalSource> sources)
        {
            for(int i = 0; i < vertices.Length; i++)
                vertices[i].SignalSource = sources[sources.Count - 1 - i];
        }
    }

    private void Update()
    {
        Proceed();
    }
}
