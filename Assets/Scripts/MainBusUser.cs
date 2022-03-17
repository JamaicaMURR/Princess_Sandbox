using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBusUser : MonoBehaviour
{
    protected MainBus mainBus;

    public string mainBusHolderName = "MainBus";

    protected virtual void ConnectMainBus() => mainBus = GameObject.Find(mainBusHolderName).GetComponent<MainBus>();
}
