using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroVacio : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[0];
    }

    public override void delete() {}

    public override void setTargets(params object[] targets) {}
}
