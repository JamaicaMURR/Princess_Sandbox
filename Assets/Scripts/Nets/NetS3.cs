using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS3 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string blueZoneDetectorKey = "bzd";

    public string redZoneVertexKey = "Is_On_RedZone";
    public string blueZoneVertexKey = "Is_On_BlueZone";

    public string[] verticalVerticesKeys = new string[] { "y>-2", "y>0", "y>2" };
    public string[] horizontalVerticesKeys = new string[] { "x>-2", "x>0", "x>2" };

    public string nodeLeftKey = "Go_Left";
    public string nodeRightKey = "Go_Right";
    public string nodeUpKey = "Go_Up";
    public string nodeDownKey = "Go_Down";

    public string dotXKey = "x";
    public string dotYKey = "y";

    public string leftBasisKey = "Left";
    public string rightBasisKey = "Right";
    public string upBasisKey = "Up";
    public string downBasisKey = "Down";

    public string redZoneBasisKey = "RedZone_Basis";
    public string blueZoneBasisKey = "BlueZone_Basis";

    public string CallerKey = "Caller";

    Sink left, right, up, down;
    Vertex redZoneVertex, blueZoneVertex;
    Vertex[] verticals;
    Vertex[] horizontals;

    List<Vertex> allVertices;
    List<Node> allNodes;

    private void Awake()
    {
        ConnectMainBus();

        left = new Sink();
        right = new Sink();
        up = new Sink();
        down = new Sink();

        Morpheus morpheus = new Morpheus();
        ICaller caller = new Mortar();

        redZoneVertex = GetRawVertex();
        blueZoneVertex = GetRawVertex();

        Basis leftBasis = new Basis(left);
        Basis rightBasis = new Basis(right);
        Basis upBasis = new Basis(up);
        Basis downBasis = new Basis(down);
        Basis redZoneBasis = new Basis(redZoneVertex);
        Basis blueZoneBasis = new Basis(blueZoneVertex);

        mainBus.Add(leftBasis, leftBasisKey);
        mainBus.Add(rightBasis, rightBasisKey);
        mainBus.Add(upBasis, upBasisKey);
        mainBus.Add(downBasis, downBasisKey);
        mainBus.Add(redZoneBasis, redZoneBasisKey);
        mainBus.Add(blueZoneBasis, blueZoneBasisKey);

        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(up, nodeUpKey);
        mainBus.Add(down, nodeDownKey);

        mainBus.Add(redZoneVertex, redZoneVertexKey);
        mainBus.Add(blueZoneVertex, blueZoneVertexKey);

        verticals = new Vertex[3];
        horizontals = new Vertex[3];

        for(int i = 0; i < 3; i++)
        {
            verticals[i] = GetRawVertex();
            horizontals[i] = GetRawVertex();
            mainBus.Add(verticals[i], verticalVerticesKeys[i]);
            mainBus.Add(horizontals[i], horizontalVerticesKeys[i]);
        }

        mainBus.Add(caller, CallerKey);

        Vertex GetRawVertex() => new Vertex(new Plume(10), new Plume(10), morpheus, caller);
    }

    private void Start()
    {
        allNodes = new List<Node>() { left, right, up, down };

        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        redZoneVertex.SignalSource = redZoneDetector;

        ISignalSource blueZoneDetector = mainBus.Get<ISignalSource>(blueZoneDetectorKey);
        blueZoneVertex.SignalSource = blueZoneDetector;

        allVertices = new List<Vertex>() { redZoneVertex, blueZoneVertex };

        IRawProvider xProvider = mainBus.Get<IRawProvider>(dotXKey);
        IRawProvider yProvider = mainBus.Get<IRawProvider>(dotYKey);

        List<ISignalSource> horizontalSensors = SensorMaker.MakeLadder(xProvider.GetRaw, 4, -4, 4);
        List<ISignalSource> verticalSensors = SensorMaker.MakeLadder(yProvider.GetRaw, 4, -4, 4);

        for(int i = 0; i < 3; i++)
        {
            horizontals[2 - i].SignalSource = horizontalSensors[i];
            verticals[2 - i].SignalSource = verticalSensors[i];
            allVertices.Add(horizontals[i]);
            allVertices.Add(verticals[i]);
        }

        EdgeMaker<Edge> edgeMaker = new SinglePourerEdgeMaker<Edge, Dozer>();

        List<Vertex> connected = new List<Vertex>();

        // Makes Paragon vertexnet
        for(int i = 0; i < allVertices.Count; i++)
        {
            for(int j = 0; j < connected.Count; j++)
            {
                allVertices[i].Connect(connected[j], edgeMaker);
                connected[j].Connect(allVertices[i], edgeMaker);
            }

            connected.Add(allVertices[i]);

            for(int j = 0; j < allNodes.Count; j++)
                allVertices[i].Connect(allNodes[j], edgeMaker);
        }
    }

    private void Update()
    {
        Proceed();
    }

    public void Sleep()
    {
        allVertices.ForEach((x) => x.Sleep());
    }

    void Proceed()
    {
        allNodes.ForEach((x) => x.Listen());
        allVertices.ForEach((x) => x.Listen());
        allVertices.ForEach((x) => x.Think());
        allVertices.ForEach((x) => x.Call());
    }
}
