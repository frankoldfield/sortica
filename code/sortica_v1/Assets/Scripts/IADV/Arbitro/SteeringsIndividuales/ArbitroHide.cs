using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de Seek
public class ArbitroHide : ArbitroPonderado
{
    public override void create() {
        Hide hide = gameObject.AddComponent<Hide>();
        behaviours = new BehaviourPeso[1];
        behaviours[0].steeringBehaviour = hide;
        behaviours[0].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<Hide>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        Hide hide = behaviours[0].steeringBehaviour as Hide;

        hide.targetHide = (Agente) targets[0];
    }
}