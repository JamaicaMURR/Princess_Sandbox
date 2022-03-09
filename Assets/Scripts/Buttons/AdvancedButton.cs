using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AdvancedButton : MonoBehaviour
{
    public abstract void Press();
    public abstract void Release();
}
