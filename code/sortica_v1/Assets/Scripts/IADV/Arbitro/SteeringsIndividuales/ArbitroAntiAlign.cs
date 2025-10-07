using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de AntiAlign
public class ArbitroAntiAlign : ArbitroPonderado
{
    public override void create() {
        AntiAlign antiAlign = gameObject.AddComponent<AntiAlign>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = antiAlign;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<AntiAlign>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}
