using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Seek
public class ArbitroSeek : ArbitroPonderado
{
    public override void create() {
        Seek seek = gameObject.AddComponent<Seek>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = seek;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Seek>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}