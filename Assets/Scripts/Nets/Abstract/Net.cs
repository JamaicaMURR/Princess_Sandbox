using Princess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Net : MainBusUser
{
    public const string VERTEX_POSTFIX = "_Vertex";
    public const string SINK_POSTFIX = "_Sink";
    public const string BASIS_POSTFIX = "_Basis";

    public int memorySize = 10;
    public int diggerDepth = 10;

    List<Vertex> _vertices;
    List<Sink> _sinks;

    IDispenser<IMemory> _memoryDispenser;
    IDispenser<ISandman> _sandmanDispenser;
    IDispenser<IPourer> _heapPourerDispenser;
    IDispenser<Digger> _oppositeDiggerDispenser;
    IDispenser<Digger> _conservativeDiggerDispenser;
    IDispenser<IAttenuator> _attenuatorDispenser;
    IDispenser<ICooler> _coolerDispenser;

    public string NetName { get; private set; }

    protected IDispenser<IMemory> MemoryDispenser { get => _memoryDispenser; set => Setter.Init(ref _memoryDispenser, value); }
    protected IDispenser<ISandman> SandmanDispenser { get => _sandmanDispenser; set => Setter.Init(ref _sandmanDispenser, value); }
    protected IDispenser<IPourer> HeapPourerDispenser { get => _heapPourerDispenser; set => Setter.Init(ref _heapPourerDispenser, value); }
    protected IDispenser<Digger> OppositeDiggerDispenser { get => _oppositeDiggerDispenser; set => Setter.Init(ref _oppositeDiggerDispenser, value); }
    protected IDispenser<Digger> ConservativeDiggerDispenser { get => _conservativeDiggerDispenser; set => Setter.Init(ref _conservativeDiggerDispenser, value); }
    protected IDispenser<IAttenuator> AttenuatorDispenser { get => _attenuatorDispenser; set => Setter.Init(ref _attenuatorDispenser, value); }
    protected IDispenser<ICooler> CoolerDispenser { get => _coolerDispenser; set => Setter.Init(ref _coolerDispenser, value); }

    protected void InitiateNet(string netName)
    {
        ConnectMainBus();

        NetName = netName;

        _vertices = new List<Vertex>();
        _sinks = new List<Sink>();

        if(MemoryDispenser == null)
            MemoryDispenser = new UniversalDispenser<IMemory>(() => new Plume(memorySize));

        if(SandmanDispenser == null)
            SandmanDispenser = new OneInstanceDispenser<ISandman>(new Morpheus());

        if(HeapPourerDispenser == null)
            HeapPourerDispenser = new OneInstanceDispenser<IPourer>(new Mixer());

        if(OppositeDiggerDispenser == null)
            OppositeDiggerDispenser = OppositeDiggerDispenser ?? new OneInstanceDispenser<Digger>(new LastAnswerOAASFDigger(new Competent(), diggerDepth));

        if(ConservativeDiggerDispenser == null)
            ConservativeDiggerDispenser = ConservativeDiggerDispenser ?? new OneInstanceDispenser<Digger>(new NullDigger());

        if(AttenuatorDispenser == null)
            AttenuatorDispenser = AttenuatorDispenser ?? new OneInstanceDispenser<IAttenuator>(new Stairway());

        if(CoolerDispenser == null)
            CoolerDispenser = CoolerDispenser ?? new ControlledExponentialCoolersDispenser();

        AddPrototypeToMainBus(SandmanDispenser);
        AddPrototypeToMainBus(HeapPourerDispenser);
        AddPrototypeToMainBus(OppositeDiggerDispenser);
        //AddPrototypeToMainBus(ConservativeDiggerDispenser);
        AddPrototypeToMainBus(AttenuatorDispenser);
        AddPrototypeToMainBus(CoolerDispenser);

        // Internal method
        void AddPrototypeToMainBus<T>(IDispenser<T> dispenser)
        {
            if(dispenser is IPrototypeHolder)
            {
                string componentName = typeof(T).ToString().Remove(0, "Princess.".Length);

                if(componentName.StartsWith("I"))
                    componentName = componentName.Remove(0, 1);

                string busKey = $"{NetName}_{componentName}";
                object prototype = (dispenser as IPrototypeHolder).Prototype;
                mainBus.Add(prototype, busKey);
                Debug.Log($"\"{busKey}\" added to MainBus | type is {prototype.GetType().ToString().Remove(0, "Princess.".Length)}");
            }
        }
    }

    protected Vertex SpawnVertex(string busKey = null, bool addBasis = false)
    {
        Vertex vertex = new Vertex(MemoryDispenser.Dispense(),
                                   MemoryDispenser.Dispense(),
                                   SandmanDispenser.Dispense(),
                                   HeapPourerDispenser.Dispense(),
                                   OppositeDiggerDispenser.Dispense(),
                                   ConservativeDiggerDispenser.Dispense(),
                                   AttenuatorDispenser.Dispense(),
                                   CoolerDispenser.Dispense());

        string basisPrefix = AddToNet(vertex, busKey);

        if(addBasis)
            AddBasis(vertex, basisPrefix + BASIS_POSTFIX);

        return vertex;
    }

    protected Sink SpawnSink(string busKey = null, bool addBasis = false)
    {
        Sink sink = new Sink(CoolerDispenser.Dispense());

        string basisPrefix = AddToNet(sink, busKey);

        if(addBasis)
            AddBasis(sink, basisPrefix + BASIS_POSTFIX);

        return sink;
    }

    protected void AddBasis(Node node, string busKey)
    {
        mainBus.Add(new Basis(node), busKey);
        Debug.Log($"\"{busKey}\" added to MainBus");
    }

    protected string AddToNet(Node node, string busKey = null)
    {
        if(!_vertices.Contains(node) && !_sinks.Contains(node))
        {
            string keyPostfix;

            if(node is Vertex)
            {
                _vertices.Add(node as Vertex);
                keyPostfix = VERTEX_POSTFIX;
            }
            else if(node is Sink)
            {
                _sinks.Add(node as Sink);
                keyPostfix = SINK_POSTFIX;
            }
            else
                throw new NotImplementedException("Unkown node type");

            if(busKey == null)
                busKey = $"{NetName}_{_vertices.Count}{keyPostfix}";

            mainBus.Add(node, busKey);

            Debug.Log($"\"{busKey}\" added to MainBus | type is {node.GetType().ToString().Remove(0, "Princess.".Length)}");
            return busKey;
        }
        else
        {
            Debug.LogError("This node is already added");
            return string.Empty;
        }
    }

    protected void AssembleParagon(EdgeMaker edgeMaker = null)
    {
        if(edgeMaker == null)
            edgeMaker = new SinglePourerEdgeMaker<Edge, Mixer>();

        List<Vertex> connected = new List<Vertex>();

        for(int i = 0; i < _vertices.Count; i++)
        {
            for(int j = 0; j < connected.Count; j++)
            {
                _vertices[i].Connect(connected[j], edgeMaker);
                connected[j].Connect(_vertices[i], edgeMaker);
            }

            for(int k = 0; k < _sinks.Count; k++)
                _vertices[i].Connect(_sinks[k], edgeMaker);

            connected.Add(_vertices[i]);
        }

        int totalEdges = 0;

        foreach(Vertex v in _vertices)
            totalEdges += v.Edges.Length;

        Debug.Log($"Paragon assembled with {totalEdges} edges");
    }

    protected void Proceed()
    {
        _sinks.ForEach((x) => x.Listen());

        _vertices.ForEach((x) => x.Listen());
        _vertices.ForEach((x) => x.Think());
        _vertices.ForEach((x) => x.Call());
    }

    public void Sleep() => _vertices.ForEach((x) => x.Sleep());
}
