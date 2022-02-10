using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class Simple : IActivator
{
    public bool Activate(float summ, bool lastResult)
    {
        if(summ > 0)
            return true;
        else
            return false;
    }
}
