using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class NetS2 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string rightHalfDetectorKey = "rhd";
    public string vertex1Key = "vertex1";
    public string vertex2Key = "vertex2";
    public string nodeLeftKey = "nodeLeft";
    public string nodeRightKey = "nodeRight";

    public string leftControllerKey = "lc";
    public string rightControllerKey = "rc";
    public string redZoneDesireControllerKey = "rzdc";

    Node left, right;
    Vertex vertex1, vertex2;

    private void Awake()
    {
        ConnectMainBus();

        Alpha activator = new Alpha(0, 0.1f);

        left = new Node() { Activator = activator };
        right = new Node() { Activator = activator };

        vertex1 = new Vertex() { Activator = activator, Sandman = new Morpheus(), Predictor = new Haruspex(), RMemory = new Plume(10), FMemory = new Plume(10) };
        vertex2 = new Vertex() { Activator = activator, Hopper = new Heap(), Sandman = new Morpheus(), Predictor = new Haruspex(), RMemory = new Plume(10), FMemory = new Plume(10) };

        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(vertex1, vertex1Key);
        mainBus.Add(vertex2, vertex2Key);
    }

    private void Start()
    {
        IRawProvider leftController = mainBus.Get<IRawProvider>(leftControllerKey);
        IRawProvider rightController = mainBus.Get<IRawProvider>(rightControllerKey);

        left.Hopper = new Controllable(new Heap(), leftController.GetRaw);
        right.Hopper = new Controllable(new Heap(), rightController.GetRaw);

        IRawProvider redZoneDesireController = mainBus.Get<IRawProvider>(redZoneDesireControllerKey);
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);

        vertex1.Hopper = new Controllable(new Heap(), redZoneDesireController.GetRaw);
        vertex1.SignalSource = redZoneDetector;

        ISignalSource rightHalfDetector = mainBus.Get<ISignalSource>(rightHalfDetectorKey);

        vertex2.SignalSource = rightHalfDetector;

        EdgeMaker edgeMaker = new HeavyMaker();

        vertex1.Connect(left, edgeMaker);
        vertex1.Connect(right, edgeMaker);
        vertex1.Connect(vertex2, edgeMaker);

        vertex2.Connect(left, edgeMaker);
        vertex2.Connect(right, edgeMaker);
        vertex2.Connect(vertex1, edgeMaker);
    }

    private void FixedUpdate()
    {
        Proceed();
    }

    public void Sleep()
    {
        vertex1.Sleep(Princess.EventType.Rise);
        vertex1.Sleep(Princess.EventType.Fall);
        vertex2.Sleep(Princess.EventType.Rise);
        vertex2.Sleep(Princess.EventType.Fall);
    }

    void Proceed()
    {
        left.Listen();
        right.Listen();
        vertex1.Listen();
        vertex2.Listen();

        vertex1.Think();
        vertex2.Think();

        vertex1.Call();
        vertex2.Call();
    }
}
