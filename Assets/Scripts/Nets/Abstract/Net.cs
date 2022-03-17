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

    List<Vertex> _vertices;
    List<Sink> _sinks;

    IDispenser<IMemory> _memoryDispenser;
    IDispenser<ISandman> _sandmanDispenser;
    IDispenser<IForum> _ignitorDispenser;
    IDispenser<Digger> _diggerDispenser;
    IDispenser<IAttenuator> _attenuatorDispenser;
    IDispenser<ICooler> _coolerDispenser;

    public string NetName { get; private set; }

    protected IDispenser<IMemory> MemoryDispenser { get => _memoryDispenser; set => Setter.Init(ref _memoryDispenser, value); }
    protected IDispenser<ISandman> SandmanDispenser { get => _sandmanDispenser; set => Setter.Init(ref _sandmanDispenser, value); }
    protected IDispenser<IForum> IgnitorDispenser { get => _ignitorDispenser; set => Setter.Init(ref _ignitorDispenser, value); }
    protected IDispenser<Digger> DiggerDispenser { get => _diggerDispenser; set => Setter.Init(ref _diggerDispenser, value); }
    protected IDispenser<IAttenuator> AttenuatorDispenser { get => _attenuatorDispenser; set => Setter.Init(ref _attenuatorDispenser, value); }
    protected IDispenser<ICooler> CoolerDispenser { get => _coolerDispenser; set => Setter.Init(ref _coolerDispenser, value); }

    protected void InitiateNet(string netName)
    {
        ConnectMainBus();

        NetName = netName;

        _vertices = new List<Vertex>();
        _sinks = new List<Sink>();

        MemoryDispenser = MemoryDispenser ?? new UniversalDispenser<IMemory>(() => new Plume(10));
        SandmanDispenser = SandmanDispenser ?? new OneInstanceDispenser<ISandman>(new Morpheus());
        IgnitorDispenser = IgnitorDispenser ?? new OneInstanceDispenser<IForum>(new Competent());
        DiggerDispenser = DiggerDispenser ?? new OneInstanceDispenser<Digger>(new Usual(new Competent()));
        AttenuatorDispenser = AttenuatorDispenser ?? new OneInstanceDispenser<IAttenuator>(new Stairway());
        CoolerDispenser = CoolerDispenser ?? new ControlledExponentialCoolersDispenser();

        AddPrototypeToMainBus(SandmanDispenser);
        AddPrototypeToMainBus(IgnitorDispenser);
        AddPrototypeToMainBus(DiggerDispenser);
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
                                   IgnitorDispenser.Dispense(),
                                   DiggerDispenser.Dispense(),
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
        Debug.Log($"\"{busKey}\" added to MainBus | type is {node.GetType().ToString().Remove(0, "Princess.".Length)}");
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
            edgeMaker = new SinglePourerEdgeMaker<Edge, Dozer>();

        List<Vertex> connected = new List<Vertex>();

        for(int i = 0; i < _vertices.Count; i++)
        {
            for(int j = 0; j < connected.Count; j++)
                _vertices[i].Connect(connected[j], edgeMaker);

            for(int k = 0; k < connected.Count; k++)
                _vertices[i].Connect(_sinks[k], edgeMaker);

            connected.Add(_vertices[i]);
        }
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
