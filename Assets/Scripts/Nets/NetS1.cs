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

    Sink left, right;
    Vertex vertex;

    private void Awake()
    {
        ConnectMainBus();

        left = new Sink();
        right = new Sink();

        vertex = new Vertex()
        {
            RMemory = new Plume(10),
            FMemory = new Plume(10),
            Sandman = new Morpheus(),
            Caller = new Mortar()
        };

        Basis leftController = new Basis(left);
        Basis rightController = new Basis(right);
        Basis redZoneDesireController = new Basis(vertex);

        mainBus.Add(leftController, leftControllerKey);
        mainBus.Add(rightController, rightControllerKey);
        mainBus.Add(redZoneDesireController, redZoneDesireControllerKey);
        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(vertex, vertexKey);
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);

        vertex.SignalSource = redZoneDetector;

        EdgeMaker edgeMaker = new HeavyEM();

        vertex.Connect(left, edgeMaker);
        vertex.Connect(right, edgeMaker);
    }

    private void Update()
    {
        Proceed();
    }

    public void Sleep() => vertex.Sleep();

    void Proceed()
    {
        left.Listen();
        right.Listen();
        vertex.Listen();

        vertex.Think();

        vertex.Call();
    }
}
