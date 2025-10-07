using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroPursue : ArbitroPonderado
{
    /// <summary>
    /// Behaviours para ArbitroPursue:
    /// 
    ///     - Pursue, peso 1.
    ///     
    /// </summary>
    public override void create()
    {
        behaviours = new BehaviourPeso[1];

        Pursue pursue = gameObject.AddComponent<Pursue>();

        behaviours[0].steeringBehaviour = pursue;
        behaviours[0].peso = 1;
    }

    public override void delete()
    {
        foreach(BehaviourPeso bh in behaviours)
        {
            Destroy(bh.steeringBehaviour);
        }

        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        Agente target = (Agente)targets[0];
        ((Pursue)(behaviours[0].steeringBehaviour)).targetDelegado = target;
    }
}
