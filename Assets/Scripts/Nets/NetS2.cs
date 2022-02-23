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
    public string rightHalfDesireControllerKey = "rhdc";

    Sink left, right;
    Vertex vertex1, vertex2;

    private void Awake()
    {
        ConnectMainBus();

        left = new Sink();
        right = new Sink();

        vertex1 = new Vertex()
        {
            RMemory = new Plume(10),
            FMemory = new Plume(10),
            Sandman = new Morpheus(),
            Caller = new Mortar()
        };

        vertex2 = new Vertex()
        {
            RMemory = new Plume(10),
            FMemory = new Plume(10),
            Sandman = new Morpheus(),
            Caller = new Mortar()
        };

        Basis leftController = new Basis(left);
        Basis rightController = new Basis(right);
        Basis redZoneDesireController = new Basis(vertex1);
        Basis rightHalfDesireController = new Basis(vertex2);

        mainBus.Add(leftController, leftControllerKey);
        mainBus.Add(rightController, rightControllerKey);
        mainBus.Add(redZoneDesireController, redZoneDesireControllerKey);
        mainBus.Add(rightHalfDesireController, rightHalfDesireControllerKey);

        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(vertex1, vertex1Key);
        mainBus.Add(vertex2, vertex2Key);
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        vertex1.SignalSource = redZoneDetector;

        ISignalSource rightHalfDetector = mainBus.Get<ISignalSource>(rightHalfDetectorKey);
        vertex2.SignalSource = rightHalfDetector;

        EdgeMaker edgeMaker = new HeavyEM();

        vertex1.Connect(vertex1, edgeMaker);
        vertex2.Connect(vertex2, edgeMaker);

        vertex1.Connect(left, edgeMaker);
        vertex1.Connect(right, edgeMaker);
        vertex1.Connect(vertex2, edgeMaker);

        vertex2.Connect(left, edgeMaker);
        vertex2.Connect(right, edgeMaker);
        vertex2.Connect(vertex1, edgeMaker);
    }

    private void Update()
    {
        Proceed();
    }

    public void Sleep()
    {
        vertex1.Sleep();
        vertex2.Sleep();
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
