using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Align
public class ArbitroAlign : ArbitroPonderado
{
    public override void create() {
        Align align = gameObject.AddComponent<Align>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = align;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Align>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}
