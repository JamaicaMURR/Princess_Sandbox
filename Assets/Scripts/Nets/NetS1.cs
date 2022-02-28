using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using Princess.ConnectionToolkit;

public class NetS1 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string vertexKey = "Is_On_RedZone";
    public string nodeLeftKey = "Go_Left";
    public string nodeRightKey = "Go_Right";

    public string leftBasisKey = "Left";
    public string rightBasisKey = "Right";
    public string redZoneBasisKey = "Red zone basis";

    public string leftEdgeKey = "el";
    public string rightEdgeKey = "er";

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

        Basis leftBasis = new Basis(left);
        Basis rightBasis = new Basis(right);
        Basis redZoneBasis = new Basis(vertex);

        mainBus.Add(leftBasis, leftBasisKey);
        mainBus.Add(rightBasis, rightBasisKey);
        mainBus.Add(redZoneBasis, redZoneBasisKey);
        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(vertex, vertexKey);

        EdgeMaker<Edge> edgeMaker = new SinglePourerEdgeMaker<Edge, Dozer>();

        Edge leftEdge = vertex.Connect(left, edgeMaker);
        Edge rightEdge = vertex.Connect(right, edgeMaker);

        vertex.Connect(vertex, edgeMaker);

        mainBus.Add(leftEdge, leftEdgeKey);
        mainBus.Add(rightEdge, rightEdgeKey);
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);

        vertex.SignalSource = redZoneDetector;
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
