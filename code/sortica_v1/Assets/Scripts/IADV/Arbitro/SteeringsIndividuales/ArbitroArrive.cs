using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Arrive
public class ArbitroArrive : ArbitroPonderado
{
    public override void create() {
        Arrive arrive = gameObject.AddComponent<Arrive>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = arrive;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Arrive>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}