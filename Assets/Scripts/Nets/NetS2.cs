using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class NetS2 : MainBusUser
{
    public string redZoneDetectorKey = "rzd";
    public string rightHalfDetectorKey = "rhd";
    public string redZoneVertexKey = "Is_On_RedZone";
    public string rightHalfVertexKey = "Is_On_RightHalf";
    public string nodeLeftKey = "Go_Left";
    public string nodeRightKey = "Go_Right";

    public string leftBasisKey = "Left";
    public string rightBasisKey = "Right";
    public string redZoneBasisKey = "RedZone_Basis";
    public string rightHalfBasisKey = "RightHalf_Basis";

    public string callerKey = "Caller";

    Sink left, right;
    Vertex redZoneVertex, rightHalfVertex;

    private void Awake()
    {
        ConnectMainBus();

        left = new Sink();
        right = new Sink();

        ISandman sandman = new Morpheus();
        ICaller caller = new Mortar();

        mainBus.Add(caller, callerKey);

        redZoneVertex = GetVertex();
        rightHalfVertex = GetVertex();

        Basis leftBasis = new Basis(left);
        Basis rightBasis = new Basis(right);
        Basis redZoneBasis = new Basis(redZoneVertex);
        Basis rightHalfBasis = new Basis(rightHalfVertex);

        mainBus.Add(leftBasis, leftBasisKey);
        mainBus.Add(rightBasis, rightBasisKey);
        mainBus.Add(redZoneBasis, redZoneBasisKey);
        mainBus.Add(rightHalfBasis, rightHalfBasisKey);

        mainBus.Add(left, nodeLeftKey);
        mainBus.Add(right, nodeRightKey);
        mainBus.Add(redZoneVertex, redZoneVertexKey);
        mainBus.Add(rightHalfVertex, rightHalfVertexKey);

        EdgeMaker<Edge> edgeMaker = new SinglePourerEdgeMaker<Edge, Dozer>();

        redZoneVertex.Connect(redZoneVertex, edgeMaker);
        rightHalfVertex.Connect(rightHalfVertex, edgeMaker);

        redZoneVertex.Connect(left, edgeMaker);
        redZoneVertex.Connect(right, edgeMaker);
        redZoneVertex.Connect(rightHalfVertex, edgeMaker);

        rightHalfVertex.Connect(left, edgeMaker);
        rightHalfVertex.Connect(right, edgeMaker);
        rightHalfVertex.Connect(redZoneVertex, edgeMaker);

        Vertex GetVertex() => new Vertex(new Plume(10), new Plume(10), sandman, caller);
    }

    private void Start()
    {
        ISignalSource redZoneDetector = mainBus.Get<ISignalSource>(redZoneDetectorKey);
        redZoneVertex.SignalSource = redZoneDetector;

        ISignalSource rightHalfDetector = mainBus.Get<ISignalSource>(rightHalfDetectorKey);
        rightHalfVertex.SignalSource = rightHalfDetector;
    }

    private void Update()
    {
        Proceed();
    }

    public void Sleep()
    {
        redZoneVertex.Sleep();
        rightHalfVertex.Sleep();
    }

    void Proceed()
    {
        left.Listen();
        right.Listen();
        redZoneVertex.Listen();
        rightHalfVertex.Listen();

        redZoneVertex.Think();
        rightHalfVertex.Think();

        redZoneVertex.Call();
        rightHalfVertex.Call();
    }
}
