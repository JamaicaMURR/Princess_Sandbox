using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBus : MonoBehaviour
{
    static Dictionary<string, object> _bus = new Dictionary<string, object>();

    public void Add(object o, string key) => _bus.Add(key, o);

    public T Get<T>(string key) => (T)_bus[key];
}
