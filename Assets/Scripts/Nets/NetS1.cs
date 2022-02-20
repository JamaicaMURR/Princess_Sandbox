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

        Alpha activator = new Alpha(0, 0.1f);

        left = new Node() { Activator = activator };
        right = new Node() { Activator = activator };

        vertex = new Vertex()
        {
            Activator = activator,
            Sandman = new Morpheus(),
            RMemory = new Plume(10),
            FMemory = new Plume(10),
            Caller = new Cannon()
        };

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

        EdgeMaker edgeMaker = new HeavyEM();

        vertex.Connect(left, edgeMaker);
        vertex.Connect(right, edgeMaker);
    }

    private void Update()
    {
        Proceed();
    }

    public void Sleep()
    {
        vertex.Sleep(Princess.EventType.Rise);
        vertex.Sleep(Princess.EventType.Fall);
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
