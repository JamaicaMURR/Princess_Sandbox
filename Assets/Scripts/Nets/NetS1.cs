using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS1 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string vertexKey = "vertex1";
    public string nodeLeftKey = "nodeLeft";
    public string nodeRightKey = "nodeRight";

    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string redZoneDesireControllerKey = "rzdc";

    Node left, right;
    Vertex vertex;

    private void Awake()
    {
        ConnectMainBus();

        Simple simple = new Simple();

        left = new Node() { Activator = simple };
        right = new Node() { Activator = simple };

        vertex = new Vertex() { Activator = simple, Sandman = new Morpheus(), Predictor = new Haruspex(), RMemory = new Plume(10), FMemory = new Plume(10) };

        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(vertex, vertexKey);
    }

    private void Start()
    {
        IRawProvider leftController = mainBus.Get<IRawProvider>(leftControllerKey);
        IRawProvider rightController = mainBus.Get<IRawProvider>(rightControllerKey);

        left.Hopper = new Controllable(new Heap(), leftController.GetRaw);
        right.Hopper = new Controllable(new Heap(), rightController.GetRaw);

        IRawProvider redZoneDesireController = mainBus.Get<IRawProvider>(redZoneDesireControllerKey);
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);

        vertex.Hopper = new Controllable(new Heap(), redZoneDesireController.GetRaw);
        vertex.SignalSource = redZoneDetector;

        EdgeMaker edgeMaker = new HeavyMaker();

        vertex.Connect(left, edgeMaker);
        vertex.Connect(right, edgeMaker);
    }

    private void FixedUpdate()
    {
        Proceed();
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
