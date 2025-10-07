using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroFormacion : ArbitroPonderado
{
    AgentNPC agent = null;
    float rotacionAnterior = 0;
    float angularAnterior = 0;

    public override void create()
    {
        behaviours = new BehaviourPeso[3];

        GameObject target = new GameObject();
        Agente agenteTarget = target.AddComponent<Agente>();
        agenteTarget.RadioExterior = 2.5f;
        agenteTarget.RadioInterior = 0.5f;
        agenteTarget.AnguloInterior = 2f;
        agenteTarget.AnguloExterior = 20f;
        agenteTarget.Posicion = transform.position;
        target.SetActive(false);

        behaviours[0].steeringBehaviour = gameObject.AddComponent<Arrive>();
        behaviours[0].steeringBehaviour.target = agenteTarget;
        behaviours[0].peso = 1;

        Align align = gameObject.AddComponent<Align>();
        align.target = agenteTarget;
        behaviours[1].steeringBehaviour = align;
        behaviours[1].peso = 1;

        behaviours[2].steeringBehaviour = gameObject.AddComponent<MirarHaciaDelante>();
        behaviours[2].peso = 10;

        // Esto es para que el agente se comporte mejor.
        agent = GetComponent<AgentNPC>();
        rotacionAnterior = agent.RotacionMaxima;
        angularAnterior = agent.AngularMaxima;
        agent.RotacionMaxima = 400;
        agent.AngularMaxima = 20000;
    }

    public void setTarget(Location location)
    {
        Arrive arrive = (Arrive) behaviours[0].steeringBehaviour;
        arrive.target.Posicion = location.posicion;

        Align align = (Align) behaviours[1].steeringBehaviour;
        align.target.Orientacion = location.orientacion;
    }

    public override void delete()
    {
        Destroy(behaviours[0].steeringBehaviour.target);

        for(int i = 0; i < behaviours.Length; i++)
        {
            Destroy(behaviours[i].steeringBehaviour);
        }

        // Se restauran los valores anteriores para que el resto de comportamientos no sean extraños.
        agent.RotacionMaxima = rotacionAnterior;
        agent.AngularMaxima = angularAnterior;
    }

    public override void setTargets(params object[] targets) {}
}
