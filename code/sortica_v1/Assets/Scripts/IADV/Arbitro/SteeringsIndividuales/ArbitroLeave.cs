using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroLeave : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = gameObject.AddComponent<Leave>();
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
        behaviours[0].steeringBehaviour.target = (Agente)targets[0];
    }
}
