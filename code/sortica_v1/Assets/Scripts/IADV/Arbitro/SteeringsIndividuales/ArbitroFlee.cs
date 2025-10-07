using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Flee
public class ArbitroFlee : ArbitroPonderado
{
    public override void create() {
        Flee flee = gameObject.AddComponent<Flee>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = flee;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Flee>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}