using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroQuieto : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[1];

        Quieto quieto = gameObject.AddComponent<Quieto>();

        behaviours[0].steeringBehaviour = quieto;
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
        return;
    }
}
