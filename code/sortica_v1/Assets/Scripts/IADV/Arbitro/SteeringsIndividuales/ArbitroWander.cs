using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Wander
public class ArbitroWander : ArbitroPonderado
{
    public override void create() {
        Wander wander = gameObject.AddComponent<Wander>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = wander;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Wander>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets) {}
}