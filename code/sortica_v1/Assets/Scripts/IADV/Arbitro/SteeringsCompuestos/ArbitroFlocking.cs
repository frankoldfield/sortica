using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroFlocking : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[5];
        behaviours[0].steeringBehaviour = gameObject.AddComponent<Separation>();
        behaviours[0].peso = 10;
        behaviours[1].steeringBehaviour = gameObject.AddComponent<Cohesion>();
        behaviours[1].peso = 5;
        behaviours[2].steeringBehaviour = gameObject.AddComponent<Alignment>();
        behaviours[2].peso = 4;
        behaviours[3].steeringBehaviour = gameObject.AddComponent<Wander>();
        behaviours[3].peso = 1;
        behaviours[4].steeringBehaviour = gameObject.AddComponent<MirarHaciaDelante>();
        behaviours[4].peso = 10;
    }

    public override void delete()
    {
        foreach(BehaviourPeso bh in behaviours)
        {
            Destroy(bh.steeringBehaviour);
        }

        behaviours = null;
    }

    public override void setTargets(params object[] targets) {
        AgentNPC[] agentes = new AgentNPC[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            agentes[i] = targets[i] as AgentNPC;
        }

        Separation separation = behaviours[0].steeringBehaviour as Separation;
        Cohesion cohesion = behaviours[1].steeringBehaviour as Cohesion;
        Alignment alignment = behaviours[2].steeringBehaviour as Alignment;

        separation.Targets = agentes;
        separation.threshold = 2; // Cuando están a menos de 2 unidades de distancia, quieren separarse.
        separation.decayCoefficient = 15;

        cohesion.Targets = agentes;
        cohesion.threshold = 30; // Cuando están a menos de 30 unidades de distancia, quieren juntarse.

        alignment.Targets = agentes;
        alignment.threshold = 30; // Cuando están a menos de 30 unidades de distancia, quieren alinearse.
    }
}
