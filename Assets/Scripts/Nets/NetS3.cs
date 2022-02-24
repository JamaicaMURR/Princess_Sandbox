using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS3 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string blueZoneDetectorKey = "bzd";

    public string redZoneVertexKey = "rzv";
    public string blueZoneVertexKey = "bzv";

    public string[] verticalVerticesKeys = new string[] { "v1", "v2", "v3" };
    public string[] horizontalVerticesKeys = new string[] { "h1", "h2", "h3" };

    public string nodeLeftKey = "nodeLeft";
    public string nodeRightKey = "nodeRight";
    public string nodeUpKey = "nodeUp";
    public string nodeDownKey = "nodeDown";

    public string dotXKey = "x";
    public string dotYKey = "y";

    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string upControllerKey = "uc";
    public string downControllerKey = "dc";

    public string redZoneDesireControllerKey = "rzdc";
    public string blueZoneDesireControllerKey = "bzdc";

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

        Basis leftController = new Basis(left);
        Basis rightController = new Basis(right);
        Basis upController = new Basis(up);
        Basis downController = new Basis(down);
        Basis redZoneDesireController = new Basis(redZoneVertex);
        Basis blueZoneDesireController = new Basis(blueZoneVertex);

        mainBus.Add(leftController, leftControllerKey);
        mainBus.Add(rightController, rightControllerKey);
        mainBus.Add(upController, upControllerKey);
        mainBus.Add(downController, downControllerKey);
        mainBus.Add(redZoneDesireController, redZoneDesireControllerKey);
        mainBus.Add(blueZoneDesireController, blueZoneDesireControllerKey);

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

        Vertex GetRawVertex()
        {
            return new Vertex()
            {
                RMemory = new Plume(10),
                FMemory = new Plume(10),
                Sandman = morpheus,
                Caller = caller
            };
        }
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

        EdgeMaker edgeMaker = new HeavyEM();

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