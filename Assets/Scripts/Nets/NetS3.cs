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

    //public string vertex1ActualDesireKey = "v1ad";
    //public string vertex2ActualDesireKey = "v2ad";

    public string dotXKey = "x";
    public string dotYKey = "y";

    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string upControllerKey = "uc";
    public string downControllerKey = "dc";

    public string redZoneDesireControllerKey = "rzdc";
    public string blueZoneDesireControllerKey = "bzdc";

    Node left, right, up, down;
    Vertex redZoneVertex, blueZoneVertex;
    Vertex[] verticals;
    Vertex[] horizontals;

    List<Vertex> allVertices;
    List<Node> allNodes;

    private void Awake()
    {
        ConnectMainBus();

        Alpha activator = new Alpha(0, 0.1f);

        left = new Node() { Activator = activator };
        right = new Node() { Activator = activator };
        up = new Node() { Activator = activator };
        down = new Node() { Activator = activator };

        Simple simple = new Simple();
        Morpheus morpheus = new Morpheus();
        Cannon cannon = new Cannon();

        redZoneVertex = GetRawVertex();
        blueZoneVertex = GetRawVertex();

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
            verticals[i].Hopper = new Heap();
            horizontals[i].Hopper = new Heap();
            mainBus.Add(verticals[i], verticalVerticesKeys[i]);
            mainBus.Add(horizontals[i], horizontalVerticesKeys[i]);
        }

        //mainBus.Add(new RawWrap(() => vertex1.RawDesire), vertex1ActualDesireKey);
        //mainBus.Add(new RawWrap(() => vertex2.RawDesire), vertex2ActualDesireKey);

        Vertex GetRawVertex()
        {
            return new Vertex()
            {
                Activator = simple,
                Sandman = morpheus,
                RMemory = new Plume(10),
                FMemory = new Plume(10),
                Caller = cannon
            };
        }
    }

    private void Start()
    {
        IRawProvider leftController = mainBus.Get<IRawProvider>(leftControllerKey);
        IRawProvider rightController = mainBus.Get<IRawProvider>(rightControllerKey);
        IRawProvider upController = mainBus.Get<IRawProvider>(upControllerKey);
        IRawProvider downController = mainBus.Get<IRawProvider>(downControllerKey);

        left.Hopper = new ControllableHeap(leftController.GetRaw);
        right.Hopper = new ControllableHeap(rightController.GetRaw);
        up.Hopper = new ControllableHeap(upController.GetRaw);
        down.Hopper = new ControllableHeap(downController.GetRaw);

        allNodes = new List<Node>() { left, right, up, down };

        IRawProvider redZoneDesireController = mainBus.Get<IRawProvider>(redZoneDesireControllerKey);
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);

        redZoneVertex.Hopper = new ControllableHeap(redZoneDesireController.GetRaw);
        redZoneVertex.SignalSource = redZoneDetector;

        IRawProvider blueZoneDesireController = mainBus.Get<IRawProvider>(blueZoneDesireControllerKey);
        ISignalSource blueZoneDetector = mainBus.Get<ISignalSource>(blueZoneDetectorKey);

        blueZoneVertex.Hopper = new ControllableHeap(blueZoneDesireController.GetRaw);
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
