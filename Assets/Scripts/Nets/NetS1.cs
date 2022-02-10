using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS1 : MainBusUser
{
    public string RedZoneDetectorKey = "rzd";
    public string VertexKey = "vertex1";
    public string NodeLeftKey = "nodeLeft";
    public string NodeRightKey = "nodeRight";

    public string LeftControllerKey = "lc";
    public string RightControllerKey = "rc";
    public string RedZoneDesireControllerKey = "rzdc";

    Node left, right;
    Vertex vertex;

    private void Awake()
    {
        ConnectMainBus();

        Alpha alpha = new Alpha(0, 0.1f);

        left = new Node() { Activator = alpha };
        right = new Node() { Activator = alpha };

        vertex = new Vertex() { Activator = alpha, Sandman = new Morpheus(), Predictor = new Haruspex() };

        mainBus.Add(left, NodeLeftKey);
        mainBus.Add(right, NodeRightKey);
        mainBus.Add(vertex, VertexKey);
    }

    private void Start()
    {
        IRawProvider leftController = mainBus.Get<IRawProvider>(LeftControllerKey);
        IRawProvider rightController = mainBus.Get<IRawProvider>(RightControllerKey);

        left.Hopper = new Controllable(new Heap(), leftController.GetRaw);
        right.Hopper = new Controllable(new Heap(), rightController.GetRaw);

        IRawProvider redZoneDesireController = mainBus.Get<IRawProvider>(RedZoneDesireControllerKey);
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(RedZoneDetectorKey);

        vertex.Hopper = new Controllable(new Heap(), redZoneDesireController.GetRaw);
        vertex.SignalSource = redZoneDetector;

        EdgeMaker edgeMaker = new HeavyMaker();

        vertex.Connect(left, edgeMaker);
        vertex.Connect(right, edgeMaker);
    }

    void Proceed()
    {
        left.Listen();
        right.Listen();
        vertex.Listen();

        vertex.Think();

        vertex.Call();
    }
}
